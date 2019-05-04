# ![Bot Framework SDK](./docs/media/FrameWorkSDK.png)

# Bot Framework SDK 

The Bot Framework SDK v4, part of the [Bot Framework](https://github.com/microsoft/botframework), provides the most comprehensive experience for building conversation applications. With the Bot Framework SDK, developers can easily model and build sophisticated conversation using their favorite programming languages. With the Bot Framework SDK, you can build bots that converse free-form or your bot can also have more guided interactions where it provides the user choices or possible actions. The conversation can use simple text or more complex rich cards that contain text, images, and action buttons. You can add natural language interactions and questions and answers, which let your users interact with your bots in a natural way.

## Bot Framework SDK v4
The Bot Framework SDK v4 is an [open source SDK][1a] that enable developers to model and build sophisticated conversation using their favorite programming language.


|   | C#  | JS  | Python |  Java | 
|---|:---:|:---:|:------:|:-----:|
|Stable Release |[4.4.3][1] | [4.4.0][2] | [4.4.0b1 (preview)][3] | [4.0.0a6 (preview)][3a]|
|Docs | [docs][5] |[docs][5] |  | |
|Samples |[.NET Core][6], [WebAPI][10] |[Node.js][7] , [TypeScript][8], [es6][9]  | [Python][111] | | 

[1a]:https://github.com/microsoft/botframework-sdk
[1]:https://github.com/Microsoft/botbuilder-dotnet/#packages
[2]:https://github.com/Microsoft/botbuilder-js#packages
[3]:https://github.com/Microsoft/botbuilder-python#packages
[3a]:https://github.com/Microsoft/botbuilder-java#packages
[4]:https://github.com/Microsoft/botbuilder-java#packages
[5]:https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[6]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore
[7]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/javascript_nodejs
[8]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/javascript_typescript
[9]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/javascript_es6
[10]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/csharp_webapi
[111]:https://github.com/Microsoft/botbuilder-python/tree/master/samples

<a name="V4-whats-new"></a>
### Bot Framework SDK v4 - (New! - version 4.5 preview)
The Bot Framework SDK v4 - Version 4.5 new capabilites in preview. 

- [Adaptive Dialog][47] | [docs][48] | [C# samples][49] :: Adaptive Dialogs enable developers to build conversations that can be dynamically changed as the conversation progresses.  Traditionally developers have mapped out the entire flow of a conversation up front, which limits the flexibility of the conversation.  Adaptive dialogs allow them to be more flexible, to respond to changes in context and insert new steps or entire sub-dialogs into the conversation as it progresses. Additionally as with other SDK V4 concepts, we have defined adaptive dialogs such that they can be defined via [declarative][50] that are interpreted at runtime; which allows us to have tooling on top of this and integrate with services. 

- [Language Generation][43] | [docs][44] | [C# samples][45] :: Learning from our customers experiences and bringing together capabilities first implemented by Cortana and Cognition teams, we are introducing Language Generation; which allows the developer to extract the embedded strings from their code and resource files and manage them through a Language Generation runtime and file format.  Language Generation enable customers to define multiple variations on a phrase, execute simple expressions based on context, refer to conversational memory, and over time will enable us to bring additional capabilities all leading to a more natural conversational experience.

- [Common Expression Language][40] | [api][41] :: Both Adaptive dialogs and Language Generation rely on and use a common expression language to power bot conversations.


[40]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language
[41]:https://github.com/Microsoft/BotBuilder-Samples/blob/master/experimental/common-expression-language/api-reference.md
[43]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
[44]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation/docs
[45]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation/csharp_dotnetcore
[46]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation/javascript_nodejs/13.core-bot
[47]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/adaptive-dialog
[48]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/adaptive-dialog/docs
[49]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/adaptive-dialog/csharp_dotnetcore
[50]:https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/adaptive-dialog/declarative

## Community Extensions
Adapters and plugins from the open source community are available to extend your bot application.

| Name | Description
|--- |---
| [botbuilder-community](https://github.com/botbuildercommunity) | A collection of community-powered plugins and extensions
| [Botkit](https://github.com/howdyai/botkit#readme) | A Javascript extension to Bot Framework SDK with new syntax for creating triggers and managing dialogs
| [botbuilder-adapter-slack](https://github.com/howdyai/botkit/tree/master/packages/botbuilder-adapter-slack) | A platform adapter for Slack
| [botbuilder-adapter-webex](https://github.com/howdyai/botkit/tree/master/packages/botbuilder-adapter-webex) | A platform adapter for Webex Teams
| [botbuilder-adapter-hangouts](https://github.com/howdyai/botkit/tree/master/packages/botbuilder-adapter-hangouts) | A platform adapter for Google
| [botbuilder-adapter-twilio-sms](https://github.com/howdyai/botkit/tree/master/packages/botbuilder-adapter-twilio-sms) | A platform adapter for Twilio SMS
| [botbuilder-adapter-facebook](https://github.com/howdyai/botkit/tree/master/packages/botbuilder-adapter-facebook) | A platform adapter for Facebook Messenger

## Questions and Help 
If you have questions about Bot Framework SDK or using Azure Bot Service, we encourage you to reach out to the community and Azure Bot Service dev team for help.
- For questions which fit the Stack Overflow format ("how does this work?"), we monitor the both [Azure Bot Service](https://stackoverflow.com/questions/tagged/azure-bot-service) and [Bot Framework](https://stackoverflow.com/questions/tagged/botframework) tags (search [both](https://stackoverflow.com/questions/tagged/azure-bot-service+or+botframework))
- You can also tweet/follow [@msbotframework](https://twitter.com/msbotframework) 

Join the conversation on **[Gitter](https://gitter.im/Microsoft/BotBuilder)**.

See all the support options **[here](https://docs.microsoft.com/en-us/bot-framework/resources-support)**.


## Issues and feature requests 
We track functional issues and features asks for and Bot Builder and Azure Bot Service in a variety of locations. If you have found an issue or have a feature request, please submit an issue to the below repositories.

|Item&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|Description|Link&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|
|----|:-----|---------|
|SDK v4 .NET| core bot runtime for .NET, connectors, middleware, dialogs, prompts, LUIS and QnA| [File an issue](https://github.com/Microsoft/botbuilder-dotnet/issues) |
|SDK v4 JS| core bot runtime for Typescript/Javascript, connectors, middleware, dialogs, prompts, LUIS and QnA | [File an issue](https://github.com/Microsoft/botbuilder-js/issues) |
|SDK v4 Python| core bot runtime for Python, connectors, middleware, dialogs, prompts, LUIS and QnA | [File an issue](https://github.com/Microsoft/botbuilder-python/issues) |
|SDK v4 Java| core bot runtime for Java, connectors, middleware, dialogs, prompts, LUIS and QnA | [File an issue]( https://github.com/Microsoft/botbuilder-java/issues)|


## Prior releases

- Bot Builder v3 SDK has been migrated to the [Bot Framework SDK V3](https://github.com/microsoft/botbuilder-v3) repository.


## Prior releases
- Bot Builder V3 SDK has been migrated to the [Bot Framework SDK v3](https://github.com/microsoft/botbuilder-v3) repository.
- [BotKit SDK](http://botkit.ai) is a popular SDK which joined the Microsoft Bot Framework family and is built on top of the Bot Framework SDK V4.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting Security Issues
Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the [MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).

## License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE) License.
