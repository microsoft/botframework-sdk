const NEWLINE = require('os').EOL;
const helperClasses = require('./../lufile/classes/hclasses')
const EntityTypeEnum = require('./../utils/enums/luisEntityTypes');

/**
 * Parses a Luis object into Lu Content
 * @param {Luis} luisJSON
 * @returns {string} Lu Content
 * @throws {exception} Throws on errors. exception object includes errCode and text. 
 */
const luisToLuContent = function(luisJSON){
    let fileContent = '';
    let luisObj = new helperClasses.rLuisObj();
    (luisJSON.intents || []).forEach(function(intent) {
        luisObj.intents.push(new helperClasses.intent(intent, []));
    });
    // add utterances to luisObj
    updateUtterancesList(luisJSON.utterances, luisObj.intents, 'text');
    // add patterns to luisObj
    updateUtterancesList(luisJSON.patterns, luisObj.intents, 'pattern');

    // Parse Intents
    fileContent += parseIntentsToLu(luisObj, luisJSON)
    fileContent += parseEntitiesToLu(luisJSON)
    fileContent += parseToLuPrebuiltEntities(luisJSON)
    fileContent += handlePhraseLists(luisJSON.model_features);
    fileContent += handlePhraseLists(luisJSON.phraselists);
    fileContent += parseToLuClosedLists(luisJSON)
    fileContent += parseRegExEntitiesToLu(luisJSON.regex_entities)
    // handle regexEntities in json returned from luis export api
    // similar with regex_entities
    fileContent += parseRegExEntitiesToLu(luisJSON.regexEntities)
    fileContent += parseCompositesToLu(luisJSON)
    fileContent += parsePatternAnyEntitiesToLu(luisJSON)
    return fileContent
}

const parseIntentsToLu = function(luisObj, luisJSON){
    let fileContent = ''
    fileContent += NEWLINE;
    fileContent += addAppMetaData(luisJSON);
    // Add test result if in test mode
    if (luisJSON.test === true) {
        fileContent += `> Total passed: ${luisJSON.passNumber}/${luisJSON.count}` + NEWLINE + NEWLINE
    }
    fileContent += '> # Intent definitions' + NEWLINE + NEWLINE;
    
    if(luisObj.intents.length <= 0) {
        return fileContent
    }
    // write out intents and utterances..
    luisObj.intents.forEach(function(intent) {
        // Add inherits information if any
        if (intent.intent.inherits !== undefined) {
            // > !# @intent.inherits = {name = Web.WebSearch; domain_name = Web; model_name = WebSearch}
            fileContent += '> !# @intent.inherits = name : ' + intent.intent.name;
            if (intent.intent.inherits.domain_name !== undefined) {
                fileContent += '; domain_name : ' + intent.intent.inherits.domain_name;
            }
            if (intent.intent.inherits.model_name !== undefined) {
                fileContent += '; model_name : ' + intent.intent.inherits.model_name;
            }
            fileContent += NEWLINE + NEWLINE;
        }
        // Add test result if in test mode
        if (luisJSON.test === true) {
            fileContent += `> Utterance passed in this intent: ${intent.intent.passNumber}/${intent.intent.count}` + NEWLINE
        }
        fileContent += '## ' + intent.intent.name + NEWLINE;
        fileContent += parseUtterancesToLu(intent.utterances, luisJSON)
        fileContent += NEWLINE + NEWLINE;
        if (intent.intent.features) {
            let rolesAndFeatures = addRolesAndFeatures(intent.intent);
            if (rolesAndFeatures !== '') {
                fileContent += `@ intent ${intent.intent.name.includes(' ') ? `"${intent.intent.name}"` : `${intent.intent.name}`}`;
                fileContent += rolesAndFeatures;
                fileContent += NEWLINE + NEWLINE;
            }
        }
    });
    return fileContent
}

const parseUtterancesToLu = function(utterances, luisJSON){
    let fileContent = ''
    utterances.forEach(function(utterance) {
        let updatedText = utterance.text;
        // Add test result if in test mode and utterance has test result
        if(luisJSON.test === true && utterance.predictedResult !== undefined){
            fileContent += parsePredictedResultToLu(utterance, luisJSON)
        }
        if(utterance.entities.length >= 0) {
            // update utterance for each entity
            let text = utterance.text;
            // flatten entities
            let flatEntities = [];
            Object.assign([], utterance.entities).forEach(entity => flattenEntities(entity, flatEntities));
            let sortedEntitiesList = objectSortByStartPos(flatEntities);
            // remove all children
            sortedEntitiesList.forEach(entity => delete entity.children);
            let tokenizedText = text.split('');
            // handle cases where we have both child as well as cases where more than one entity can have the same start position
            // if there are multiple entities in the same start position, then order them by composite, nDepth, regular entity
            getEntitiesByPositionList(sortedEntitiesList, tokenizedText);
            updatedText = tokenizedText.join('');
        }

        // remove duplicated whitespaces between words inside utterance to make sure they are aligned with the luis portal
        // as luis portal only keeps one whitespace between words even if you type multiple ones
        // this will benefit the comparison of lu files that are converted from local and remote luis application
        if(updatedText) fileContent += '- ' + updatedText.replace(/\s+/g, ' ') + NEWLINE;
    });
    return fileContent
}

const flattenEntities = function(entity, flatEntities)
{
    if (entity.children !== undefined && Array.isArray(entity.children) && entity.children.length !== 0) {
        entity.children.forEach(child => flattenEntities(child, flatEntities));
    }
    flatEntities.push(Object.assign({}, entity));
}

const getEntitiesByPositionList = function(entitiesList, tokenizedText) {
    (entitiesList || []).forEach(entity => {
        // does this entity have child labels?
        (entity.children || []).forEach(child => {
            getEntitiesByPositionList(child.children, tokenizedText);
            updateTokenizedTextByEntity(tokenizedText, child);
        })
        updateTokenizedTextByEntity(tokenizedText, entity);
    })
};

const updateTokenizedTextByEntity = function(tokenizedText, entity) {
    if (entity.role !== undefined) {
        tokenizedText[parseInt(entity.startPos)] = `{@${entity.role}=${tokenizedText[parseInt(entity.startPos)]}`;    
    } else {
        tokenizedText[parseInt(entity.startPos)] = `{@${entity.entity}=${tokenizedText[parseInt(entity.startPos)]}`;    
    }
    tokenizedText[parseInt(entity.endPos)] = tokenizedText[parseInt(entity.endPos)] + '}';
}

const parsePredictedResultToLu =  function(utterance, luisJSON){
    let fileContent = ''
    let updatedText = utterance.text;
    let intents = []
    // parse predicted result into the .lu content
    if(utterance.predictedResult.predictedIntents!==undefined && utterance.predictedResult.predictedIntents.length > 0){
        for(let intent of utterance.predictedResult.predictedIntents){
            intents.push(`${intent.intent}(${intent.score})`);
        }
    }
    let passText = utterance.predictedResult.IntentPass? "> PASS." : "> FAIL.";
    fileContent += passText + " Predicted intent: " + intents.join(', ')  + NEWLINE;

    if(utterance.predictedResult.predictedEntities!==undefined) {
        if (utterance.predictedResult.predictedEntities.length > 0){
            // update utterance for each entity
            let text = utterance.text;
            let sortedEntitiesList = objectSortByStartPos(utterance.predictedResult.predictedEntities);
            let tokenizedText = text.split('');
            let nonCompositesInUtterance = sortedEntitiesList.filter(entity => luisJSON.composites.find(composite => composite.name == entity.entity) == undefined);
            nonCompositesInUtterance.forEach(entity => {
                if (entity.role !== undefined) {
                    tokenizedText[parseInt(entity.startPos)] = `{@${entity.role}=${tokenizedText[parseInt(entity.startPos)]}`;    
                } else {
                    tokenizedText[parseInt(entity.startPos)] = `{@${entity.entity}=${tokenizedText[parseInt(entity.startPos)]}`;    
                }
                tokenizedText[parseInt(entity.endPos)] += `}`;
            })
            let compositeEntitiesInUtterance = sortedEntitiesList.filter(entity => luisJSON.composites.find(composite => composite.name == entity.entity) != undefined);
            compositeEntitiesInUtterance.forEach(entity => {
                if (entity.role !== undefined) {
                    tokenizedText[parseInt(entity.startPos)] = `{@${entity.role}=${tokenizedText[parseInt(entity.startPos)]}`;
                } else {
                    tokenizedText[parseInt(entity.startPos)] = `{@${entity.entity}=${tokenizedText[parseInt(entity.startPos)]}`;
                }
                tokenizedText[parseInt(entity.endPos)] += `}`;
            })
            updatedText = tokenizedText.join(''); 
        }
        let passText = utterance.predictedResult.EntityPass ? "> PASS." : "> FAIL.";
        if(updatedText) fileContent +=  passText + ' Predicted entities: ' + updatedText + NEWLINE;
    }
    return fileContent
}

const parseEntitiesToLu =  function(luisJson){
    let fileContent = ''
    if(!luisJson.entities) {
        return fileContent
    }
    
    fileContent += '> # Entity definitions' + NEWLINE + NEWLINE;
    luisJson.entities.forEach(function(entity) {
        if (!entity.children || entity.children.length === 0) {
            // Add inherits information if any
            if (entity.inherits !== undefined) {
                // > !# @intent.inherits = {name = Web.WebSearch; domain_name = Web; model_name = WebSearch}
                fileContent += '> !# @entity.inherits = name : ' + entity.name;
                if (entity.inherits.domain_name !== undefined) {
                    fileContent += '; domain_name : ' + entity.inherits.domain_name;
                }
                if (entity.inherits.model_name !== undefined) {
                    fileContent += '; model_name : ' + entity.inherits.model_name;
                }
                fileContent += NEWLINE + NEWLINE;
            }
            fileContent += `@ ${getEntityType(entity.features)} ${writeEntityName(entity.name)}`;
            fileContent += addRolesAndFeatures(entity);
            fileContent += NEWLINE + NEWLINE;
        } else {
            // handle n-depth entity
            fileContent += handleNDepthEntity(entity);
        }
    });
    fileContent += NEWLINE;
    
    return fileContent
}

const writeEntityName = function(entityName) {
    return entityName.includes(' ') ? `"${entityName}"` : `${entityName}`
}

const parseToLuPrebuiltEntities = function(luisJson){
    let fileContent = ''
    if(!luisJson.prebuiltEntities){
        return fileContent
    }
    fileContent += '> # PREBUILT Entity definitions' + NEWLINE + NEWLINE;
    luisJson.prebuiltEntities.forEach(function(entity) {
        fileContent += `@ prebuilt ${entity.name}`;
        fileContent += addRolesAndFeatures(entity);
        fileContent += NEWLINE + NEWLINE;
    });
    fileContent += NEWLINE;
    return fileContent
}

const parseToLuClosedLists = function(luisJson){
    let fileContent = ''
    if(!luisJson.closedLists){
        return fileContent
    }
    fileContent += '> # List entities' + NEWLINE + NEWLINE;
    luisJson.closedLists.forEach(function(ListItem) {
        fileContent += `@ list `;
        fileContent += ListItem.name.includes(' ') ? `"${ListItem.name}"` : `${ListItem.name}`;
        fileContent += addRolesAndFeatures(ListItem);
        if (ListItem.subLists.length !== 0) {
            fileContent += ` = `;
            fileContent += NEWLINE;
        }
        ListItem.subLists.forEach(function(list) {
            fileContent += `\t- ${list.canonicalForm} :`;
            fileContent += NEWLINE;
            list.list.forEach(function(listItem) {
                fileContent += '\t\t- ' + listItem + NEWLINE;
            });
        });
        fileContent += NEWLINE + NEWLINE;
    });
    return fileContent
}

const parseRegExEntitiesToLu = function(regex_entities){
    let fileContent = ''
    if(!regex_entities) {
        return fileContent
    }
    fileContent += '> # RegEx entities' + NEWLINE + NEWLINE; 
    regex_entities.forEach(function(regExEntity) {
        fileContent += `@ regex `;
        fileContent += regExEntity.name.includes(' ') ? `"${regExEntity.name}"` : regExEntity.name;
        fileContent += addRolesAndFeatures(regExEntity);
        if (regExEntity.regexPattern !== '') {
            fileContent += ` = /${regExEntity.regexPattern}/`;
        }
        fileContent += NEWLINE;
    });
    fileContent += NEWLINE;
    return fileContent
}

const parseCompositesToLu = function(luisJson){
    let fileContent = ''
    // add composite entities if found in source LUIS JSON
    if(!luisJson.composites || luisJson.composites.length <= 0) {
        return fileContent
    }
    fileContent += '> # Composite entities' + NEWLINE + NEWLINE; 
    luisJson.composites.forEach(composite => {
        fileContent += `@ composite `;
        fileContent += composite.name.includes(' ') ? `"${composite.name}"` : composite.name;
        fileContent += addRolesAndFeatures(composite);
        if (composite.children.length > 0) {
            fileContent += (typeof composite.children[0] == "object") ? ` = [${composite.children.map(item => item.name).join(', ')}]`: ` = [${composite.children.join(', ')}]`;
        }
        fileContent += NEWLINE;
    })
    return fileContent
}

const parsePatternAnyEntitiesToLu = function(luisJson){
    let fileContent = ''
    if(!luisJson.patternAnyEntities || luisJson.patternAnyEntities.length <= 0) {
        return fileContent;
    }
    fileContent += '> # Pattern.Any entities' + NEWLINE + NEWLINE;
    luisJson.patternAnyEntities.forEach(patternAnyEntity => {
        // Add inherits information if any
        if (patternAnyEntity.inherits !== undefined) {
            // > !# @intent.inherits = {name = Web.WebSearch; domain_name = Web; model_name = WebSearch}
            fileContent += '> !# @patternAnyEntity.inherits = name : ' + patternAnyEntity.name;
            if (patternAnyEntity.inherits.domain_name !== undefined) {
                fileContent += '; domain_name : ' + patternAnyEntity.inherits.domain_name;
            }
            if (patternAnyEntity.inherits.model_name !== undefined) {
                fileContent += '; model_name : ' + patternAnyEntity.inherits.model_name;
            }
            fileContent += NEWLINE + NEWLINE;
        }
        // For back compat we will only write this if the pattern.any has inherits information.
        fileContent += `@ patternany `;
        fileContent += patternAnyEntity.name.includes(' ') ? `"${patternAnyEntity.name}"` : patternAnyEntity.name;
        fileContent += addRolesAndFeatures(patternAnyEntity);
        fileContent += NEWLINE;
    })
    return fileContent;
}

/**
 * Helper to handle phrase lists both in the new and old property.
 * @param {Object[]} collection 
 */
const handlePhraseLists = function(collection) {
    let fileContent = ''
    if(!collection) {
        return fileContent
    }
    fileContent = '> # Phrase list definitions' + NEWLINE + NEWLINE;
    collection.forEach(function(entity) {
        let flags = '';
        fileContent += `@ phraselist `;
        fileContent += entity.name.includes(' ') ? `"${entity.name}"` : `${entity.name}`;
        fileContent += `${(entity.mode ? `(interchangeable)` : ``)}`;
        if (entity.activated !== undefined && !entity.activated) flags += `disabled`;
        if (entity.enabledForAllModels !== undefined) {
            if (entity.enabledForAllModels === true) {
                flags += (flags !== '') ? `, enabledForAllModels` : `enabledForAllModels`;
            } else {
                flags += (flags !== '') ? `, disabledForAllModels` : `disabledForAllModels`;
            }
        } 
        if (flags !== '') fileContent += ` ${flags}`;
        if (entity.words && entity.words !== '') {
            fileContent += ` = ${NEWLINE}\t- ${entity.words}`;
        }
        fileContent += NEWLINE + NEWLINE;
    });
    fileContent += NEWLINE;

    return fileContent;
}
/**
 * Helper to add application inforamtion metadata
 * @param {Object} LUISJSON 
 */
const addAppMetaData = function(LUISJSON) {
    let fileContent = '';
    if (LUISJSON.name) fileContent += `> !# @app.name = ${LUISJSON.name}` + NEWLINE;
    if (LUISJSON.desc) fileContent += `> !# @app.desc = ${LUISJSON.desc}` + NEWLINE;
    if (LUISJSON.versionId) fileContent += `> !# @app.versionId = ${LUISJSON.versionId}` + NEWLINE;
    if (LUISJSON.culture) fileContent += `> !# @app.culture = ${LUISJSON.culture}` + NEWLINE;
    if (LUISJSON.luis_schema_version) fileContent += `> !# @app.luis_schema_version = ${LUISJSON.luis_schema_version}` + NEWLINE;
    if (LUISJSON.settings) {
        LUISJSON.settings.forEach(setting => {
            fileContent += `> !# @app.settings.${setting.name} = ${setting.value}` + NEWLINE;
        })
    }
    if (LUISJSON.tokenizerVersion) fileContent += `> !# @app.tokenizerVersion = ${LUISJSON.tokenizerVersion}` + NEWLINE;
    return fileContent === '' ? fileContent : `> LUIS application information` + NEWLINE + fileContent + NEWLINE + NEWLINE;
}
/**
 * Helper function to handle nDepth entity definition
 * @param {Object} entity 
 */
const handleNDepthEntity = function(entity) {
    let fileContent = '';
    const BASE_TAB_STOP = 1;
    fileContent += `@ ${getEntityType(entity.features)} ${writeEntityName(entity.name)}`;
    fileContent += addRolesAndFeatures(entity);
    fileContent += NEWLINE;
    fileContent += addNDepthChildDefinitions(entity.children, BASE_TAB_STOP, fileContent) + NEWLINE + NEWLINE
    return fileContent;
}
/**
 * Recursive helper function to add child definitions. 
 * @param {Object[]} childCollection 
 * @param {number} tabStop 
 * @param {string} fileContent 
 */
const addNDepthChildDefinitions = function(childCollection, tabStop, fileContent) {
    let myFileContent = '';
    (childCollection || []).forEach(child => {
        myFileContent += "".padStart(tabStop * 4, ' ');
        myFileContent += `- @ ${getEntityType(child.features)} ${writeEntityName(child.name)}`;
        myFileContent += addRolesAndFeatures(child);
        myFileContent += NEWLINE;
        if (child.children && child.children.length !== 0) {
            myFileContent += addNDepthChildDefinitions(child.children, tabStop + 1, myFileContent);
        }
    });
    return myFileContent;
}
const getEntityType = function(features) {
    // find constraint
    let constraint = (features || []).find(feature => feature.isRequired == true);
    if (constraint !== undefined) {
        return constraint.modelName;
    } else {
        return EntityTypeEnum.ML;
    }
}
/**
 * Helper to construt role and features list for an entity
 * @param {Object} entity 
 * @returns {String} file content to include.
 */
const addRolesAndFeatures = function(entity) {
    let roleAndFeatureContent = ''
    if (entity.roles && entity.roles.length > 0) {
        roleAndFeatureContent += ` ${entity.roles.length > 1 ? `hasRoles` : `hasRole`} `;
        entity.roles.forEach(item => {
            roleAndFeatureContent += item.includes(' ') ? `"${item}",` : `${item},`;
        })
    }
    roleAndFeatureContent = roleAndFeatureContent.substring(0, roleAndFeatureContent.length - 1);
    if (!entity.features || entity.features.length <= 0) {
        return roleAndFeatureContent
    }

    let featuresList = new Array();
    entity.features.forEach(item => {
        if (item.featureName) featuresList.push(item.featureName);
        if (item.modelName) {
            if (item.isRequired !== undefined) {
                if (item.isRequired !== true) 
                    featuresList.push(item.modelName);
            } else {
                featuresList.push(item.modelName);
            }
        }
    })
    if (featuresList.length > 0) {
        roleAndFeatureContent += ` ${featuresList.length > 1 ? `usesFeatures` : `usesFeature`} `;
        featuresList.forEach(feature => {
            roleAndFeatureContent += feature.includes(' ') ? `"${feature}",` : `${feature},`;
        });
        roleAndFeatureContent = roleAndFeatureContent.substring(0, roleAndFeatureContent.length - 1);
    }
    
    //${featuresList.join(',')}`;
    return roleAndFeatureContent
}

/**
 * helper function to add utterances to collection if it does not exist
 * @param {object[]} tgtCollection target collection of utterance objects
 * @param {object []} srcCollection source collection of utterance objects
 * @param {string} attribute attribute to check on and copy over
 * @returns {void}
 */
const updateUtterancesList = function (srcCollection, tgtCollection, attribute) {
    (srcCollection || []).forEach(srcItem => {
        let matchInTarget = tgtCollection.find(item => item.intent.name == srcItem.intent);
        if(!matchInTarget || matchInTarget.utterances.length === 0) {
            addUtteranceToCollection(attribute, srcItem, matchInTarget);
            return;
        }
        if(!matchInTarget.utterances.find(item => item.text == srcItem[attribute])) {
            addUtteranceToCollection(attribute, srcItem, matchInTarget);
            return;
        }
    });
}
/**
 * helper function to add utterances to collection based on src type (pattern or utterance)
 * @param {string} attribute attribute to check on and copy over
 * @param {object} srcItem source object
 * @param {object []} matchInTarget target collection of objects
 * @returns {void}
 */
const addUtteranceToCollection = function (attribute, srcItem, matchInTarget) {
    if(attribute === 'text') {
        matchInTarget.utterances.push(srcItem); 
    } else {
        matchInTarget.utterances.push(new helperClasses.uttereances(srcItem.pattern.replace('{', '{@'),srcItem.intent,[]));
    }
}

/**
 * helper function sort entities list by starting position
 * @param {object} objectArray array of entity objects
 * @returns {object} sorted entities array by start position
 */
const objectSortByStartPos = function (objectArray) {
    let ObjectByStartPos = objectArray.slice(0);
    ObjectByStartPos.sort(function(a,b) {
        if (a.startPos === b.startPos)
            return a.endPos - b.endPos; 
        return a.startPos - b.startPos;
    });
    return ObjectByStartPos;
}

module.exports = luisToLuContent