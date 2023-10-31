# Bot Framework -- Activity

## Abstract

The Bot Framework Activity schema is an application-level representation of conversational actions made by humans and automated software. The schema includes provisions for communicating text, multimedia, and non-content actions like social interactions and typing indicators.

This schema is used within the Bot Framework protocol and is implemented by Microsoft chat systems and by interoperable bots and clients from many sources.

## Table of Contents

1. [Introduction](#introduction)
2. [Basic activity structure](#basic-activity-structure)
3. [Message activity](#message-activity)
4. [Contact relation update activity](#contact-relation-update-activity)
5. [Conversation update activity](#conversation-update-activity)
6. [End of conversation activity](#end-of-conversation-activity)
7. [Event activity](#event-activity)
8. [Invoke activity](#invoke-activity)
9. [Installation update activity](#installation-update-activity)
10. [Message delete activity](#message-delete-activity)
11. [Message update activity](#message-update-activity)
12. [Message reaction activity](#message-reaction-activity)
13. [Suggestion activity](#suggestion-activity)
13. [Trace activity](#trace-activity)
14. [Typing activity](#typing-activity)
15. [Handoff activity](#handoff-activity)
16. [Command activity](#command-activity)
17. [Command result activity](#command-result-activity)
18. [Complex types](#complex-types)
19. [References](#references)
20. [Appendix I - Changes](#appendix-i---changes)
21. [Appendix II - Non-IRI entity types](#appendix-ii---non-iri-entity-types)
22. [Appendix III - Protocols using the Invoke activity](#appendix-iii---protocols-using-the-invoke-activity)
23. [Appendix IV - Priming format](#appendix-iv---priming-format)
24. [Appendix V - Caller ID values](#appendix-v---caller-id-values)
25. [Appendix VI - Protocols using the Command activity](#appendix-vi---protocols-using-the-command-activity)

## Introduction

### Overview

The Bot Framework Activity schema represents conversational behaviors made by humans and automated software within chat applications, email, and other text interaction programs. Each activity object includes a type field and represents a single action: most commonly, sending text content, but also including multimedia attachments and non-content behaviors like a "like" button or a typing indicator.

This document provides meanings for each type of activity, and describes the required and optional fields that may be included. It also defines the roles of the client and server, and provides guidance on which fields are mastered by each participant, and which may be ignored.

There are three roles of consequence in this specification: clients, which send and receive activities on behalf of users; bots, which send and receive activities and are typically automated; and the channel, which stores and forwards activities between clients and bots.

Although this specification requires activities to be transmitted between roles, the exact nature of that transmission is not described here. This may be found instead in the companion Bot Framework Protocol specification [[1](#references)].

For compactness, visual interactive cards are not defined in this specification. Instead, these are defined within the [Bot Framework Cards](botframework-cards.md) [[10](#references)] and [Adaptive Cards](https://adaptivecards.io) [[11](#references)] specifications. These cards, and other undefined card types, may be included as attachments within Bot Framework activities.

### Requirements

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL NOT", "SHOULD", "SHOULD NOT", "RECOMMENDED", "MAY", and "OPTIONAL" in this document are to be interpreted as described in [RFC 2119](https://tools.ietf.org/html/rfc2119) [[2](#references)].

An implementation is not compliant if it fails to satisfy one or more of the MUST or REQUIRED level requirements for the protocols it implements. An implementation that satisfies all the MUST or REQUIRED level and all the SHOULD level requirements for its protocols is said to be "unconditionally compliant"; one that satisfies all the MUST level requirements but not all the SHOULD level requirements for its protocols is said to be "conditionally compliant."

Explanatory text describes the intent. Editors are encouraged to omit normative statements from explanatory text. All [numbered requirements](#numbered-requirements) are normative. All examples are non-normative.

### Numbered requirements

Lines beginning with markers of the form `AXXXX` are specific requirements designed to be referenced by number in discussion outside of this document. They do not carry any more or less weight than normative statements made outside of `AXXXX` lines.

`A1000`: Editors of this specification MAY add new `AXXXX` requirements. They SHOULD find numeric `AXXXX` values that preserve the document's flow.

`A1001`: Editors MUST NOT renumber existing `AXXXX` requirements.

`A1002`: Editors MAY delete or revise `AXXXX` requirements. If revised, editors SHOULD retain the existing `AXXXX` value if the topic of the requirement remains largely intact.

`A1003`: Editors SHOULD NOT reuse retired `AXXXX` values. A list of deleted values MAY be maintained at the end of this document.

### Terminology

activity
> An action expressed by a bot, a channel, or a client that conforms to the Activity schema.

channel
> Software that sends and receives activities, and transforms them to and from chat or application behaviors. Channels are the authoritative store for activity data.

bot
> Software that sends and receives activities, and generates automated, semi-automated, or entirely manual responses. Bots have endpoints that are registered with channels.

client
> Software that sends and receives activities, typically on behalf of human users. Clients do not have endpoints.

sender
> Software transmitting an activity.

receiver
> Software accepting an activity.

endpoint
> A programmatically addressable location where a bot or channel can receive activities.

address
> An identifier or address where a user or bot can be contacted.

field
> A named value within an activity or nested object.

### Overall organization

The activity object is a flat list of name/value pairs, some of which are primitive objects, and some of which are complex (nested). The activity object is commonly expressed in the JSON format, but can also be projected into in-memory data structures in .Net or JavaScript.

The activity `type` field controls the meaning of the activity and the fields contained within it. Depending on the role that a participant is playing (client, bot, or channel), each field is mandatory, optional, or ignored. For example, the `id` field is mastered by the channel, and is mandatory in some circumstances, but ignored if it is sent by a bot.

Fields that describe the identity of the activity and any participants, such as the `type` and `from` fields, are shared across all activities. In many programming languages, it is convenient to organize these fields on a core base type from which other, more specific, activity types derive.

When storing or transmitting activities, some fields may be duplicated within the transport mechanism. For example, if an activity is transmitted via HTTP POST to a URL that includes the conversation ID, the receiver may infer its value without requiring it to be present within the activity body. This document merely describes the abstract requirements for these fields, and it is up to the controlling protocol to establish whether the values must be explicitly declared or if implicit or inferred values are allowed.

When a bot or client sends an activity to a channel, it is typically a request for the activity to be recorded. When a channel sends an activity to a bot or client, it is typically a notification that the activity has already been recorded.

## Basic activity structure

This section defines the requirements for the basic structure of the activity object.

Activity objects include a flat list of name/value pairs, called fields. Fields may be primitive types. JSON is used as the common interchange format and although not all activities must be serialized to JSON at all times, they must be serializable to it. This allows implementations to rely on a simple set of conventions for handling known and unknown activity fields.

`A2001`: Activities MUST be serializable to the JSON format defined in [RFC 4627](http://www.ietf.org/rfc/rfc4627.txt) [[15](#references)], including adherence to e.g. field uniqueness constraints.

`A2002`: Receivers MAY allow improperly-cased field names, although this is not required. Receivers MAY reject activities that do not include fields with the proper casing.

`A2004`: Unless otherwise noted, senders SHOULD NOT include empty string values for string fields.

`A2005`: Unless otherwise noted, senders MAY include additional fields within the activity or any nested complex objects. Receivers MUST accept fields they do not understand.

`A2006`: Receivers SHOULD accept events of types they do not understand.

This document defines data types for fields used within the Activity object. These type definitions include a syntactic type (e.g. `string` or `complex type`) and in the case of strings, an optional format (e.g. [ISO 8601 date time format](https://www.iso.org/iso-8601-date-and-time-format.html) [[3](#references)]).

`A2007`: Senders MUST adhere to data type definitions contained in this document.

`A2003`: Receivers SHOULD reject activities that contain field values whose types do not match the data types described in this specification.

### Type

The `type` field controls the meaning of each activity, and are by convention short strings (e.g. "`message`"). Senders may define their own application-layer types, although they are encouraged to choose values that are unlikely to collide with future well-defined values. If senders use URIs as type values, they SHOULD NOT implement URI ladder comparisons to establish equivalence.

`A2010`: Activities MUST include a `type` field, with string value type.

`A2011`: Two `type` values are equivalent only if they are ordinally identical.

`A2012`: A sender MAY generate activity `type` values not defined in this document.

`A2013`: A channel SHOULD reject activities of type it does not understand.

`A2014`: A bot or client SHOULD ignore activities of type it does not understand.

### Channel ID

The `channelId` field establishes the channel and authoritative store for the activity. The value of the `channelId` field is of type string.

`A2020`: Activities MUST include a `channelId` field, with string value type.

`A2021`: Two `channelId` values are equivalent only if they are ordinally identical.

`A2022`: A channel MAY ignore or reject any activity it receives without an expected `channelId` value.

### ID

The `id` field establishes the identity for the activity once it has been recorded in the channel. Activities in-flight that have not yet been recorded do not have identities. Not all activities are assigned identities (for example, a [typing activity](#typing-activity) may never be assigned an `id`.) The value of the `id` field is of type string.

`A2030`: Channels SHOULD include an `id` field if it is available for that activity.

`A2031`: Clients and bots SHOULD NOT include an `id` field in activities they generate.

For ease of implementation, it should be assumed that other participants do not have sophisticated knowledge of activity IDs, and that they will use only ordinal comparison to establish equivalency.

For example, a channel may use hex-encoded GUIDs for each activity ID. Even though GUIDs encoded in uppercase are logically equivalent to GUIDs encoded in lowercase, senders SHOULD NOT use these alternative encodings when possible. The normalized version of each ID is established by the authoritative store, the channel.

`A2032`: When generating `id` values, senders SHOULD choose values whose equivalency can be established by ordinal comparison. However, senders and receivers MAY allow logical equivalence of two values that are not ordinally equivalent if they have special knowledge of the circumstances.

The `id` field is designed to allow de-duplication, but this is prohibitive in most applications.

`A2033`: Receivers MAY de-duplicate activities by ID, however senders SHOULD NOT rely on receivers performing this de-duplication.

### Timestamp

The `timestamp` field records the exact UTC time when the activity occurred. Due to the distributed nature of computing systems, the important time is when the channel (the authoritative store) records the activity. The time when a client or bot initiated an activity may be transmitted separately in the `localTimestamp` field. The value of the `timestamp` field is an ISO 8601 [[3](#references)] encoded datetime within a string.

`A2040`: Channels SHOULD include a `timestamp` field if it is available for that activity.

`A2041`: Clients and bots SHOULD NOT include a `timestamp` field in activities they generate.

`A2042`: Clients and bots SHOULD NOT use `timestamp` to reject activities, as they may appear out-of-order. However, they MAY use `timestamp` to order activities within a UI or for downstream processing.

`A2043`: Senders SHOULD always use encode the value of `timestamp` fields as UTC, and they SHOULD always include Z as an explicit UTC mark within the value.

### Local timezone
The `localTimezone` field expresses the timezone where the activity was generated. The value of the `localTimezone` field is a time zone name (zone entry) per the IANA Time Zone database. [[15](#references)]

`A2055`: Clients MAY include the `localTimezone` in their activities.

`A2056`: Channels SHOULD preserve `localTimezone` when forwarding activities from a sender to recipient(s).

`A2057`: A receiver MAY ignore `localTimezone` values it does not understand.

### Local timestamp

The `localTimestamp` field expresses the datetime and timezone offset where the activity was generated. This may be different from the UTC `timestamp` where the activity was recorded. The value of the `localTimestamp` field is an ISO 8601 [[3](#references)] encoded datetime within a string.

When both the `localTimezone` and `localTimestamp` fields are included in an activity, the interpretation is to first convert the value of the localTimestamp to UTC and then apply a conversion to the local timezone.

`A2050`: Clients and bots MAY include the `localTimestamp` field in their activities. They SHOULD explicitly list the timezone offset within the encoded value.

`A2051`: Channels SHOULD preserve `localTimestamp` when forwarding activities from a sender to recipient(s).

### From

The `from` field describes which client, bot, or channel generated an activity. The value of the `from` field is a complex object of the [Channel account](#channel-account) type.

The `from.id` field identifies who generated an activity. Most commonly, this is another user or bot within the system. In some cases, the `from` field identifies the channel itself.

`A2060`: Channels MUST include the `from` and `from.id` fields when generating an activity.

`A2061`: Bots and clients SHOULD include the `from` and `from.id` fields when generating an activity. A channel MAY reject an activity due to missing `from` and `from.id` fields.

The `from.name` field is optional and represents the display name for the account within the channel. Channels SHOULD include this value so clients and bots can populate their UIs and backend systems. Bots and clients SHOULD NOT send this value to channels that have a central record of this store, but they MAY send this value to channels that populate the value on every activity (e.g. an email channel).

`A2062`: Channels SHOULD include the `from.name` field if the `from` field is present and `from.name` is available.

`A2063`: Bots and clients SHOULD NOT include the `from.name` field unless it is semantically valuable within the channel.

### Recipient

The `recipient` field describes which client or bot is receiving this activity. This field is only meaningful when an activity is transmitted to exactly one recipient; it is not meaningful when it is broadcast to multiple recipients (as happens when an activity is sent to a channel). The purpose of the field is to allow the recipient to identify themselves. This is helpful when a client or bot has more than one identity within the channel. The value of the `recipient` field is a complex object of the [Channel account](#channel-account) type.

`A2070`: Channels MUST include the `recipient` and `recipient.id` fields when transmitting an activity to a single recipient.

`A2071`: Bots and clients SHOULD NOT include the `recipient` field when generating an activity. The exception to this is when sending a [Suggestion activity](#suggestion-activity), in which case the recipient MUST identify the user that should receive the suggestion.

The `recipient.name` field is optional and represents the display name for the account within the channel. Channels SHOULD include this value so clients and bots can populate their UIs and backend systems.

`A2072`: Channels SHOULD include the `recipient.name` field if the `recipient` field is present and `recipient.name` is available.

### Conversation

The `conversation` field describes the conversation in which the activity exists. The value of the `conversation` field is a complex object of the [Conversation account](#conversation-account) type.

`A2080`: Channels, bots, and clients MUST include the `conversation` and `conversation.id` fields when generating an activity.

The `conversation.name` field is optional and represents the display name for the conversation if it exists and is available.

`A2081`: Channels SHOULD include the `conversation.name` and `conversation.isGroup` fields if they are available.

`A2082`: Bots and clients SHOULD NOT include the `conversation.name` field unless it is semantically valuable within the channel.

`A2083`: Bots and clients SHOULD NOT include the `conversation.isGroup` and `conversation.converationType` fields in activities they generate.

`A2084`: Channels SHOULD include the `conversation.conversationType` field if more than one value is defined for the channel. Channels SHOULD NOT include the field if there is only one possible value.

### Reply to ID

The `replyToId` field identifies the prior activity to which the current activity is a reply. This field allows threaded conversation and comment nesting to be communicated between participants. `replyToId` is valid only within the current conversation. (See [relatesTo](#relates-to) for references to other conversations.) The value of the `replyToId` field is a string.

`A2090`: Senders SHOULD include `replyToId` on an activity when it is a reply to another activity.

`A2091`: A channel MAY reject an activity if its `replyToId` does not reference a valid activity within the conversation.

`A2092`: Bots and clients MAY omit `replyToId` if it knows the channel does not make use of the field, even if the activity being sent is a reply to another activity.

### Entities

The `entities` field contains a flat list of metadata objects pertaining to this activity. Unlike attachments (see the [attachments](#attachments) field), entities do not necessarily manifest as user-interactable content elements, and are intended to be ignored if not understood. Senders may include entities they think may be useful to a receiver even if they are not certain the receiver can accept them. The value of each `entities` list element is a complex object of the [Entity](#entity) type.

`A2100`: Senders SHOULD omit the `entities` field if it contains no elements.

`A2101`: Senders MAY send multiple entities of the same type, provided the entities have distinct meaning.

`A2102`: Senders MUST NOT include two or more entities with identical types and contents.

`A2103`: Senders and receivers SHOULD NOT rely on specific ordering of the entities included in an activity.

`A2104`: Receivers MUST ignore entities whose types they do not understand.

`A2105`: Receivers SHOULD ignore entities whose type they understand but are unable to process due to e.g. syntactic errors.

### Channel data

Extensibility data in the activity schema is organized principally within the `channelData` field. This simplifies plumbing in SDKs that implement the protocol. The format of the `channelData` object is defined by the channel sending or receiving the activity.

`A2200`: Channels SHOULD NOT define `channelData` formats that are JSON primitives (e.g., strings, ints). Instead, they SHOULD define `channelData` as a complex type, or leave it undefined.

`A2201`: If the `channelData` format is undefined for the current channel, receivers SHOULD ignore the contents of `channelData`.

### Caller ID

In some cases, it's important to record where an activity was sent. The `callerId` field is a string containing an [IRI](https://tools.ietf.org/html/rfc3987) [[4](#references)] identifying the caller of a bot, described in more detail in [Appendix V](#appendix-v---caller-id-values). This field is not intended to be transmitted over the wire, but is instead populated by bots and clients based on cryptographically verifiable data that asserts the identity of the callers (e.g. tokens).

`A2250`: Senders SHOULD NOT populate the `callerId` field.

`A2251`: Receivers SHOULD discard any data included in the `callerId` field on the wire.

`A2252`: Bots SHOULD, after receiving an Activity, populate its `callerId` field with an identifier described in [Appendix V](#appendix-v---caller-id-values)

### Service URL

Activities are frequently sent asynchronously, with separate transport connections for sending and receiving traffic. The `serviceUrl` field is used by channels to denote the URL where replies to the current activity may be sent. The value of the `serviceUrl` field is of type string.

`A2300`: Channels MUST include the `serviceUrl` field in all activities they send to bots.

`A2301`: Channels SHOULD NOT include the `serviceUrl` field to clients who demonstrate they already know the channel's endpoint.

`A2302`: Bots and clients SHOULD NOT populate the `serviceUrl` field in activities they generate.

`A2302`: Channels MUST ignore the value of `serviceUrl` in activities sent by bots and clients.

`A2304`: Channels SHOULD use stable values for the `serviceUrl` field as bots may persist them for long periods.

## Message activity

Message activities represent content intended to be shown within a conversational interface. Message activities may contain text, speech, interactive cards, and binary or unknown attachments; typically channels require at most one of these for the message activity to be well-formed.

Message activities are identified by a `type` value of `message`.

### Text

The `text` field contains text content, either in the Markdown format, XML, or as plain text. The format is controlled by the [`textFormat`](#text-Format) field as is plain if unspecified or ambiguous. The value of the `text` field is of type string.

`A3000`: The `text` field MAY contain an empty string to indicate sending text without contents.

`A3001`: Channels SHOULD handle `markdown`-formatted text in a way that degrades gracefully for that channel.

### Text format

The `textFormat` field denotes whether the [`text`](#text) field should be interpreted as [Markdown](https://daringfireball.net/projects/markdown/) [[5](#references)], plain text, or XML. The value of the `textFormat` field is of type string, with defined values of `markdown`, `plain`, and `xml`. The default value is `plain`. This field is not designed to be extended with arbitrary values.

The `textFormat` field controls additional fields within attachments etc. This relationship is described within those fields, elsewhere in this document.

`A3010`: If a sender includes the `textFormat` field, it SHOULD only send defined values.

`A3011`: Senders SHOULD omit `textFormat` if the value is `plain`.

`A3012`: Receivers SHOULD interpret undefined values as `plain`.

`A3013`: Bots and clients SHOULD NOT send the value `xml` unless they have prior knowledge that the channel supports it, and the characteristics of the supported XML dialect.

`A3014`: Channels SHOULD NOT send `markdown` or `xml` contents to bots.

`A3015`: Channels SHOULD accept `textformat` values of at least `plain` and `markdown`.

`A3016`: Channels MAY reject `textformat` of value `xml`.

### Locale

The `locale` field communicates the language code of the [`text`](#text) field. The value of the `locale` field is an [IETF BCP-47](https://tools.ietf.org/html/bcp47) [[19](#references)] language tag within a string.

`A3020`: Receivers SHOULD treat missing and unknown values of the `locale` field as unknown.

`A3021`: Receivers SHOULD NOT reject activities with unknown locale.

### Speak

The `speak` field indicates how the activity should be spoken via a text-to-speech system. The field is only used to customize speech rendering when the default is deemed inadequate. It replaces speech synthesis for any content within the activity, including text, attachments, and summaries. The value of the `speak` field is either plain text or [SSML](https://www.w3.org/TR/speech-synthesis/) [[7](#references)] encoded within a string.

`A3030`: The `speak` field MAY contain an empty string to indicate no speech should be generated.

`A3031`: Receivers unable to generate speech SHOULD ignore the `speak` field.

`A3033`: Receivers SHOULD NOT use XML DTD or schema resolution to include remote resources from outside the communicated XML fragment.

`A3034`: Channels SHOULD NOT send the `speak` field to bots.

`A3035`: Receivers generating speech from an Activity with a missing or null `speak` field SHOULD render message contents such as [`text`](#text) and [`summary`](#summary) instead.

### Input hint

The `inputHint` field indicates whether or not the generator of the activity is anticipating a response. This field is used predominantly within channels that have modal user interfaces, and is typically not used in channels with continuous chat feeds. The value of the `inputHint` field is of type string, with defined values of `accepting`, `expecting`, and `ignoring`. The default value is `accepting`.

`A3040`: If a sender includes the `inputHint` field, it SHOULD only send defined values.

`A3041`: If sending an activity to a channel where `inputHint` is used, bots SHOULD include the field, even when the value is `accepting`.

`A3042`: Receivers SHOULD interpret undefined values as `accepting`.

### Attachments

The `attachments` field contains a flat list of objects to be displayed as part of this activity. The value of each `attachments` list element is a complex object of the [Attachment](#attachment) type.

`A3050`: Senders SHOULD omit the `attachments` field if it contains no elements.

`A3051`: Senders MAY send multiple entities of the same type.

`A3052`: Receivers MAY treat attachments of unknown types as downloadable documents.

`A3053`: Receivers SHOULD preserve the ordering of attachments when processing content, except when rendering limitations force changes, e.g. grouping of documents after images.

### Attachment layout

The `attachmentLayout` field instructs user interface renderers how to present content included in the [`attachments`](#attachments) field. The value of the `attachmentLayout` field is of type string, with defined values of `list` and `carousel`. The default value is `list`.

`A3060`: If a sender includes the `attachmentLayout` field, it SHOULD only send defined values.

`A3061`: Receivers SHOULD interpret undefined values as `list`.

### Summary

The `summary` field contains text used to replace [`attachments`](#attachments) on channels that do not support them. The value of the `summary` field is of type string.

`A3070`: Receivers SHOULD consider the `summary` field to logically follow the `text` field.

`A3071`: Channels SHOULD NOT send the `summary` field to bots.

`A3072`: Channels able to process all attachments within an activity SHOULD ignore the `summary` field.

### Suggested actions

The `suggestedActions` field contains a payload of interactive actions that may be displayed to the user. Support for `suggestedActions` and their manifestation depends heavily on the channel. The value of the `suggestedActions` field is a complex object of the [Suggested actions](#suggested-actions-2) type.

### Value

The `value` field contains a programmatic payload specific to the activity being sent. Its meaning and format are defined in other sections of this document that describe its use.

`A3080`: Senders SHOULD NOT include `value` fields of primitive types (e.g. string, int). `value` fields SHOULD be complex types or omitted.

### Expiration

The `expiration` field contains a time at which the activity should be considered to be "expired" and should not be presented to the recipient. The value of the `expiration` field is an ISO 8601 [[3](#references)] encoded datetime within a string.

`A3090`: Senders SHOULD always use encode the value of `expiration` fields as UTC, and they SHOULD always include Z as an explicit UTC mark within the value.

### Importance

The `importance` field contains an enumerated set of values to signal to the recipient the relative importance of the activity. It is up to the receiver to map these importance hints to the user experience. The value of the `importance` field is of type string, with defined values of `low`, `normal` and `high`. The default value is `normal`.

`A3100`: If a sender includes the `importance` field, it SHOULD only send defined values.

`A3101`: Receivers SHOULD interpret undefined values as `normal`.

### Delivery mode

The `deliveryMode` field contains any one of an enumerated set of values to signal to the recipient alternate delivery paths for the activity or response. The value of the `deliveryMode` field is of type string, with defined values of `normal`, `notification` and `expectReplies`. The default value is `normal`.

Activities with a `deliveryMode` of `expectReplies` differ only in their requirement to return a response payload back to the caller synchronously, as a direct response to the initial request.

`A3110`: If a sender includes the `deliveryMode` field, it SHOULD only send defined values.

`A3111`: Receivers SHOULD interpret undefined values as `normal`.

`A3112`: Receivers SHOULD reject activities with `deliveryMode` of `expectReplies` if they do not support synchronous responses.

`A3113`: Receivers SHOULD NOT reply with asynchronous responses to activities with `deliveryMode` of `expectReplies`.

`A3114`: Senders MUST NOT include `deliveryMode` of `expectReplies` on Invoke activities unless the Invoke profile explicitly allows and describes its behavior.

`A3115`: Senders MUST establish whether a receiver understands `deliveryMode` of `expectReplies` prior to sending activities with that value.

`A3116`: Bots SHOULD NOT send activities with `deliveryMode` of `expectReplies` to channels.

### Listen for

The `listenFor` field contains a list of terms or references to term sources that speech and language processing systems can listen for. The value of the `listenFor` field is an array of strings whose format is defined in [Appendix IV](#appendix-iv---priming-format).

A missing `listenFor` field indicates default priming behavior should be used. The default is defined by the channel and may depend on variables such as the identity of the user and the bot.

`A3120`: Channels SHOULD NOT populate the `listenFor` field.

`A3121`: Bots SHOULD send `listenFor` contents that reflect the complete set of utterances expected from users, not just the utterances in response to the content in the message in which the `listenFor` is included.

### Semantic action

The `semanticAction` field contains an optional programmatic action accompanying the user request. The semantic action field is populated by the channel and bot based on some understanding of what the user is trying to accomplish; this understanding may be achieved with natural language processing, additional user interface elements tied specifically to these actions, through a process of conversational refinement, or contextually via other means. The meaning and structure of the semantic action is agreed ahead of time between the channel and the bot.

The value of the `semanticAction` field is a complex object of the [semantic action](#semantic-action-type) type.

`A3130`: Channels and bots MAY populate the `semanticAction` field. Other senders SHOULD NOT populate the `semanticAction` field.

Information within the semantic action field is meant to augment, not replace, existing content within the activity. A well-formed semantic action has a defined name, corresponding well-formed entities, and matches the user's intent in generating the activity.

`A3131`: Senders SHOULD NOT remove any content used to generate the `semanticAction` field.

`A3132`: Receivers MAY ignore parts or all of the `semanticAction` field.

`A3133`: Receivers MUST ignore `semanticAction` fields they cannot parse or do not understand.

Semantic actions are sometimes used to indicate a change in which participant controls the conversation. For example, a channel may use actions during an exchange with a skill. When so defined, skills can relinquish control through the [handoff activity](#handoff-activity) after the final `semanticAction` `state` is `done`.

`A3135`: Channels MAY define the use of [handoff activity](#handoff-activity) in conjunction with semantic actions.

`A3136`: Bots MAY use semantic action and [handoff activity](#handoff-activity) internally to coordinate conversational focus between components of the bot.

## Contact relation update activity

Contact relation update activities signal a change in the relationship between the recipient and a user within the channel. Contact relation update activities generally do not contain user-generated content. The relationship update described by a contact relation update activity exists between the user in the `from` field (often, but not always, the user initiating the update) and the user or bot in the `recipient` field.

Contact relation update activities are identified by a `type` value of `contactRelationUpdate`.

### Action

The `action` field describes the meaning of the contact relation update activity. The value of the `action` field is a string. Only values of `add` and `remove` are defined, which denote a relationship between the users/bots in the `from` and `recipient` fields.

## Conversation update activity

Conversation update activities describe a change in a conversation's members, description, existence, or otherwise. Conversation update activities generally do not contain user-generated content. The conversation being updated is described in the `conversation` field.

Conversation update activities are identified by a `type` value of `conversationUpdate`.

`A4100`: Senders MAY include zero or more of `membersAdded`, `membersRemoved`, `topicName`, and `historyDisclosed` fields in a conversation update activity.

`A4101`: Each `channelAccount` (identified by `id` field) SHOULD appear at most once within the `membersAdded` and `membersRemoved` fields. An ID SHOULD NOT appear in both fields. An ID SHOULD NOT be duplicated within either field.

`A4102`: Channels SHOULD NOT use conversation update activities to indicate changes to a channel account's fields (e.g., `name`) if the channel account was not added to or removed from the conversation.

`A4103`: Channels SHOULD NOT send the `topicName` or `historyDisclosed` fields if the activity is not signaling a change in value for either field.

### Members added

The `membersAdded` field contains a list of channel participants (bots or users) added to the conversation. The value of the `membersAdded` field is an array of type [`channelAccount`](#channel-account).

### Members removed

The `membersRemoved` field contains a list of channel participants (bots or users) removed from the conversation. The value of the `membersRemoved` field is an array of type [`channelAccount`](#channel-account).

### Topic name

The `topicName` field contains the text topic or description for the conversation. The value of the `topicName` field is of type string.

### History disclosed

The `historyDisclosed` field is deprecated.

`A4110`: Senders SHOULD NOT include the `historyDisclosed` field.

## End of conversation activity

End of conversation activities signal the end of a conversation from the recipient's perspective. This may be because the conversation has been completely ended, or because the recipient has been removed from the conversation in a way that is indistinguishable from it ending. The conversation being ended is described in the `conversation` field.

End of conversation activities are identified by a `type` value of `endOfConversation`.

Both the `code` and the `text` fields are optional.

### Code

The `code` field contains a programmatic value describing why or how the conversation was ended. The value of the `code` field is of type string and its meaning is defined by the channel sending the activity.

### Text

The `text` field contains optional text content to be communicated to a user. The value of the `text` field is of type string, and its format is plain text.

## Event activity

Event activities communicate programmatic information from a client or channel to a bot. The meaning of an event activity is defined by the `name` field, which is meaningful within the scope of a channel. Event activities are designed to carry both interactive information (such as button clicks) and non-interactive information (such as a notification of a client automatically updating an embedded speech model).

Event activities are the asynchronous counterpart to [invoke activities](#invoke-activity). Unlike invoke, event is designed to be extended by client application extensions.

Event activities are identified by a `type` value of `event` and specific values of the `name` field.

`A5000`: Channels MAY allow application-defined event messages between clients and bots, if the clients allow application customization.

### Name

The `name` field controls the meaning of the event and the schema of the `value` field. The value of the `name` field is of type string.

`A5001`: Event activities MUST contain a `name` field.

`A5002`: Receivers MUST ignore event activities with `name` fields they do not understand.

### Value

The `value` field contains parameters specific to this event, as defined by the event name. The value of the `value` field is a complex type.

`A5100`: The `value` field MAY be missing or empty, if defined by the event name.

`A5101`: Extensions to the event activity SHOULD NOT require receivers to use any information other than the activity `type` and `name` fields to understand the schema of the `value` field.

### Relates to

The `relatesTo` field references another conversation, and optionally a specific activity within that conversation. The value of the `relatesTo` field is a complex object of the [Conversation reference](#conversation-reference) type.

`A5200`: `relatesTo` SHOULD NOT reference an activity within the conversation identified by the `conversation` field.

## Invoke activity

Invoke activities communicate programmatic information from a client or channel to a bot, and have a corresponding return payload for use within the channel. The meaning of an invoke activity is defined by the `name` field, which is meaningful within the scope of a channel.

Invoke activities are the synchronous counterpart to [event activities](#event-activity). Event activities are designed to be extensible. Invoke activities differ only in their ability to return response payloads back to the channel; because the channel must decide where and how to process these response payloads, Invoke is useful only in cases where explicit support for each invoke name has been added to the channel. Thus, Invoke is not designed to be a generic application extensibility mechanism.

Invoke activities are identified by a `type` value of `invoke` and specific values of the `name` field.

The list of defined Invoke activities is included in [Appendix III](#appendix-iii---protocols-using-the-invoke-activity).

`A5301`: Channels SHOULD NOT allow application-defined invoke messages between clients and bots.

### Name

The `name` field controls the meaning of the invocation and the schema of the `value` field. The value of the `name` field is of type string.

`A5401`: Invoke activities MUST contain a `name` field.

`A5402`: Receivers MUST ignore event activities with `name` fields they do not understand.

### Value

The `value` field contains parameters specific to this event, as defined by the event name. The value of the `value` field is a complex type.

`A5500`: The `value` field MAY be missing or empty, if defined by the event name.

`A5501`: Extensions to the event activity SHOULD NOT require receivers to use any information other than the activity `type` and `name` fields to understand the schema of the `value` field.

### Relates to

The `relatesTo` field references another conversation, and optionally a specific activity within that conversation. The value of the `relatesTo` field is a complex object of the [Conversation reference](#conversation-reference) type.

`A5600`: `relatesTo` SHOULD NOT reference an activity within the conversation identified by the `conversation` field.

## Installation update activity

Installation update activities represent an installation or uninstallation of a bot within an organizational unit (such as a customer tenant or "team") of a channel. Installation update activities generally do not represent adding or removing a channel.

Installation update activities are identified by a `type` value of `installationUpdate`.

`A5700`: Channels MAY send installation activities when a bot is added to or removed from a tenant, team, or other organization unit within the channel.

`A5701`: Channels SHOULD NOT send installation activities when the bot is installed into or removed from a channel.

### Action

The `action` field describes the meaning of the installation update activity. The value of the `action` field is a string. Only values of `add` and `remove` are defined.

## Message delete activity

Message delete activities represent a deletion of an existing message activity within a conversation. The deleted activity is referred to by the `id` and `conversation` fields within the activity.

Message delete activities are identified by a `type` value of `messageDelete`.

`A5800`: Channels MAY elect to send message delete activities for all deletions within a conversation, a subset of deletions within a conversation (e.g. only deletions by certain users), or no activities within the conversation.

`A5801`: Channels SHOULD NOT send message delete activities for conversations or activities that the bot did not observe.

`A5802`: If a bot triggers a delete, the channel SHOULD NOT send a message delete activity back to that bot.

`A5803`: Channels SHOULD NOT send message delete activities corresponding to activities whose type is not `message`.

## Message update activity

Message update activities represent an update of an existing message activity within a conversation. The updated activity is referred to by the `id` and `conversation` fields within the activity, and the message update activity contains all fields in the revised message activity.

Message update activities are identified by a `type` value of `messageUpdate`.

`A5900`: Channels MAY elect to send messageUpdate  activities for all updates within a conversation, a subset of updates within a conversation (e.g. only updates by certain users), or no activities within the conversation.

`A5901`: If a bot triggers an update, the channel SHOULD NOT send a message update activity back to that bot.

`A5902`: Channels SHOULD NOT send message update activities corresponding to activities whose type is not `message`.

## Message reaction activity

Message reaction activities represent a social interaction on an existing message activity within a conversation. The original activity is referred to by the `id` and `conversation` fields within the activity. The `from` field represents the source of the reaction (i.e., the user that reacted to the message).

Message reaction activities are identified by a `type` value of `messageReaction`.

### Reactions added

The `reactionsAdded` field contains a list of reactions added to this activity. The value of the `reactionsAdded` field is an array of type [`messageReaction`](#message-reaction).

### Reactions removed

The `reactionsRemoved` field contains a list of reactions removed from this activity. The value of the `reactionsRemoved` field is an array of type [`messageReaction`](#message-reaction).

## Suggestion activity

Suggestion activities allow a bot to send content targeting a single user in the conversation which refers to a previous activity and suggests content that augments it. The suggested content is a superset of the [message activity](#message-activity), and fields present on the message activity are schematically valid on the suggestion activity. The channel in which the suggestion activity is sent defines limitations on text or attachment content, and these limitations may differ from the channel's limitations on message activities. The suggestion activity includes the `textHighlights` property so that the suggestions can be surfaced as annotations to the original content in the source activity.

For example: an message activity with the text "...we should meet on Monday to review this plan..." could cause a bot to send a suggestion activity which refers to that section of text and offers the user an affordance to create a meeting on their calendar. Some channels use this information to highlight and turn into a hot link the snippet of text to show the suggestion in context.

Suggestion activities are identified by a `type` value of `suggestion`. Suggestion activities refer to another activity by using the [`replyToId`](#reply-to-ID) property.

`A6100`: If there is no `replyToId` then the suggested content should be shown to the recipient as a normal message activity.

Suggestion activity uses the [`recipient`](#recipient) field to signal which user the suggestion is for.

All `textHighlight` objects are relative to the activity specified by the `replyToId` property.  There can be multiple `textHighlight` provided, allowing a client the option to turn different sections of the text into links which would show the suggestion content.

`A6101`: Bots MAY use `replyToId` and `textHighlights` to associate the suggestion with an activity.

`A6102`: Channels MAY use `replyToId` and `textHighlights` to show the suggestion activity contextually to the recipient.

`A6103`: Channels MUST only show the suggestion activity to the recipient. If unable to identify the intended recipient or show the content only to them, the channel MUST drop the activity.

`A6104`: Channels SHOULD NOT send suggestion activities to bots.

### Suggestion activity text highlights

The `textHighlights` field contains a list of text to highlight in the `text` field of the activity referred to by `replyToId`. The value of the `textHighlights` field is an array of type [`textHighlight`](#text-highlight).

## Trace activity

The Trace activity is an activity which the developer inserts in to the stream of activities to represent a point in the developers bot logic. The trace activity typically is logged by transcript history components to become part of a [Transcript-format](../transcript/transcript.md) history file.  In remote debugging scenarios the Trace activity can be sent to the client so that the activity can be inspected as part of the debug flow.

Trace activities are normally not shown to the user, and are internal to transcript logging and developer debugging.

Trace  activities are identified by a `type` value of `trace`.

`A6150`: channels SHOULD NOT display trace activities to the user, unless the user has identified itself as the developer in a secure manner.

### Name

The `name` field controls the name of the trace operation. The value of the `name` field is of type string.

`A6151`: Trace activities MAY contain a `name` field.

`A6152`: Receivers MUST ignore event activities with `name` fields they do not understand.

### Label

The `label` field contains optional a label which can provide contextual information about the trace. The value of the `label` field is of type string.

`A6153`: Trace activities MAY contain a `label` field.

### ValueType

The `valueType` field is a string type which contains a unique value which identifies the shape of the `value` object for this trace.

`A6154`: The `valueType` field MAY be missing or empty, if the `name` property is sufficient to understand the shape of the `value` property.

### Value

The `value` field contains an object for this trace, as defined by the `valueType` or `name` property if there is no `valueType`. The value of the `value` field is a complex type.

`A6155`: The `value` field MAY be missing or empty.

`A6156`: Extensions to the trace activity SHOULD NOT require receivers to use any information other than the activity `type` and `name` or `valueType` field to understand the schema of the `value` field.

### Relates to

The `relatesTo` field references another conversation, and optionally a specific activity within that conversation. The value of the `relatesTo` field is a complex object of the [Conversation reference](#conversation-reference) type.

`A6157`: `relatesTo` MAY reference an activity within the conversation identified by the `conversation` field.

## Typing activity

Typing activities represent ongoing input from a user or a bot. This activity is often sent when keystrokes are being entered by a user, although it's also used by bots to indicate they're "thinking," and could also be used to indicate e.g. collecting audio from users.

Typing activities are intended to persist within UIs for three seconds.

Typing activities are identified by a `type` value of `typing`.

`A6000`: If able, clients SHOULD display typing indicators for three seconds upon receiving a typing activity.

`A6001`: Unless otherwise known for the channel, senders SHOULD NOT send typing activities more frequently than one every three seconds. (Senders MAY send typing activities every two seconds to prevent gaps from appearing.)

`A6002`: If a channel assigns an [`id`](#id) to a typing activity, it MAY allow bots and clients to delete the typing activity before its expiration.

`A6003`: If able, channels SHOULD send typing activities to bots.

## Handoff activity

Handoff activities are used to request or signal a change in focus between elements inside a bot. They are not intended to be used in wire communication (besides internal communication that occurs between services in a distributed bot).

Handoff activities are identified by a `type` value of `handoff`.

`A6200`: Channels SHOULD drop handoff activities if they are not supported.

## Command activity

Command activities communicate a request to perform a specific action. They are identified by a `type` value of `command` and specific values of the `name` field.

Commands look similar in structure to events but have different semantics. Commands are requests to perform an action and receivers typically respond with one or more commandResult activities. Receivers are also expected to explicitly reject unsupported command activities.

`A6300`: Channels MAY allow application-defined command activities between clients and bots, if the clients allow application customization.

`A6301`: Application-defined command activities MUST be declared in the `application/*` namespace.

`A6302`: Command activities outside the `application/*` are considered reserved for Bot Framework Protocols.

The list of all Bot Framework protocols using command activities is included in [Appendix VI](#appendix-vi---protocols-using-the-command-activity).

### Name

The `name` field defines the meaning of the command activity. The value of the `name` field is of type string.

`A6310`: Command activities MUST contain a `name` field.

`A6311`: The `name` of a command activity MUST use a [MIME media type](https://www.iana.org/assignments/media-types/media-types.xhtml) [[9](#references)] format.

`A6312`: Receivers MUST ignore command activities with missing or invalid `name` field.

The recommended patterns for rejecting command activities are included in [Appendix VI](#appendix-vi---protocols-using-the-command-activity).

### Value

The `value` field contains the command metadata and parameters specific to a command, as defined by the command `name`. The `value` field is a complex object of the [command value](#command-value) type.

`A6321`: Command activities MUST contain a `value` field.

`A6322`: Receivers MUST ignore command activities with missing or invalid `value` field.

## Command result activity

Command result activities communicate the result of a [command activity](#command-activity).

Command result activities are identified by a `type` value of `commandResult` and specific values of the `name` field. The `name` field of a command result is always set to the `name` of the original command activity.

`A6400`: Senders MAY send one or more command result activities to communicate the result of the command.

### Name

The `name` field defines the meaning of the command result activity. The value of the `name` field is of type string.

`A6411`: Command result activities MUST contain a `name` field.

`A6412`: Receivers MUST ignore command activities with missing or invalid `name` field.

`A6413`: The `name` of a command result activity MUST be the same as the `name` of the original command activity.

### Value

The `value` field contains the command metadata and additional information specific to a command result, as defined by the command result `name`. The value of the `value` field is a complex object of type [command result value](#command-result-value) type.

`A6421`: Command result activities MUST contain a `value` field.

`A6422`: Receivers MUST reject command result activities with missing or invalid `value` field.

## Complex types

This section defines complex types used within the activity schema, described above.

### Attachment

Attachments are content included within a [message activity](#message-activity): cards, binary documents, and other interactive content. They're intended to be displayed in conjunction with text content. Content may be sent via URL data URI within the `contentUrl` field, or inline in the `content` field.

`A7100`: Senders SHOULD NOT include both `content` and `contentUrl` fields in a single attachment.

#### Content type

The `contentType` field describes the [MIME media type](https://www.iana.org/assignments/media-types/media-types.xhtml) [[9](#references)] of the content of the attachment. The value of the `contentType` field is of type string.

#### Content

When present, the `content` field contains a structured JSON object attachment. The value of the `content` field is a complex type defined by the `contentType` field.

`A7110`: Senders SHOULD NOT include JSON primitives in the `content` field of an attachment.

#### Content URL

When present, the `contentUrl` field contains a URL to the content in the attachment. Data URIs, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)] are typically supported by channels. The value of the `contentUrl` field is of type string.

`A7120`: Receivers SHOULD accept HTTPS URLs.

`A7121`: Receivers MAY accept HTTP URLs.

`A7122`: Channels SHOULD accept data URIs.

`A7123`: Channels SHOULD NOT send data URIs to clients or bots.

#### Name

The `name` field contains an optional name or filename for the attachment. The value of the `name` field is of type string.

#### Thumbnail URL

Some clients have the ability to display custom thumbnails for non-interactive attachments or as placeholders for interactive attachments. The `thumbnailUrl` field identifies the source for this thumbnail. Data URIs, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)] are typically also allowed.

`A7140`: Receivers SHOULD accept HTTPS URLs.

`A7141`: Receivers MAY accept HTTP URLs.

`A7142`: Channels SHOULD accept data URIs.

`A7143`: Channels SHOULD NOT send `thumbnailUrl` fields to bots.

### Card action

A card action represents a clickable or interactive button for use within cards or as [suggested actions](#suggested-actions). They are used to solicit input from users. Despite their name, card actions are not limited to use solely on cards.

Card actions are meaningful only when sent to channels.

Channels decide how each action manifests in their user experience. In most cases, the cards are clickable. In others, they may be selected by speech input. In cases where the channel does not offer an interactive activation experience (e.g., when interacting over SMS), the channel may not support activation whatsoever. The decision about how to render actions is controlled by normative requirements elsewhere in this document (e.g. within the card format, or within the [suggested actions](#suggested-actions) definition).

#### Type

The `type` field describes the meaning of the button and behavior when the button is activated. Values of the `type` field are by convention short strings (e.g. "`openUrl`"). See subsequent sections for requirements specific to each action type.

* An action of type `messageBack` represents a text response to be sent via the chat system.
* An action of type `imBack` represents a text response that is added to the chat feed.
* An action of type `postBack` represents a text response that is not added to the chat feed.
* An action of type `openUrl` represents a hyperlink to be handled by the client.
* An action of type `downloadFile` represents a hyperlink to be downloaded.
* An action of type `showImage` represents an image that may be displayed.
* An action of type `signin` represents a hyperlink to be handled by the client's signin system.
* An action of type `playAudio` represents audio media that may be played.
* An action of type `playVideo` represents video media that may be played.
* An action of type `call` represents a telephone number that may be called.

#### Title

The `title` field includes text to be displayed on the button's face. The value of the `title` field is of type string, and does not contain markup.

This field applies to actions of all types.

`A7210`: Channels SHOULD NOT process markup within the `title` field (e.g. Markdown).

#### Image

The `image` field contains a URL referencing an image to be displayed on the button's face. Data URIs, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)] are typically supported by channels. The value of the `image` field is of type string.

This field applies to actions of all types.

`A7220`: Channels SHOULD accept HTTPS URLs.

`A7221`: Channels MAY accept HTTP URLs.

`A7222`: Channels SHOULD accept data URIs.

#### Image alt text

The `imageAltText` field contains alternate image text to be used in place of the `image` field. This field should only be used when the `text` field is inadequate as alt text. The value of the `imageAltText` field is of type string.

`A7225`: Senders SHOULD NOT populate the `imageAltText` field if its value is identical to the `text` field.

`A7226`: Receivers SHOULD use the value of the `imageAltText` field if it is present in the enclosing [card action](#card-action). If the `imageAltText` field is missing, receivers SHOULD use the `text` field as the alt text.

#### Text

The `text` field contains text content to be sent to a bot and included in the chat feed when the button is clicked. The contents of the `text` field may or may not be displayed, depending on the button type. The `text` field may contain markup, controlled by the [`textFormat`](#text-format) field in the activity root. The value of the `text` field is of type string.

This field is only used on actions of select types. Details on each type of action are included later in this document.

`A7230`: The `text` field MAY contain an empty string to indicate sending text without contents.

`A7231`: Channels SHOULD process the contents of the `text` field in accordance with the [`textFormat`](#text-format) field in the activity root.

#### Display text

The `displayText` field contains text content to be included in the chat feed when the button is clicked. The contents of the `displayText` field SHOULD always be displayed, when technically possible within the channel. The `displayText` field may contain markup, controlled by the [`textFormat`](#text-format) field in the activity root. The value of the `displayText` field is of type string.

This field is only used on actions of select types. Details on each type of action are included later in this document.

`A7240`: The `displayText` field MAY contain an empty string to indicate sending text without contents.

`A7241`: Channels SHOULD process the contents of the `displayText` field in accordance with the [`textFormat`](#text-format) field in the activity root.

#### Value

The `value` field contains programmatic content to be sent to a bot when the button is clicked. The contents of the `value` field are of any primitive or complex type, although certain activity types constrain this field.

This field is only used on actions of select types. Details on each type of action are included later in this document.

#### Message Back

A `messageBack` action represents a text response to be sent via the chat system. Message Back uses the following fields:
* `type` ("`messageBack`")
* `title`
* `image`
* `text`
* `displayText`
* `value` (of any type)

`A7350`: Senders SHOULD NOT include `value` fields of primitive types (e.g. string, int). `value` fields SHOULD be complex types or omitted.

`A7351`: Channels MAY reject or drop `value` fields not of complex type.

`A7352`: When activated, channels MUST send an activity of type `message` to all relevant recipients.

`A7353`: If the channel supports storing and transmitting text, the contents of the `text` field of the action MUST be preserved and transmitted in the `text` field of the generated message activity.

`A7355`: If the channel supports storing and transmitting additional programmatic values, the contents of the `value` field MUST be preserved and transmitted in the `value` field of the generated message activity.

`A7356`: If the channel supports preserving a different value in the chat feed than is sent to bots, it MUST include the `displayText` field in the chat history.

`A7354`: If the channel does not support `A7353` but does support recording text within the chat feed, it MUST include the `text` field in the chat history.

`A7357`: If the channel supports presenting an image on the action, the contents of the `image` field of the action MUST be shown on or in connection with the action.

`A7358`: The contents of the `title` field of the action MUST be shown on or in connection with the action.

`A7359`: Senders SHOULD include an `image` field, a `title` field, or both. Channels MAY reject or drop `messageBack` actions which have neither an `image` field nor a `title` field.

#### IM Back

An `imBack` action represents a text response that is added to the chat feed. IM Back uses the following fields:
* `type` ("`imBack`")
* `title`
* `image`
* `value` (of type string)

`A7360`: When activated, channels MUST send an activity of the type `message` to all relevant recipients.

`A7361`: If the channel supports storing and transmitting text, the contents of the `title` field MUST be preserved and transmitted in the `text` field of the generated message activity.

`A7362`: If the `title` field on an action is missing and the `value` field is of type string, the channel MAY transmit the contents of the `value` field in the `text` field of the generated message activity.

`A7363`: If the channel supports recording text within the chat feed, it MUST include the contents of the `title` field in the chat history.

#### Post Back

A `postBack` action represents a text response that is not added to the chat feed. Post Back uses the following fields:
* `type` ("`postBack`")
* `title`
* `image`
* `value` (of type string)

`A7370`: When activated, channels MUST send an activity of the type `message` to all relevant recipients.

`A7371`: Channels SHOULD NOT include text within the chat history when a Post Back action is activated.

`A7372`: Channels MUST reject or drop `value` fields not of string type.

`A7373`: If the channel supports storing and transmitting text, the contents of the `value` field MUST be preserved and transmitted in the `text` field of the generated message activity.

`A7374`: If the channel is unable to support transmitting to the bot without including history in the chat feed, it SHOULD use the `title` field as the display text.

#### Open URL actions

An `openUrl` action represents a hyperlink to be handled by the client. Open URL uses the following fields:
* `type` ("`openUrl`")
* `title`
* `image`
* `value` (of type string)

`A7380`: Senders MUST include a URL in the `value` field of an `openUrl` action.

`A7381`: Receivers MAY reject `openUrl` action whose `value` field is missing or not a string.

`A7382`: Receivers SHOULD reject or drop `openUrl` actions whose `value` field contains a data URI, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)].

`A7383`: Receivers SHOULD NOT reject `openUrl` actions whose `value` URI is of an otherwise unexpected URI scheme or value.

`A7384`: Clients with knowledge of particular URI schemes (e.g. HTTP) MAY handle `openUrl` actions within an embedded renderer (e.g., a browser control).

`A7385`: When available, clients SHOULD delegate handling of `openUrl` actions not handled by `A7354` to the operating-system- or shell-level URI handler.

#### Download File actions

An `downloadFile` action represents a hyperlink to be downloaded. Download File uses the following fields:
* `type` ("`downloadFile`")
* `title`
* `image`
* `value` (of type string)

`A7390`: Senders MUST include a URL in the `value` field of an `downloadFile` action.

`A7391`: Receivers MAY reject `downloadFile` action whose `value` field is missing or not a string.

`A7392`: Receivers SHOULD reject or drop `downloadFile` actions whose `value` field contains a data URI, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)].

#### Show Image File actions

An `showImage` action represents an image that may be displayed. Show Image uses the following fields:
* `type` ("`showImage`")
* `title`
* `image`
* `value` (of type string)

`A7400`: Senders MUST include a URL in the `value` field of an `showImage` action.

`A7401`: Receivers MAY reject `showImage` action whose `value` field is missing or not a string.

`A7402`: Receivers MAY reject `showImage` actions whole `value` field is a Data URI, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)].

#### Signin

A `signin` action represents a hyperlink to be handled by the client's signin system. Signin uses the following fields:
* `type` ("`signin`")
* `title`
* `image`
* `value` (of type string)

`A7410`: Senders MUST include a URL in the `value` field of an `signin` action.

`A7411`: Receivers MAY reject `signin` action whose `value` field is missing or not a string.

`A7412`: Receivers MUST reject or drop `signin` actions whose `value` field contains a data URI, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)].

#### Play Audio

A `playAudio` action represents audio media that may be played. Play Audio uses the following fields:
* `type` ("`playAudio`")
* `title`
* `image`
* `value` (of type string)

`A7420`: When activated, channels MAY play the audio referenced by URI in the `value` field.

`A7421`: Channels MUST reject or drop `value` fields not of string type.

`A7422`: Senders SHOULD NOT send data URIs, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)], without prior knowledge that the channel supports them.

#### Play video

A `playVideo` action represents video media that may be played. Play Video uses the following fields:
* `type` ("`playVideo`")
* `title`
* `image`
* `value` (of type string)

`A7430`: When activated, channels MAY play the video referenced by URI in the `value` field.

`A7431`: Channels MUST reject or drop `value` fields not of string type.

`A7432`: Senders SHOULD NOT send data URIs, as defined in [RFC 2397](https://tools.ietf.org/html/rfc2397) [[10](#references)], without prior knowledge that the channel supports them.

#### Call

A `call` action represents a telephone number that may be called. Call uses the following fields:
* `type` ("`call`")
* `title`
* `image`
* `value` (of type string)

`A7440`: Senders MUST include a URL of scheme `tel` in the `value` field of an `signin` action.

`A7441`: Receivers MUST reject `signin` action whose `value` field is missing or not a string URI of the `tel` scheme.

### Channel account

Channel accounts represent identities within a channel. The channel account includes an ID that can be used to identify and contact the account within that channel. Sometimes these IDs exist within a single namespace (e.g. Skype IDs); sometimes, they are federated across many servers (e.g. email addresses). In addition to the ID, channel accounts include display names and Azure Active Directory (AAD) object IDs.

#### Channel account ID

The `id` field is the identifier and address within the channel. The value of the `id` field is a string. An example `id` within a channel that uses email addresses is "name@example.com"

`A7510`: Channels SHOULD use the same values and conventions for account IDs regardless of their position within the schema (`from.id`, `recipient.id`, `membersAdded`, etc.). This allows bots and clients to use ordinal string comparisons to know when e.g. they are described in the `membersAdded` field of a `conversationUpdate` activity.

#### Channel account name

The `name` field is an optional, friendly name within the channel. The value of the `name` field is a string. An example `name` within a channel is "John Doe"

#### Channel account AAD Object ID

The `aadObjectId` field is an optional ID corresponding to the account's object ID within Azure Active Directory (AAD). The value of the `aadObjectId` field is a string.

#### Channel account role

The `role` field indicates whether entity behind the account is a user or bot. This field is intended for use in the [Transcript format](../transcript/transcript.md) [[16](#references)] to distinguish between activities sent by users and activities sent by bots. The value of the `role` field is a string.

`A7511`: Senders SHOULD NOT include this field. Receivers SHOULD ignore this field.

### Conversation account

Conversation accounts represent the identity of conversations within a channel. In channels that support only a single conversation between two accounts (e.g. SMS), the conversation account is persistent and does not have a predetermined start or end. In channels that support multiple parallel conversations (e.g. email), each conversation will likely have a unique ID.

#### Conversation account ID

The `id` field is the identifier within the channel. The format of this ID is defined by the channel and is used as an opaque string throughout the protocol.

Channels SHOULD choose `id` values that are stable for all participants within a conversation. (For example, a poor example for the `id` field for a 1:1 conversation is to use the other participant's ID as the `id` value. This would result in a different `id` from each participant's perspective. A better choice is to sort the IDs of both participants and concatenate them together, which would be the same for both parties.)

#### Conversation account name

The `name` field is an optional, friendly name for the conversation within the channel. The value of the `name` field is a string.

#### Conversation account AAD Object ID

The `aadObjectId` field is an optional ID corresponding to the conversation's object ID within Azure Active Directory (AAD). The value of the `aadObjectId` field is a string.

#### Conversation account Is Group

The `isGroup` field indicates whether the conversation contains more than two participants at the time the activity was generated. The value of the `isGroup` field is a boolean; if omitted, the default value is `false`. This field typically controls the at-mention behavior for participants in the channel, and SHOULD be set to `true` if and only if more than two participants have the ability to both send and receive activities within the conversation.

#### Conversation account Conversation Type

If the channel distinguishes between types of conversations (e.g. group vs. personal), the `conversationType` field indicates the type of the conversation. This field augments the lower-fidelity [`isGroup`](#conversation-account-is-group) field. The value of the `conversationType` field is a string and its meaning is defined by the channel in which the type occurs.

#### Conversation account role

The `role` field indicates whether entity behind the account is a user or bot. This field is intended for use in the [Transcript format](../transcript/transcript.md) [[16](#references)] to distinguish between activities sent by users and activities sent by bots. The value of the `role` field is a string.

`A7512`:Senders SHOULD NOT include this field. Receivers SHOULD ignore this field.

#### Conversation account tenant ID

The `tenantId` field is an optional ID corresponding to the conversation's tenant ID within the channel. The value of the `tenantId` field is a string.

### Conversation reference

The `conversationReference` type contains a reference into another conversation. In its most minimal form, this reference may only contain the IDs of the target conversation. Implementers may wish to carry additional information in the `conversationReference`, such as the identity and roles of participants, and the ID of a specific [`activity`](#activity) within the conversation. Consumers of the `conversationReference` type are not provided any de facto guarantees about the validity or consistency of the IDs within the object; this is instead conferred by the sender who created the object.

The `conversationReference` type is frequently used to store a reference to a conversation so it can be later retrieved and used to continue a conversation.

`A7550`: Senders MUST include the `channelId` and the `conversation.id` properties within a conversation reference, even if they contain the same values as the parent Activity.

#### Conversation reference channel ID

The `channelId` field contains the [channel ID](#channel-id) of the conversation referenced by the conversationReference object. The value of the `channelId` field is of type string.

#### Conversation reference activity ID

The `activityId` field contains an optional [ID](#id) of an activity to refer to within the conversation. The value of the `activityId` field is of type string.

#### Conversation reference conversation

The `conversation` field contains the [Conversation account](#conversation-account) representing the conversation's identity. The value of the `conversation` field is a complex object of type [Conversation account](#conversation-account).

#### Conversation reference user

The `user` field contains an optional reference to the user's identity within the conversation. The value of the `user` field is of type [Channel account](#channel-account).

#### Conversation reference bot

The `bot` field contains an optional reference to the bot's identity within the conversation. The value of the `bot` field is of type [Channel account](#channel-account).

#### Conversation reference serviceUrl

The `serviceUrl` field contains an optional copy of the [`serviceUrl`](#service-url) that applies to the referenced conversation. The value of the `serviceUrl` field is of type string.

`A7560`: Receivers SHOULD ensure the `serviceUrl` property in conversation references is accompanied by either a corresponding security endorsement (in the case of transmitted Conversation references), or a flag indicating the `serviceUrl` and its security endorsement was verified before storage (in the case of a stored Conversation reference).

#### Conversation reference locale

The `locale` field contains an optional copy of the [`locale`](#locale) that applies to the referenced conversation. The value of the `locale` field is an [IETF BCP-47](https://tools.ietf.org/html/bcp47) [[19](#references)] language tag within a string.

### Entity

Entities carry metadata about an activity or conversation. Each entity's meaning and shape is defined by the `type` field. Additional type-specific fields sit as peers to the `type` field.

Some non-Bot-Framework entities may have a preexisting field called `type`. Parties integrating these entities into the activity entity format are advised to define field-level mapping to resolve conflicts with the `type` field name and other incompatibilities with serialization requirement `A2001` as part of the IRI defining the entity type.

Frequently, entities used within Bot Framework are also expressed elsewhere using [JSON-LD](https://www.w3.org/TR/json-ld/) [[18](#references)]. The entity format is designed to be compatible with JSON-LD contexts, but does not require senders or receivers to implement JSON-LD to successfully process an entity.

`A7603`: Senders MAY include [JSON-LD](https://www.w3.org/TR/json-ld/) [[18](#references)] fields within entities.

`A7604`: Senders MUST NOT expect or require [JSON-LD](https://www.w3.org/TR/json-ld/) [[18](#references)] processing to parse an entity.

#### Entity type

The `type` field is required, and defines the meaning and shape of the entity. `type` is intended to contain [IRIs](https://tools.ietf.org/html/rfc3987) [[4](#references)] although there are a small number on non-IRI entity types defined in [Appendix II](#appendix-ii---non-iri-entity-types). The value of the `type` field is a string.

`A7610`: Senders SHOULD use non-IRI types names only for types described in [Appendix II](#appendix-ii---non-iri-entity-types).

`A7611`: Senders MAY send IRI types for types described in [Appendix II](#appendix-ii---non-iri-entity-types) if they have knowledge that the receiver understands them.

`A7612`: Senders SHOULD use or establish IRIs for entity types not defined in [Appendix II](#appendix-ii---non-iri-entity-types).

`A7613`: Senders MUST NOT use relative IRIs within the `type` field, nor require JSON-LD IRI resolution to understand a type identifier.

### Suggested actions

Suggested actions may be sent within message content to create interactive action elements within a client UI.

`A7700`: Clients that do not support UI capable of rendering suggested actions SHOULD ignore the `suggestedActions` field.

`A7701`: Senders SHOULD omit the `suggestedActions` field if the `actions` field is empty.

#### To

The `to` field contains channel account IDs to whom the suggested actions should be displayed. This field may be used to filter actions to a subset of participants within the conversation.

`A7710`: If the `to` field is missing or empty, the client SHOULD display the suggested actions to all conversation participants.

`A7711`: If the `to` field contains invalid IDs, those values SHOULD be ignored.

#### Actions

The `actions` field contains a flat list of actions to be displayed. The value of each `actions` list element is a complex object of type `cardAction`.

### Message reaction

Message reactions represent a social interaction ("like", "+1", etc.). Message reactions currently only carry a single field: the `type` field.

#### Type

The `type` field describes the type of social interaction. The value of the `type` field is a string, and its meaning is defined by the channel in which the interaction occurs. Some common values such as `like` and `+1` although these are uniform by convention and not by rule.

### Text highlight

A text highlight refers to a substring of content within another field. This type is used within [suggestion activities](#suggestion-activity) to annotate text within another activity.

`A7720`: Receivers MUST ignore a text highlight if the `text` field is missing or empty, if it contains with a `occurrence` value less than 0, or if the `occurrence` field greater than the number of occurrences of the `text` field within the referenced text.

#### Text

The `text` field is required, and defines the snippet of text to highlight. The contents of the `text` field MUST be ordinally identical to the content of the referenced text. The value of the `text` field is of type string.

`A7721`: Senders MUST NOT send missing or empty strings for `text`. Receivers MUST ignore text higlights with missing or empty `text` fields.

#### Occurrence

The `occurrence` field is optional. It gives the sender the ability to specify which occurrence of the `text` to highlight. If it is not specified or is 0 then clients should highlight the first occurrence. The value of the `occurrence` field is of type integer.

`A7722`: Senders SHOULD NOT include the `occurrence` field if its value is `0` or `1`.

### Semantic action type

The semantic action type represents a programmatic reference. It is used within the [`semanticAction`](#semantic-action) field in [message activities](#message-activity). Actions are defined and registered externally to this protocol, typically as part of the [Bot Framework Manifest](../manifest/botframework-manifest.md) [[14](#references)]. The action definition declares the ID for the action and associates it with named entities, each of which has a corresponding type. Senders are receivers of actions use these names and types to create and parse actions that conform to the action definition.

Actions proceed through a lifecycle, described by the [`state`](#semantic-action-state) and [`id`](#semantic-action-id) fields.

At the beginning of the lifecycle, the channel indicates the `id` of the action it wishes to invoke, and sends a `state` of `start`.

The bot then sends and receives subsequent activities. During this time, both the bot and the channel may send additional metadata in the form of actions with `state` of `continue`. If the bot detects the user changing topic, it reflects this in a revised value of `id`.

When complete, the bot may send `state` of `done`. Just like earlier steps, the bot can update the `id` field to reflect what was actually in the conversation.

Example data flow for `semanticAction`. (Note that [entities](#semantic-action-entities) are abbreviated).
```
User: Book a flight from SeaTac to NYC
    Semantic action: state="start", id="bookFlight", entities="SeaTac", "NYC"

Bot: Would you like to arrive at LaGuardia Airport or JFK International Airport?
    Semantic action: state="continue", id="bookFlight", entities="KLGA", "KJFK"

User: Hold on a second
    Semantic action empty

Bot: No problem, please reply when you'd like to continue.
    Semantic action empty

User: LaGuardia
    Semantic action: state="continue", id="bookFlight", entities="KLGA"

Bot: OK, would you like to book your flight?
    Semantic action: state="continue", id="bookFlight"

User: Yes
    Semantic action empty

Bot: OK, your flight has been booked for November 10 at 8:05am.
    Semantic action: state="done", id="flightBooked", entities="Flight 81, KSEA to KLGA"
```

#### Semantic action state

The `state` field describes whether the action is beginning, continuing, or ending. The value of the `state` field is of type string with defined values of `start`, `continue`, and `done`. This field is not extensible.

**Note: the `state` field is not a mechanism to control a bot's state machine. Instead, it may be used to *request* that an action be started, or it may be used by bots to *inform* that the current action has changed or completed.**

Because `semanticAction` is optional and the topic of a conversation may drift over time, receivers are cautioned to be flexible in accepting `semanticAction` contents, especially with states of `continue` and `done`. Specifically, channels and bots are cautioned not to create a state machine requiring specific values of `semanticAction` fields `state` and `id`. Instead, treat `semanticAction` as additional metadata that can be used to optionally enrich data sharing between actors. Receivers may always discard unexpected `semanticAction` values in accordance with `A3133`.

The `start` state indicates an action is being started. Subsequent `start` actions indicate the sender wishes to start this action or another action (depending on the value of the `id` field).

`A7760`: Channels MUST only send `state` of `start` on the first message of an exchange invoking an action. Others senders MUST NOT send `state` of `start`.

The `continue` state indicates processing of an action is ongoing, and this activity contains new information within the `semanticAction` field.

`A7761`: Channels and bots MAY send `state` of `continue` when they populate the `semanticAction` field. Other senders MUST NOT send `state` of `continue`.

The `done` state indicates an action was successfully completed.

`A7762`: Bots SHOULD send `state` of `done` when an action has been completed, even if `semanticAction` contains no entities. Other senders MUST NOT send `state` of `done`.

Channels are expected to provide continuity when issuing actions to bots, but should expect changes in the `state` or `id` values received from bots.

`A7763`: Channels SHOULD NOT send `state` of `continue` without a preceding `state` of `start`.

`A7764`: Receivers MUST NOT require senders to include `semanticAction` fields with `state` of `continue` or `done`, nor a specific `id` value during a conversation.

#### Semantic action ID

The `id` field establishes the identity for the action, and is associated with a definition for the meaning and structure of the action (typically communicated via a registration system). The value of the `id` field is of type string.

`A7730`: Senders MUST NOT generate semantic actions with missing or empty `id` fields.

`A7731`: Two `id` values are equivalent only if they are ordinally identical.

`A7732`: Channels SHOULD only change the value of the `id` field within a conversation when a new action with `state` of `start` is sent, or when the bot sends a `state` of `continue` with a new `id` value.

`A7733`: Bots SHOULD make best-effort attempts to update the `id` field to reflect the topic of the conversation. Channels SHOULD make best-effort attempts to honor the bot's stated `id` values.

#### Semantic action entities

The `entities` field contains entities associated with this action. The value of the `entities` field is a complex object; the keys of this object are entity names and the values of each key is the corresponding entity values of type [entity](#entity). The meaning of each entity is defined by the enclosing action and the entity name. An additional field named `$instance` occurs after the named entities. The value of the `$instance` field is of type [`entityInstance`](#entity-instance).

`A7740`: Unless otherwise specified, senders MAY omit some or all entities associated with an action definition.

`A7742`: Senders MAY add entities with unknown keys if they have special knowledge that the bot supports them.

Actions support dynamic typing. An implementer of an action expresses a list of types it prefers, and callers of that action can match the desired types with known entities of varying fidelity. For example, assume an action prefers to receive a destination in the form of a *city* entity. The caller may not have a city available, but is able to supply either a *string* or *geocoordinates* based on what it was able to extract from the conversation. The caller can examine the action's preferred types and send the *string* or *geocoordinates* if the action can handle it.

`A7744`: Senders MAY send downgraded entities in accordance with the rules outlined in the action definition.

Entities sent within the semantic action have a specific meaning, defined by their name. For example, an action may be named `findRoute` with entities named `source` and `destination`. Sometimes, additional entities are available that do not fit a specific meaning within the action. The root [`entities`](#entities) array is a suitable location to transmit these entities.

`A7745`: Senders MAY send entities not listed in the action definition in the [`entities`](#entities) array in the activity root. Senders SHOULD NOT send these entities in the semantic action.

The `$instance` field carries metadata about the source of each entity. The keys of this object are identical to the entity names as peers. The values of each key is the corresponding instance metadata of type [`semanticEntityInstance`](#semantic-entity-instance).

`A7746`: Senders SHOULD include properties within `$instance` for any entities bearing instance metadata.

`A7747`: Senders SHOULD NOT include empty fields within `$instance` or an empty `$instance` object.

`A7748`: Senders MUST NOT include a `$instance` field within the `$instance` object.

Example of semantic action entities
```
"entities": {
    "sourceAirport": {
        "type": "http://icao.org/airport/code"
        "code": "KSEA",
        "name": "Seattle-Tacoma International Airport",
    },
    "destinationAirport": {
        "type": "http://schema.org/City",
        "name": "New York",
        "geo": {
            "type": "http://schema.org/GeoCoordinates",
            "latitude": 40.7127,
            "longitude": -74.0059
        }
    },
    "$instance": {
        "sourceAirport": {
            "text": "SeaTac",
            "startIndex": 19,
            "endIndex": 25
        },
        "destinationAirport": {
            "text": "New York",
            "startIndex": 29,
            "endIndex": 37
        }
    }
}
```

### Semantic entity instance

The `semanticEntityInstance` type references to source information about where the entity was mentioned. This specification includes source data to refer to the `text` field although others may be added in the future. The value of the `$instance` field is a complex object with fields `text`, `startIndex`, and `endIndex`. The `text` field is a string containing a copy of the text within the [`text`](#text) field in the activity root; `startIndex` is a number containing the index of the first character where `text` is found (inclusive); `endIndex` is a number containing the index after the last character where `text` is found (exclusive).

`A7750`: Senders MUST NOT include the `$instance` field if its `text` field is empty or null or the contents of its `text` field cannot be found within the `text` field in the activity root.

`A7751`: The `startIndex` field MUST be an integer greater than or equal to zero and less than the length of the `text` field in the activity root.

`A7752`: The `endIndex` field MUST be an integer greater than zero and less than or equal to the length of the `text` field in the activity root. Its value MUST be greater than the `startIndex` value.

`A7753`: The contents of the `text` field within `$instance` MUST contain characters ordinally identical to the value of the `text` field in the activity root starting at `startIndex` characters from the beginning and ending immediately before `endIndex` characters from the beginning.

### Command value
The `value` field of a [command activity](#command-activity) contains metadata related to a command. An optional extensible `data` payload may be included if defined by the command activity `name`.

#### Command Id

`A10100`: A command value MAY include the `commandId` field with the unique ID that the sender assigns to this command.

#### Data

The `data` field contains optional parameters specific to this command activity, as defined by the `name`. The value of the `data` field is a complex type.

`A10200`: The `data` field MAY be missing or empty, if defined by the command activity `name`.

`A10201`: Extensions to the command activity SHOULD NOT require receivers to use any information other than the activity `type` and `name` fields to understand the schema of the `data` field.

### Command result Value
The `value` field of a [command result activity](#command-result-activity) contains metadata related to a command. An optional extensible `data` payload may be included if defined by the command result activity `name`. The presence of an `error` field indicates that the original command failed to complete.

#### Command Id

`A11100`: If the original command includes a valid `commandId`, then the command result value MUST include the same value in `commandId` to allow the result to be correlated to the original command instance.

#### Data

The `data` field contains optional additional information specific to this command result activity, as defined by the `name`. The value of the `data` field is a complex type.

`A11200`: The `data` field MAY be missing or empty, if defined by the command result activity name.

`A11201`: Extensions to the command result activity SHOULD NOT require receivers to use any information other than the activity `type` and `name` fields to understand the schema of the `data` field.

#### Error

The `error` field contains the reason the original [command activity](#command-activity) failed to complete.

`A11300`: Senders MUST include the `error` field when the command was not successful. The value of the `error` field is of type  [Error Object](../botframework-protocol/botframework-protocol.md#error-object)

`A11301`: Senders MUST NOT include the `error` field when the command was successful.

## References

1. [Bot Framework Protocol](../botframework-protocol/botframework-channel.json) -- *Bot Framework Protocol swagger definition*
2. [RFC 2119](https://tools.ietf.org/html/rfc2119) -- *Key words for use in RFCs to Indicate Requirement Levels*
3. [ISO 8601](https://www.iso.org/iso-8601-date-and-time-format.html) -- *Date and time format*
4. [RFC 3987](https://tools.ietf.org/html/rfc3987) -- *Internationalized Resource Identifiers (IRIs)*
5. [Markdown](https://daringfireball.net/projects/markdown/)
6. [ISO 639](https://www.iso.org/iso-639-language-codes.html) -- *Language codes*
7. [SSML](https://www.w3.org/TR/speech-synthesis/) -- *Speech Synthesis Markup Language*
8. [XML](https://www.w3.org/TR/xml/) -- *Extensible Markup Language*
9. [MIME media types](https://www.iana.org/assignments/media-types/media-types.xhtml)
10. [RFC 2397](https://tools.ietf.org/html/rfc2397) -- *The "data" URL scheme*
11. [ISO 3166-1](https://www.iso.org/iso-3166-country-codes.html) -- *Country codes*
12. [Bot Framework Cards](botframework-cards.md)
13. [Adaptive Cards](https://adaptivecards.io)
14. [Bot Framework Manifest](../manifest/botframework-manifest.md)
15. [RFC 4627](http://www.ietf.org/rfc/rfc4627.txt) -- *The application/json Media Type for JavaScript Object Notation (JSON)*
16. [Transcript](../transcript/transcript.md)
17. [RFC 6557](https://tools.ietf.org/html/rfc6557)
18. [JSON-LD](https://www.w3.org/TR/json-ld/) -- *A JSON-based serialization for Linked Data*
19. [IETF BCP-47](https://tools.ietf.org/html/bcp47) -- *Language tag*

# Appendix I - Changes

## 2023-09-22 - hawo@microsoft.com
* Added [Schema.org Claim](#schema-org-claim) type definition

## 2020-07-07 - contact.me@nlchar.rocks
* Fix the broken link to the Bot Framework Manifest Spec in the References section

## 2020-06-19 - dandris@microsoft.com
* Fix `A7420` and `A7430` text to reflect original intent of `playAudio` and `playVideo` actions
* Add [conversation reference](#conversation-reference) type definition

## 2020-06-16 - dandris@microsoft.com
* Integrate typo and casing fixes
* Fix internal anchor links and relative file links

## 2020-03-02 - dandris@microsoft.com
* Added [`imageAltText`](#image-alt-text)

## 2019-07-15 - dandris@microsoft.com
* Added `A3035` to clarify behavior in the absence of a `speak` field.

## 2019-05-09 - toddne@microsoft.com
* Removed A3032.  Speak takes either plain text or valid SSML

## 2019-01-31 - toddne@microsoft.com
* Revised specification of [locale] to reference BCP-47.

## 2019-05-02 - dandris@microsoft.com
* Added [`callerId`](#caller-Id) and [Appendix V](#appendix-v---caller-id-values)

## 2019-01-31 - toddne@microsoft.com
* Revised specification of [locale] to reference BCP-47.

## 2019-01-31 - dandris@microsoft.com
* Add `tenantId` to [conversation account](#conversation-account) type

## 2019-01-11 - dandris@microsoft.com
* Add `A7357`, `A7358`, and `A7359` to profile `image` and `title` fields of the `messageBack` action

## 2019-01-04 - dandris@microsoft.com
* Resolved collision in [Trace activity](#trace-activity) numbering: `A610x` -> `A615x`

## 2018-12-06 - dandris@microsoft.com
* Resolved restriction number collision by renaming duplicate `A7352` and `A7353` to `A7355` and `A7356`, respectively
* Fixed broken links

## 2018-10-23 - dandris@microsoft.com
* Added action [`state`](#semantic-action-state)

## 2018-09-27 - dandris@microsoft.com
* Revised reference descriptions and links
* Clarified syntactic rules, revised `A2003`, added `A2007`
* Removed `A7743` as redundant
* Removed ordering requirement for semantic action entities (`A7741`)
* Added `$instance` to semantic action entities
* Added action type downgrading
* Move `A7600` and `A7601` and re-introduce as `A2104` and `A2105`

## 2018-09-18 - toddne@microsoft.com
* Added localTimezone property

## 2018-08-27 - daveta@microsoft.com
* Added [Channel account role](#channel-account-role) property
* Added [Conversation account role](#conversation-account-role) property

## 2018-07-17 - dandris@microsoft.com
* Added [`semanticAction`](#semantic-action)
* Added [handoff activity](#handoff-activity)

## 2018-07-05 - dandris@microsoft.com
* Changed `RXXXX` (*R*equiment) to `AXXXX` (*A*ctivity) to match other Bot Framework specifications.

## 2018-04-11 - dandris@microsoft.com
* Added [Listen for](#listen-for) field and [Appendix IV](#appendix-iv---priming-format)

## 2018-04-08 - tomlm@microsoft.com
* Added [Suggestion activity](#suggestion-activity) and [`textHighlight`](#text-highlight) complex type.
* Amended `A2071` to allow suggestion activities to specify receipients

## 2018-03-07 - dandris@microsoft.com
* Added [`conversationAccount.conversationType`](#conversation-account-conversation-type) and `A2084`.

## 2018-02-07 - dandris@microsoft.com
* Initial draft

# Appendix II - Non-IRI entity types

Activity [entities](#entity) communicate extra metadata about the activity, such as a user's location or the version of the messaging app they're using. Activity types are intended to be IRIs, but a small list of non-IRI names are in common use. This appendix is an exhaustive list of the supported non-IRI entity types.

| Type           | IRI equivalent                          | Description               |
| -------------- | --------------------------------------- | ------------------------- |
| GeoCoordinates | http://schema.org/GeoCoordinates/       | Schema.org GeoCoordinates |
| Mention        | https://botframework.com/schema/mention | @-mention                 |
| Place          | http://schema.org/Place                 | Schema.org Place          |
| Thing          | http://schema.org/Thing                 | Schema.org Thing          |
| string         | N/A                                     | String                    |
| number         | N/A                                     | Number                    |
| clientInfo     | N/A                                     | Skype client info         |
| Claim          | https://schema.org/Claim                | Schema.org Claim          |

### string and number

The `string` and `number` primitive entity types contains simple values within the `value` field. The `value` field is a string or number, respectively.

Example:
```
{
    "type": "string",
    "value": "This is the string value"
}
```

### clientInfo

The `clientInfo` entity type contains extended information about the client software used to send a user's message. It contains three properties, all of which are optional.

`A9201`: Bots SHOULD NOT send the `clientInfo` entity.

`A9202`: Senders SHOULD include the `clientInfo` entity only when one or more fields are populated.

#### Locale (Deprecated)

The `locale` field contains the user's locale. This field duplicates the [`locale`](#locale) field in the Activity root. The value of the `locale` field is an [ISO 639](https://www.iso.org/iso-639-language-codes.html) [[6](#references)] code within a string.

The `locale` field within `clientInfo` is deprecated.

`A9211`: Receivers SHOULD NOT use the `locale` field within the `clientInfo` object.

`A9212`: Senders MAY populate the `locale` field within `clientInfo` for compatibility reasons. If compatibility with older receivers is not required, senders SHOULD NOT send the `locale` property.

#### Country

The `country` field contains the user's detected location. This value may differ from any [`locale`](#locale) data as the `country` is detected whereas `locale` is typically a user or application setting. The value of the `country` field is an [ISO 3166-1](https://www.iso.org/iso-3166-country-codes.html) [[11](#references)] 2- or 3-letter country code.

`A9220`: Channels SHOULD NOT allow clients to specify arbitrary values for the `country` field. Channels SHOULD use a mechanism like GPS, location API, or IP address detection to establish the country generating a request.

#### Platform

The `platform` field describes the messaging client platform used to generate the activity. The value of the `platform` field is a string and the list of possible values and their meaning is defined by the channel sending them.

Note that on channels with a persistent chat feed, `platform` is typically useful only in deciding which content to include, not the format of that content. For instance, if a user on a mobile device asks for product support help, a bot could generate help specific to their mobile device. However, the user may then re-open the chat feed on their PC so they can read it on that screen while making changes to their mobile device. In this situation, the `platform` field is intended to inform the content, but the content should be viewable on other devices.

`A9230`: Bots SHOULD NOT use the `platform` field to control how response data is formatted unless they have specific knowledge that the content they are sending may only ever be seen on the device in question.

#### Schema.org Claim

A response may sometimes contains references to other work to support facts claimed in the message. These references provide transparency to human users. Human users may use these references to understand system's intended uses, interpret relevant system behavior effectively, and remain aware of the possible tendency of over-relying on outputs produced by the system.

When a reference is not fully accessible by the user, the bot can use [Schema.org Claim](https://schema.org/Claim) to include full or partial text from the original work. The claim is an extension to the activity and must not be cross-referenced from another activity.

Channels decide how the referenced content can be displayed in their user experience. In most cases, contents should be shown in a separate pane, popover, or dialog. In cases where the channel has limited capability to display the content, the channel may use an alternative channel (e.g. a link to a website, or sending the content via email).

`A9240`: The entity MUST include a `type` field, with value of `https://schema.org/Claim`.

`A9241`: The entity MUST include an `@context` field, with value of `https://schema.org`.

`A9242`: The entity MUST include an `@type` field, with value of `Claim`.

`A9243`: The entity SHOULD include an `@id` field, with value of type IRI unique to the activity and referenced in the message `text` field.

`A9244`: The entity MUST include a `text` field of type string, containing full, excerpted, or summarized text from the original work.

`A9245`: The entity MAY include a `name` field of type string.

# Appendix III - Protocols using the Invoke activity

The [invoke activity](#invoke-activity) is designed for use only within protocols supported by Bot Framework channels (i.e., it is not a generic extensibility mechanism). This appendix contains a list of all Bot Framework protocols using this activity.

## Teams compose extension

The Microsoft Teams channel uses Invoke for [compose extensions](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/messaging-extensions). This use of Invoke is specific to Microsoft Teams.

# Appendix IV - Priming format

The [`listenFor`](#listen-for) field within the Bot Framework activity schema contains a list of terms and references to hint to a speech or language processor which terms to prefer when processing input. This format is referred to within this appendix as the priming format.

The format allows:
1. Phrases, including single-term phrases (e.g. "house", "open the doors")
2. Sources of phrases (e.g., a LUIS model)

All contents appear within a flat array of strings.

Phrases are specified directly without markup. Sources are enclosed by `{` and `}` and their contents may be either a full URI or a shortened URI adhering to one of the conventions below.

`A9300`: Senders SHOULD NOT include punctuation that does not affect language processing, such as enclosing parentheses or trailing periods. Senders MUST NOT enclose plain phrases with leading `{` and trailing `}`.

`A9301`: Senders MAY include phrase sources by URI or short form. Phrase source URIs and short forms MUST be enclosed by leading `{` and trailing `}`. Senders SHOULD NOT include whitespace before or after `{` and `}` characters.

`A9302`: Senders MUST URI-encode any `{`, `}`, and `"` characters that occur within a phrase source.

`A9303`: Processors SHOULD ignore phrase sources they do not understand.

Phrase source URIs may be symbolic or may be URLs. For example, `https://example.com/language/models/1234#intent0001` could identify `intent0001` within the `example.com` language model with ID `1234`. The format of these URIs and relationship to the backing phrase source data is bounded by the `example.com` hostname and is established entirely by the service supporting that name. Processors must have specific knowledge of each phrase source to determine the method of extraction. In some cases, an HTTP GET directly to the URL is adequate, in others, an entire language model may be retrieved and parsed to select individual phrases. A processor knows how to retrieve the data by inspecting the hostname.

`A9304`: Processors MAY upgrade phrase source URIs to URLs when they have knowledge that the source supports phrase retrieval.

`A9305`: Processors SHOULD use plain ordinal hostname comparisons when determining whether they recognize a source URI.

`A9306`: Processors MAY ignore any resolved URLs that are not HTTPS.

Phrase sources specified in complete URIs (e.g. `https://luis.ai/apps/12345#intent0001`) can be verbose and this spec establishes a format for shortening these URIs, and one known format specifically for [LUIS](https://luis.ai) apps.

`A9307`: If a processor supports a phrase source, and that source has a compact representation, processors SHOULD support both the compact and the expanded format.

`A9308`: Phrase sources SHOULD establish a well-defined mapping between compact and expanded forms of phrase source references.

### LUIS.ai phrase source

This section provides a definition for the [LUIS](https://luis.ai) phrase source URI and compact form.

LUIS.ai models can be referenced with the following format:
    `https://luis.ai/apps/<appId>[/intents/<intentId>]`

The short form for these IDs is:
    `luis:<appId>[#intentId]`

# Appendix V - Caller ID Values

The Activity schema includes a ['callerId'](#caller-Id) field that identifies the caller sending an activity. The field is not populated on the wire but is used for internal routing within a bot, and for tracking this data when an Activity is committed to storage (in e.g. the [Transcript](../transcript/transcript.md) [[#16](#references)] format).

This specification defines three IRI schemes for caller IDs.

It is important that implementers follow the verification rules in this appendix to ensure a caller ID is set only when the caller is successfully authenticated.

## Bot Framework

Bot Framework services call registered bots to transmit messages sent over Bot Framework channels. The caller ID for any Bot Framework channel service is `urn:botframework:azure`.

The authenticity of a call from Bot Framework can be established by inspecting its JSON Web Token and ensuring it is both correctly formed and is signed with a key listed in the Bot Framework Open ID Metadata Document.

## Bot Framework (US Government Cloud)

Bot Framework services running in the US Government Cloud are in a distinct security domain from traditional Bot Framework serivces. The caller ID for any Bot Framework channel service is `urn:botframework:azureusgov`.

The authenticity of a call from Bot Framework US Government Cloud can be established by inspecting its JSON Web Token and ensuring it is both correctly formed and is signed with a key listed in the Bot Framework US Government Cloud Open ID Metadata Document.

## Bot calling skill

The Activity schema can be used when a bot initiates a request to another bot acting as a skill. The caller ID for these calls is the prefix `urn:botframework:aadappid:` followed by the Azure Active Directory App ID used by the bot initiating the call.

The authenticity of a call from a bot can be established by inspecting its JSON Web Token and ensuring it is both correctly formed and is signed with a key listed in the Azure Active Directory Open ID Metadata Document.

# Appendix VI - Protocols using the Command activity
[Command activities](#command-activity) communicate a request to perform a specific action. Command activities outside the `application` are considered reserved for Bot Framework Protocols. This appendix contains a list of command activities used in Bot Framework protocols and recommended patterns for defining and using command activities.

## Telephony Channel

The Microsoft Telephony channel defines channel command activities in the namespace `channel/vnd.microsoft.telephony.<action>`.

## Patterns for rejecting commands

### General pattern for rejecting commands
The general pattern for rejecting commands is to send an asynchronous commands result with an error. This is used in most cases when the receiver needs to process the command before rejecting it.

Here is an example of a command result indicating that the command was not supported by the receiver:
```
    {
        'type': 'commandResult'
        'name': 'channel/vnd.microsoft.telephony.<action>'
        'value': {
            'error' : {
                'code': 'NotSupported',
                'message' : 'Command channel/vnd.microsoft.telephony.<action> is not supported'
            }
        }
    }
```

### Channel rejecting commands
Some channels may may not support the command protocol or not allow for application customization of commands. In this case, the channel can reject commands with a transport-level response codes to allow a sender to detect the command activity was rejected. Example: When the transport is HTTP, 200 indicates acceptance and 400 indicates that the Activity name is not supported.
