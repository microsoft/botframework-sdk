# ![Bot Framework](./docs/media/BotFrameworkSDK.png)

### [What's new with Bot Framework?](https://docs.microsoft.com/en-us/azure/bot-service/what-is-new?view=azure-bot-service-4.0)

Bot Framework provides the most comprehensive experience for building conversation applications.

With the [Bot Framework SDK](#bot-framework-sdk-v4), developers can build bots that converse free-form or with guided interactions including using simple text or rich cards that contain text, images, and action buttons.

Developers can model and build sophisticated conversation using their favorite programming languages including C#, JS, Python and Java or using [Bot Framework Composer](https://aka.ms/bfcomposer), an open-source, visual authoring canvas for developers and multi-disciplinary teams to design and build conversational experiences with Language Understanding, QnA Maker and sophisticated composition of bot replies (Language Generation).

Checkout the [Bot Framework ecosystem](#bot-framework-ecosystem) section to learn more about other tooling and services related to the Bot Framework SDK.

![Bot Framework SDK](./docs/media/GitHubBannerV2.gif)

## Quicklinks
| [Bot Framework Composer](https://aka.ms/bfcomposer)  | [C# Repo](https://github.com/Microsoft/botbuilder-dotnet)  | [JS Repo](https://github.com/Microsoft/botbuilder-js)  | [Python Repo](https://github.com/Microsoft/botbuilder-python) |  [Java Repo](https://github.com/Microsoft/botbuilder-java) | [BF CLI](https://github.com/Microsoft/botframework-cli) |

## Bot Framework SDK v4
The Bot Framework SDK v4 is an [open source SDK][1a] that enable developers to model and build sophisticated conversation using their favorite programming language.

|   | C#  | JS  | Python |  Java |
|---|:---:|:---:|:------:|:-----:|
|Stable Release |[packages][1] | [packages][2] | [packages][3] | [4.13.0][3a]|
|Docs | [docs][5] |[docs][5] |[docs][5]  | |
|Samples |[.NET Core][6], [WebAPI][10] |[Node.js][7] , [TypeScript][8], [es6][9]  | [Python][111] | |

[1a]:https://github.com/microsoft/botframework-sdk/#readme
[1]:https://github.com/Microsoft/botbuilder-dotnet/#packages
[2]:https://github.com/Microsoft/botbuilder-js#packages
[3]:https://github.com/microsoft/botbuilder-python/#packages
[3a]:https://github.com/microsoft/botbuilder-java/blob/4.13/README.md
[4]:https://github.com/Microsoft/botbuilder-java#packages
[5]:https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[6]:https://github.com/Microsoft/BotBuilder-Samples/tree/main/samples/csharp_dotnetcore
[7]:https://github.com/Microsoft/BotBuilder-Samples/tree/main/samples/javascript_nodejs
[8]:https://github.com/Microsoft/BotBuilder-Samples/tree/main/samples/typescript_nodejs
[9]:https://github.com/Microsoft/BotBuilder-Samples/tree/main/samples/javascript_es6
[10]:https://github.com/Microsoft/BotBuilder-Samples/tree/main/samples/csharp_webapi
[111]:https://github.com/microsoft/BotBuilder-Samples/tree/main/samples/python

<a name="V4-whats-new"></a>
### Current Bot Framework SDK v4 preview features

- [Dialog Generation][40] :: The Bot Framework has a rich collection of conversational building blocks, but creating a bot that feels natural to converse with requires understanding and coordinating across language understanding, language generation and dialog management. To simplify this process and capture best practices, we've created the bf-generate plugin for the Bot Framework CLI tool. The generated dialogs make use of event-driven adaptive dialogs with a rich and evolving set of capabilities.

[40]:https://github.com/microsoft/BotBuilder-Samples/tree/main/experimental/generation#readme

## Channels and Adapters
There are two ways to connect your bot to a client experience:
* **Azure Bot Service Channel** - Language and SDK independent support via Azure Bot Service
* **Bot Framework SDK Adapter** -  A per language Adapter component

| Client          | Azure Channel  | C# Adapter           | JS Adapter        | Python Adapter     |
|-----------------|:--------------:|:--------------------:|:-----------------:|:------------------:|
| Microsoft Teams | [Azure][55abs] |                      |                   |                    |
| Direct Line     | [Azure][55abs] |                      |                   |                    |
| Web Chat        | [Azure][55abs] |                      | [Botkit][55bk]    |                    |
| Skype           | [Azure][55abs] |                      |                   |                    |
| Email           | [Azure][55abs] |                      |                   |                    |
| Facebook        | [Azure][55abs] |  [SDK][55sdkcsfb]    | [Botkit][55bk]    |                    |
| Slack           | [Azure][55abs] |  [SDK][55sdkcsslack] | [Botkit][55bk]    | [SDK][55sdkpyslack] |
| Kik             | [Azure][55abs] |                      |                   |                    |
| Telegram        | [Azure][55abs] |                      |                   |                    |
| Line            | [Azure][55abs] |                      |                   |                    |
| GroupMe         | [Azure][55abs] |                      |                   |                    |
| Twilio (SMS)    | [Azure][55abs] | [SDK][55sdkcstwilio] | [Botkit][55bk]    |                    |
| Alexa Skills    |                | [Community][55cs]    | [Community][55js] |                    |
| Google Actions  |                | [Community][55cs]    | [Community][55js] |                    |
| Google Hangouts |                |                      | [Botkit][55bk]    |                    |
| WebEx           |                | [SDK][55sdkcswebex]    | [Botkit][55bk]    |                    |
| WhatsApp (Infobip)  |            | [Community][55cs]    |                   |                    |
| Zoom            |                | [Community][55cs]    |                   |                    |
| RingCentral     |                | [Community][55cs]    |                   |                    |
| Cortana         | [Azure][55abs] |                      |                   |                    |
| Console         |                |                      | [Community][55js] |                    |

[55abs]:https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0
[55cs]:https://github.com/BotBuilderCommunity/botbuilder-community-dotnet#adapters
[55js]:https://github.com/BotBuilderCommunity/botbuilder-community-js#adapters
[55bk]:https://github.com/howdyai/botkit#readme
[55sdkcsfb]:https://github.com/microsoft/botbuilder-dotnet/tree/main/libraries/Adapters/Microsoft.Bot.Builder.Adapters.Facebook
[55sdkcsslack]:https://github.com/microsoft/botbuilder-dotnet/tree/main/libraries/Adapters/Microsoft.Bot.Builder.Adapters.Slack
[55sdkcstwilio]:https://github.com/microsoft/botbuilder-dotnet/tree/main/libraries/Adapters/Microsoft.Bot.Builder.Adapters.Twilio
[55sdkcswebex]:https://github.com/microsoft/botbuilder-dotnet/tree/main/libraries/Adapters/Microsoft.Bot.Builder.Adapters.Webex
[55sdkpyslack]:https://github.com/microsoft/botbuilder-python/tree/main/libraries/botbuilder-adapters-slack

## Community Open Source Projects
The following open source communities make various components available to extend your bot application, including adapters, recognizers, dialogs and middleware.

|                            |       C#       | JavaScript |    Python      |    Java      |
|----------------------------|:--------------:|:----------:|:--------------:|:------------:|
| [Bot Framework Community][56] | [C#][56dotnet] | [JavaScript][56js] | [Python][56py] | [Java][56ja] |
| [Botkit][56bk]             |                | [JavaScript][56bk] |                |              |

[56]:https://github.com/botbuildercommunity#readme
[56dotnet]:https://github.com/botbuildercommunity/botbuilder-community-dotnet#readme
[56js]:https://github.com/botbuildercommunity/botbuilder-community-js#readme
[56py]:https://github.com/botbuildercommunity/botbuilder-community-python#readme
[56ja]:https://github.com/botbuildercommunity/botbuilder-community-java#readme
[56bk]:https://github.com/howdyai/botkit#readme

## Questions and Help
If you have questions about Bot Framework SDK or using Azure Bot Service, we encourage you to reach out to the community and Azure Bot Service dev team for help.
- For questions which fit the Stack Overflow format ("how does this work?"), we monitor the both [Azure Bot Service](https://stackoverflow.com/questions/tagged/azure-bot-service) and [Bot Framework](https://stackoverflow.com/questions/tagged/botframework) tags (search [both](https://stackoverflow.com/questions/tagged/azure-bot-service+or+botframework))
- You can also tweet/follow [@msbotframework](https://twitter.com/msbotframework)
- Join the conversation on **[Gitter](https://gitter.im/Microsoft/BotBuilder)**.

See all of the available support options **[here](https://docs.microsoft.com/en-us/bot-framework/resources-support)**.


## Issues and feature requests
We track functional issues and features asks for the Bot Framework SDK, tools and Azure Bot Service in a variety of locations. If you have found an issue or have a feature request, please submit an issue to the below repositories.

| Item                        | Description                                                                                        | Link                              |
|-----------------------------|----------------------------------------------------------------------------------------------------|-----------------------------------|
| SDK v4 .NET                 | core bot runtime for .NET, connectors, middleware, dialogs, prompts, LUIS and QnA                  | [File an issue][70csissues]       |
| SDK v4 JavaScript           | core bot runtime for Typescript/Javascript, connectors, middleware, dialogs, prompts, LUIS and QnA | [File an issue][70jsissues]       |
| SDK v4 Python               | core bot runtime for Python, connectors, middleware, dialogs, prompts, LUIS and QnA                | [File an issue][70pyissues]       |
| SDK v4 Java                 | core bot runtime for Java, connectors, middleware, dialogs, prompts, LUIS and QnA                  | [File an issue][70jaissues]       |
| Bot Framework Composer      | bot framework composer electron and web app                                                        | [File an issue][70composerissues] |
| Bot Framework CLI           | bot framework cli tools                                                                            | [File an issue][70cliissues]  |
| Webchat                     | bot framework web chat tool                                                                        | [File an issue][70webchatissues]  |

[70csissues]:https://github.com/Microsoft/botbuilder-dotnet/issues
[70jsissues]:https://github.com/Microsoft/botbuilder-js/issues
[70pyissues]:https://github.com/Microsoft/botbuilder-python/issues
[70jaissues]:https://github.com/Microsoft/botbuilder-java/issues
[70cliissues]:https://github.com/Microsoft/botframework-cli/issues
[70composerissues]:https://github.com/Microsoft/botframework-composer/issues
[70webchatissues]:https://github.com/Microsoft/botframework-webchat/issues

## Prior releases

- Bot Builder v3 SDK has been migrated to the [Bot Framework SDK V3](https://github.com/microsoft/botbuilder-v3) repository.

## Bot Framework ecosystem

- [Bot Framework Composer](#Bot-Framework-Composer)
- [Bot Framework Solutions](#Bot-Framework-Solutions)
- [Botkit](#Botkit)
- [Azure Bot Service](#Azure-Bot-Service)
- [Bot Framework Emulator](#Bot-Framework-Emulator)
- [Bot Framework Web Chat](#Bot-Framework-Web-Chat)
- [Bot Framework Tools](#Bot-Framework-CLI-Tools)
- [Language Understanding](#Language-Understanding)
- [QnA Maker](#QnA-Maker)
- [Dispatch](#Dispatch)
- [Speech Services](#Speech-Services)
- [Adaptive cards](#Adaptive-Cards)
- [Analytics](#Analytics)

### Bot Framework Composer
[Bot Framework Composer](https://github.com/microsoft/BotFramework-Composer/blob/main/README.md) is an integrated development tool for developers and multi-disciplinary teams to build bots and conversational experiences with the Microsoft Bot Framework. Within this tool, you'll find everything you need to build a sophisticated conversational experience.

### Botkit
[Botkit][100] is a developer tool and SDK for building chat bots, apps and custom integrations for major messaging platforms. Botkit bots `hear()` triggers, `ask()` questions and `say()` replies. Developers can use this syntax to build dialogs - now cross compatible with the latest version of Bot Framework SDK.

In addition, Botkit brings with it 6 platform adapters allowing Javascript bot applications to communicate directly with messaging platforms: [Slack][102], [Webex Teams][103], [Google Hangouts][104], [Facebook Messenger][105], [Twilio][106], and [Web chat][107].

Botkit is part of Microsoft Bot Framework and is released under the [MIT Open Source license][101]

[100]:https://github.com/howdyai/botkit#readme
[101]:https://github.com/howdyai/botkit/blob/main/LICENSE.md
[102]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-slack#readme
[103]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-webex#readme
[104]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-hangouts#readme
[105]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-facebook#readme
[106]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-twilio-sms#readme
[107]:https://github.com/howdyai/botkit/tree/main/packages/botbuilder-adapter-web#readme

### Bot Framework Virtual Assistant Solution Accelerator

The [Bot Framework Solutions repository](https://github.com/Microsoft/botframework-solutions#readme) is home to the [Virtual Assistant Solution Accelerator](https://aka.ms/bfvadocs), which provides a set of templates, solution accelerators and skills to help build sophisticated conversational experiences.

- [**Virtual Assistant.**](https://aka.ms/bfvadocs) Customers and partners have a significant need to deliver a conversational assistant tailored to their brand, personalized to their users, and made available across a broad range of canvases and devices. <br/><br/>  This brings together all of the supporting components and greatly simplifies the creation of a new bot project including: basic conversational intents, Dispatch integration, QnA Maker, Application Insights and an automated deployment.

- [**Skills.**](https://aka.ms/bfskillsdocs) A library of re-usable conversational skill building-blocks enabling you to add functionality to a Bot. We currently provide: Calendar, Email, Task, Point of Interest, Automotive, Weather and News skills. Skills include LUIS models, Dialogs, and integration code delivered in source code form to customize and extend as required.

- [**Analytics.**](https://github.com/Microsoft/AI/blob/main/docs/readme.md#analytics) Gain key insights into your bot’s health and behavior with the Bot Framework Analytics solutions, which includes: sample Application Insights queries, and Power BI dashboards to understand the full breadth of your bot’s conversations with users.

### Azure Bot Service
Azure Bot Service enables you to host intelligent, enterprise-grade bots with complete ownership and control of your data. Developers can register and connect their bots to users on Skype, Microsoft Teams, Cortana, Web Chat, and more. [[Docs][28]]

* **Direct Line JS Client**: If you want to use the Direct Line channel in Azure Bot Service and are not using the WebChat client, the Direct Line JS client can be used in your custom application. [[Readme][30]]

<a name="ABS-whats-new"></a>

* **Direct Line Speech Channel**: We are bringing together the Bot Framework and Microsoft's Speech Services to provide a channel that enables streamed speech and text bi-directionally from the client to the bot application.  To sign up, add the 'Direct Line Speech' channel to your Azure Bot Service.

[27]:https://azure.microsoft.com/en-us/services/bot-service/
[28]:https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[29]:https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0
[30]:https://github.com/Microsoft/BotFramework-DirectLineJS/blob/main/README.md

* **Better isolation for your Bot - Direct Line App Service Extension** : The Direct Line App Service Extension can be deployed as part of a VNET, allowing IT administrators to have more control over conversation traffic and improved latency in conversations due to reduction in the number of hops. Get started with Direct Line App Service Extension here. A VNET lets you create your own private space in Azure and is crucial to your cloud network as it offers isolation, segmentation, and other key benefits.

### Bot Framework Emulator
The [Bot Framework Emulator][60] is a  cross-platform desktop application that allows bot developers to test and debug bots built using the Bot Framework SDK. You can use the Bot Framework Emulator to test bots running locally on your machine or to connect to bots running remotely. [[Download latest][61] | [Docs][62]]

[60]:https://github.com/Microsoft/BotFramework-Emulator#readme
[61]:https://github.com/Microsoft/BotFramework-Emulator/releases/latest
[62]:https://docs.microsoft.com/en-us/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-4.0

### Bot Framework Web Chat
The Bot Framework [Web Chat][23] is a highly customizable web-based client chat control for Azure Bot Service that provides the ability for users to interact with your bot directly in a web page. [[Stable release][24] | [Docs][25]  | [Samples][26]]

[23]:https://github.com/Microsoft/BotFramework-WebChat#readme
[24]:https://www.npmjs.com/package/botframework-webchat
[25]:https://github.com/Microsoft/BotFramework-WebChat/tree/main/doc
[26]:https://github.com/Microsoft/BotFramework-WebChat/tree/main/samples

### Bot Framework CLI
The Bot Framework CLI Tools hosts the [open source](https://github.com/microsoft/botframework-cli) cross-platform Bot Framework CLI tool, designed to support building robust end-to-end development workflows. The Bot Framework CLI tool replaced the [legacy standalone tools](https://github.com/Microsoft/BotBuilder-Tools) used to manage bots and related services. BF CLI aggregates the collection of cross-platform tools into one cohesive and consistent interface.

## Related Services

### Language Understanding
A machine learning-based service to build natural language experiences. Quickly create enterprise-ready, custom models that continuously improve. Language Understanding Service(LUIS) allows your application to understand what a person wants in their own words. [[Docs][31] | [Add language understanding to your bot][32]]

[18]:https://github.com/Microsoft/botbuilder-tools/tree/main/packages/LUIS#readme
[19]:https://github.com/Microsoft/botbuilder-tools/tree/main/packages/QnAMaker#readme
[30]:https://www.luis.ai
[31]:https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/Home
[32]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&branch=pr-en-us-1325&tabs=csharp

### QnA Maker
[QnA Maker][33] is a cloud-based API service that creates a conversational, question-and-answer layer over your data. With QnA Maker, you can build, train and publish a simple question and answer bot based on FAQ URLs, structured documents, product manuals or editorial content in minutes. [[Docs][34]  | [Add qnamaker to your bot][35]]

[33]:https://www.qnamaker.ai/
[34]:https://aka.ms/qnamaker-docs-home
[35]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna?view=azure-bot-service-4.0&branch=pr-en-us-1325&tabs=cs

### Dispatch
Dispatch tool lets you build language models that allow you to dispatch between disparate components (such as QnA, LUIS and custom code). [[Readme](https://github.com/Microsoft/botbuilder-tools/blob/main/packages/Dispatch#readme)]

### Speech Services
Speech Services convert audio to text, perform speech translation and text-to-speech with the unified Speech services. With the speech services, you can integrate speech into your bot, create custom wake words, and author in multiple languages. [[Docs](https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/)]

### Adaptive Cards
[Adaptive Cards](https://adaptivecards.io) are an open standard for developers to exchange card content in a common and consistent way,
and are used by Bot Framework developers to create great cross-channel conversatational experiences.

* **Open framework, native performance** - A simple open card format enables an ecosystem of shared tooling, seamless integration between apps, and native cross-platform performance on any device.
* **Speech enabled from day one** - We live in an exciting era where users can talk to their devices. Adaptive Cards embrace this new world and were designed from the ground up to support these new experiences.

## Contributing

See our [contributing guidelines](https://github.com/microsoft/botframework-sdk/blob/main/Contributing.md).

## Reporting Security Issues
Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the [MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).

Copyright (c) Microsoft Corporation. All rights reserved.
