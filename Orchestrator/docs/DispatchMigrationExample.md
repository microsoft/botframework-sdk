# Example Migration from LUIS Dispatch to Orchestrator

The following article describes how to migrate a legacy *dispatch* based solution to [Orchestrator][3] routing.

In  [NLP With Dispatch][2] C# Sample we use LUIS as the top intent arbitrator to redirect intent processing to subsequent language understanding services, LUIS and QnAMaker.  Recall that the top routing was performed by using *dispatch* CLI to create a language model combining the subsequent LUIS and QnAMaker models, and creating an aggregate top LUIS application to be used in the bot logic to further delegate utterances to the detected language service. 

Here, we will modify that sample to use Orchestrator in place of the top LUIS arbitrator as follows:



<p align="center">
  <img width="450" src="./media/dispatch-logic-flow.png" />
</p>




# Prerequisites

* Complete the [NLP With Dispatch][2] C# Sample to serve as the starting point.
  * Have access to create & use [LUIS][4] and [QnAMaker][5] services.
  * See [Dispatch Sample documentation][1] for full details.
* Install [BF CLI][6]
* Bot project must target x64 platform
* Install latest supported version of [Visual C++ runtime](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)



# Migration Walkthrough

Start with fully working  [NLP With Dispatch][2] C# Sample including all language artifacts (output of dispatch CLI). 



## Prepare
* Add your LUIS application language model(s) and/or QnA Maker knowledge bases(s) as Orchestrator data sources

```
> md CognitiveModels
@rem -v <luis-app-version> is optional, default to 0.1
@rem --routingName is optional, default to l_<luis-app-name> found in the exported lu file
@rem sample luis-endpoint: https://westus.api.cognitive.microsoft.com
> bf orchestrator:add -t luis --id <luis-app-id> --endpoint <luis-endpoint> -k <luis-subscription-key> -v <luis-app-version> --routingName l_<luis-app-name>

@rem --routingName is optional, default to q_<kb-id>
> bf orchestrator:add -t qna --id <kb-id> -k <qna-subscription-key> --routingName q_<kb-name>
```

* Add the ```Microsoft.Bot.Builder.AI.Orchestrator``` assembly and dependencies to your project from nuget package manager.

## Create Orchestrator Language model

* Get Orchestrator base model

* Create Orchestrator snapshot with your previously exported LUIS application language model(s) and/or QnA Maker knowledgebase(s).

  Example:

```
> md model
> bf orchestrator:basemodel:get --out model

> md generated
> bf orchestrator:create --in CognitiveModels --model model --out generated
"Processing c:\\...\\CognitiveModels\\datasources\\l_<luis-app-name>.lu...\n"
"Processing c:\\...\\CognitiveModels\\datasources\\q_<kb-name>.qna...\n"
"Snapshot written to c:\\...\\generated\\Orchestrator.blu"
```

* If you have previously used [**Dispatch CLI tool**][8] to create and refresh LUIS based Dispatch application, you could simply pass in path to the **.dispatch** file (created by Dispatch CLI tool) to bf:orchestrator:create command and skip the bf:orchestrator:add steps above.

  Example:

  ```
  > md generated
  > cd CognitiveModels
  > bf orchestrator:create --in c:\\LuisDispatchBot.dispatch --model model --out ..\\generated
  "Processing c:\\...\\CognitiveModels\\datasources\\l_<luis-app-name>.lu...\n"
  "Processing c:\\...\\CognitiveModels\\datasources\\q_<kb-name>.qna...\n"
  "Snapshot written to c:\\...\\generated\\Orchestrator.blu"
  ```

- To refresh *Orchestrator snapshot* with latest changes from your LUIS application(s) or QnAMaker knowledge base(s),  simply rerun the bf orchestrator:create command above or run the following command from the same directory previous create command was run from.

  Example:

  ```
  > bf orchestrator:create --refresh
  "Processing c:\\...\\CognitiveModels\\datasources\\l_<luis-app-name>.lu...\n"
  "Processing c:\\...\\CognitiveModels\\datasources\\q_<kb-name>.qna...\n"
  "Snapshot written to c:\\...\\generated\\Orchestrator.blu"
  ```



## Modify Settings

* Inspect your LUIS and QnAMaker configurations and modify ```appsettings.json```  so as to specify the two subsequent LUIS applications.

* Add configuration for the top Orchestrator arbitrator (i.e. the new dispatcher)

  ```
  {
    "Logging": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
  
    "MicrosoftAppId": "",
    "MicrosoftAppPassword": "",
  
    "QnAKnowledgebaseId": "--same as in original sample--",
    "QnAEndpointKey": "--same as in original sample--",
    "QnAEndpointHostName": "--same as in original sample--",
  
    "LuisHomeAutomationAppId": "--pick from generated NLPDispatchSample14.dispatch--",
    "LuisWeatherAppId": "--pick from generated NLPDispatchSample14.dispatch--",
    "LuisAPIKey": "--same as in original sample--",
    "LuisAPIHostName": "Old: westus. New: https://westus.api.cognitive.microsoft.com/",
  
    "Orchestrator": {
      "ModelPath": ".\\model",
      "SnapshotPath": ".\\generated\\Orchestrator.blu"
    },
  
    "AllowedHosts": "*"
  }
  ```



## Modify Startup Configuration

* The new ```Startup.cs``` file shall include Orchestrator initialization.
* Modify ```(I)BotService.cs``` to expose Orchestrator as dispatch.
* Add class for Orchestrator configuration settings.

**Startup.cs**

```
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {

        public OrchestratorConfig OrchestratorConfig { get; }

        public Startup(IConfiguration configuration)
        {
            OrchestratorConfig = configuration.GetSection("Orchestrator").Get<OrchestratorConfig>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<OrchestratorRecognizer>(InitializeOrchestrator());

            // Create the bot services (Orchestrator, LUIS, QnA) as a singleton.
            services.AddSingleton<IBotServices, BotServices>();

            // Create the bot as a transient.
            services.AddTransient<IBot, DispatchBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }


        private OrchestratorRecognizer InitializeOrchestrator()
        {
            string modelPath = Path.GetFullPath(OrchestratorConfig.ModelPath);
            string snapshotPath = Path.GetFullPath(OrchestratorConfig.SnapshotPath);
            OrchestratorRecognizer orc = new OrchestratorRecognizer()
            {
                ModelPath = modelPath,
                SnapshotPath = snapshotPath
            };
            return orc;
        }

    }
}

```



**IBotServices.cs**

```
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.AI.QnA;

namespace Microsoft.BotBuilderSamples
{
    public interface IBotServices
    {
        LuisRecognizer LuisHomeAutomationRecognizer { get; }
        LuisRecognizer LuisWeatherRecognizer { get; }

        OrchestratorRecognizer Dispatch { get; }
        QnAMaker SampleQnA { get; }
    }
}
```



**BotServices.cs**

```
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {

        public OrchestratorRecognizer Dispatch { get; private set; }
        public QnAMaker SampleQnA { get; private set; }
        public LuisRecognizer LuisHomeAutomationRecognizer { get; private set; }
        public LuisRecognizer LuisWeatherRecognizer { get; private set; }

        public BotServices(IConfiguration configuration, OrchestratorRecognizer dispatcher)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            // If includeApiResults is set to true, the full response from the LUIS api (LuisResult)
            // will be made available in the properties collection of the RecognizerResult
            LuisHomeAutomationRecognizer = CreateLuisRecognizer(configuration, "LuisHomeAutomationAppId");
            LuisWeatherRecognizer = CreateLuisRecognizer(configuration, "LuisWeatherAppId");

            Dispatch = dispatcher;

            SampleQnA = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAEndpointKey"],
                Host = configuration["QnAEndpointHostName"]
            });
        }

        private LuisRecognizer CreateLuisRecognizer(IConfiguration configuration, string appIdKey)
        {
            var luisApplication = new LuisApplication(
                configuration[appIdKey],
                configuration["LuisAPIKey"],
                configuration["LuisAPIHostName"]);

            // Set the recognizer options depending on which endpoint version you want to use.
            // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            return new LuisRecognizer(recognizerOptions);
        }

    }
}

```



**OrchestratorConfig.cs**

```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    public class OrchestratorConfig
{
        public string SnapshotPath { get; set; }
        public string ModelPath { get; set; }
    }
}

```



## Modify Bot Logic



**Bots\DispatchBot.cs**

```
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.BotFramework.Orchestrator;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class DispatchBot : ActivityHandler
    {
        private readonly ILogger<DispatchBot> _logger;
        private readonly IBotServices _botServices;

        public DispatchBot(IBotServices botServices, ILogger<DispatchBot> logger)
        {
            _logger = logger;
            _botServices = botServices;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            // Top intent tell us which cognitive service to use.
            var allScores = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
            // var topIntent = allScores.Intents.First().Key;
             var topIntent = allScores.GetTopScoringIntent();
             string Intent = topIntent.intent;


            
            // Next, we call the dispatcher with the top intent.
            await DispatchToTopIntentAsync(turnContext, Intent, allScores, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            const string WelcomeText = "Type a greeting, or a question about the weather to get started.";

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"**NLP with Orchestrator Sample**\n\n{WelcomeText}"), cancellationToken);
                }
            }
        }

        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
        {
            string props;

            switch (intent)
            {
                case "l_HomeAutomation":
                    props = GetRecognizerProperties("Home Automation", recognizerResult.Properties);
                    await turnContext.SendActivityAsync(MessageFactory.Text(props), cancellationToken);
                    await ProcessHomeAutomationAsync(turnContext, cancellationToken);
                    break;
                case "l_Weather":
                    props = GetRecognizerProperties("Weather", (Dictionary<string, object>)recognizerResult.Properties);
                    await turnContext.SendActivityAsync(MessageFactory.Text(props), cancellationToken);
                    await ProcessWeatherAsync(turnContext, cancellationToken);
                    break;
                case "q_sample-qna":
                    props = GetRecognizerProperties("QnAMaker", (Dictionary<string, object>)recognizerResult.Properties);
                    await turnContext.SendActivityAsync(MessageFactory.Text(props), cancellationToken);
                    await ProcessSampleQnAAsync(turnContext, cancellationToken);
                    break;
                default:
                    _logger.LogInformation($"Dispatch unrecognized intent: {intent}.");
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Dispatch unrecognized intent: {intent}."), cancellationToken);
                    break;
            }
        }

        private async Task ProcessHomeAutomationAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessHomeAutomationAsync");

            // Retrieve LUIS result for HomeAutomation.
            var recognizerResult = await _botServices.LuisHomeAutomationRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var result = recognizerResult.Properties["luisResult"] as LuisResult;

            var topIntent = result.TopScoringIntent.Intent;

            await turnContext.SendActivityAsync(MessageFactory.Text($"HomeAutomation top intent: {topIntent}.\n\n"), cancellationToken);
            // await turnContext.SendActivityAsync(MessageFactory.Text($"HomeAutomation intents detected\n\n{string.Join("\n\n* ", result.Intents.Select(i => i.Intent))}"), cancellationToken);
            if (result.Entities.Count > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"HomeAutomation entities were found in the message:\n\n{string.Join("\n\n* ", result.Entities.Select(i => i.Entity))}"), cancellationToken);
            }
        }

        private async Task ProcessWeatherAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessWeatherAsync");

            // Retrieve LUIS result for Weather.
            var recognizerResult = await _botServices.LuisWeatherRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var result = recognizerResult.Properties["luisResult"] as LuisResult;
            var topIntent = result.TopScoringIntent.Intent;

            await turnContext.SendActivityAsync(MessageFactory.Text($"ProcessWeather top intent: {topIntent}.\n\n"), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"ProcessWeather Intents detected:\n\n{string.Join("\n\n* ", result.Intents.Select(i => i.Intent))}"), cancellationToken);
            if (result.Entities.Count > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"ProcessWeather entities were found in the message:\n\n{string.Join("\n\n* ", result.Entities.Select(i => i.Entity))}"), cancellationToken);
            }
        }


        private string GetRecognizerProperties(string Domain, IDictionary<string, object> recognizerResult)
        {
            StringBuilder resultString = new StringBuilder();

            resultString.Append($"**Dispatch: {Domain}**\n\nProperties:\n\n");

            IList<BotFramework.Orchestrator.Result> result = (IList < BotFramework.Orchestrator.Result >)recognizerResult["result"];
            for (var i = 0; i < result.Count; i++)
            {
                BotFramework.Orchestrator.Result r = result[i];
                resultString.Append($"---\n\n* Closest Text: {r.ClosestText}\n\n");
                resultString.Append($"* Label: {r.Label.Name}\n\n");
                resultString.Append($"* Score: {r.Score}\n\n");
            }

            return resultString.ToString();
        }


        private async Task ProcessSampleQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessSampleQnAAsync");

            var results = await _botServices.SampleQnA.GetAnswersAsync(turnContext);
            if (results.Any())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
            }
        }
    }
}

```



 # Summary

Compile and run. The sample will use Orchestrator to arbitrate ("dispatch") to the corresponding language service, LUIS or QnAMaker which will process the intent and respond to the user.




# References
* [NLP With Dispatch Sample][2]
* [Dispatch Sample documentation][1]

[1]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs "Legacy dispatch MSDocs"
[2]:https://github.com/microsoft/BotBuilder-Samples/tree/f6af59378a89864754ef36822c4352aad648f426/samples/csharp_dotnetcore/14.nlp-with-dispatch
[3]:https://aka.ms/bf-orchestrator "Orchestrator"
[4]:https://luis.ai "LUIS"
[5]:https://qnamaker.ai "QnAMaker"
[6]:https://github.com/microsoft/botframework-cli "BF CLI"
[7]:https://github.com/microsoft/botframework-cli/tree/main/packages/orchestrator "Orchestrator plugin"
[8]:https://github.com/microsoft/botbuilder-tools/tree/master/packages/Dispatch "Dispatch CLI"
