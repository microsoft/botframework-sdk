﻿// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Bot Builder SDK GitHub:
// https://github.com/Microsoft/BotBuilder
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Linq;

using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace Microsoft.Bot.Builder.FormFlow
{
    /// <summary>
    /// Abstract base class for FormFlow attributes.
    /// </summary>
    [Serializable]
    public abstract class FormFlowAttribute : Attribute
    {
        /// <summary>
        /// True if attribute is localizable.
        /// </summary>
        /// <remarks>
        /// Attributes that are used on classes, fields and properties should have this set.
        /// That way those attributes will be in the localization files that are generated.
        /// </remarks>
        public bool IsLocalizable { get; set; } = true;
    }

    /// <summary>
    /// Attribute to override the default description of a field, property or enum value.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property)]
    public class DescribeAttribute : FormFlowAttribute
    {
        /// <summary>
        /// Description of the field, property or enum to use in templates and choices.
        /// </summary>
        public string Description;

        /// <summary>
        /// Title when a card is generated from description.
        /// </summary>
        public string Title;

        /// <summary>
        /// SubTitle when a card is generated from description.
        /// </summary>
        public string SubTitle;

        /// <summary>
        /// URL of image to use when creating cards or buttons.
        /// </summary>
        public string Image;

        /// <summary>
        /// Message to return when a button is pressed in a card.
        /// </summary>
        public string Message;

        /// <summary>
        /// Description for field, property or enum value.
        /// </summary>
        /// <param name="description">Description of field, property or enum value.</param>
        /// <param name="image">URL of image to use when generating buttons.</param>
        /// <param name="message">Message to return from button.</param>
        /// <param name="title">Text if generating card.</param>
        /// <param name="subTitle">SubTitle if generating card.</param>
        public DescribeAttribute(string description = null, string image = null, string message = null, string title = null, string subTitle = null)
        {
            Description = description;
            Title = title;
            SubTitle = subTitle;
            Image = image;
            Message = message;
        }
    }

    /// <summary>
    /// Attribute to override the default terms used to match a field, property or enum value to user input.
    /// </summary>
    /// <remarks>
    /// By default terms are generated by calling the <see cref="Advanced.Language.GenerateTerms(string, int)"/> method with a max phrase length of 3
    /// on the name of the field, property or enum value.  Using this attribute you can specify your own regular expressions to match or if you specify the 
    /// <see cref="MaxPhrase"/> attribute you can cause <see cref="Advanced.Language.GenerateTerms(string, int)"/> to be called on your strings with the
    /// maximum phrase length you specify.  If your term is a simple alphanumeric one, then it will only be matched on word boundaries with \b unless you start your
    /// expression with parentheses in which case you control the boundary matching behavior through your regular expression.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TermsAttribute : FormFlowAttribute
    {
        /// <summary>
        /// Regular expressions for matching user input.
        /// </summary>
        public string[] Alternatives;

        private int _maxPhrase;
        /// <summary>
        /// The maximum pharse length to use when calling <see cref="Advanced.Language.GenerateTerms(string, int)"/> on your supplied terms.
        /// </summary>
        public int MaxPhrase
        {
            get
            {
                return _maxPhrase;
            }
            set
            {
                _maxPhrase = value;
                Alternatives = Alternatives.SelectMany(alt => Advanced.Language.GenerateTerms(alt, _maxPhrase)).ToArray();
            }
        }

        /// <summary>
        /// Regular expressions or terms used when matching user input.
        /// </summary>
        /// <remarks>
        /// If <see cref="MaxPhrase"/> is specified the supplied alternatives will be passed to <see cref="Advanced.Language.GenerateTerms(string, int)"/> to generate regular expressions
        /// with a maximum phrase size of <see cref="MaxPhrase"/>.
        /// </remarks>
        /// <param name="alternatives">Regular expressions or terms.</param>
        public TermsAttribute(params string[] alternatives)
        {
            Alternatives = alternatives;
        }
    }

    /// <summary>
    /// Specifies how to show choices generated by {||} in a \ref patterns string.
    /// </summary>
    public enum ChoiceStyleOptions
    {
        /// <summary>
        /// Use the default <see cref="TemplateBaseAttribute.ChoiceStyle"/> from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Automatically choose how to render choices.
        /// </summary>
        Auto,

        /// <summary>
        /// Automatically generate text and switch between the <see cref="Inline"/> and <see cref="PerLine"/> styles based on the number of choices.
        /// </summary>
        AutoText,

        /// <summary>
        /// Show choices on the same line.
        /// </summary>
        Inline,

        /// <summary>
        /// Show choices with one per line.
        /// </summary>
        PerLine,

        /// <summary>   Show choices on the same line without surrounding parentheses. </summary>
        InlineNoParen,

        /// <summary>
        /// Show choices as buttons if possible.
        /// </summary>
        Buttons,

        /// <summary>
        /// Show choices as a carousel if possibe.
        /// </summary>
        Carousel
    };


    /// <summary>
    /// How to normalize the case of words.
    /// </summary>
    public enum CaseNormalization
    {

        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// First letter of each word is capitalized
        /// </summary>
        InitialUpper,

        /// <summary>
        /// Normalize words to lower case.
        /// </summary>
        Lower,

        /// <summary>
        /// Normalize words to upper case.
        /// </summary>
        Upper,

        /// <summary>
        /// Don't normalize words.
        /// </summary>
        None
    };

    /// <summary>
    /// Three state boolean value.
    /// </summary>
    /// <remarks>
    /// This is necessary because C# attributes do not support nullable properties.
    /// </remarks>
    public enum BoolDefault
    {
        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Boolean true.
        /// </summary>
        True,

        /// <summary>
        /// Boolean false.
        /// </summary>
        False
    };

    /// <summary>
    /// Control how the user gets feedback after each entry.
    /// </summary>
    public enum FeedbackOptions
    {
        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Provide feedback using the <see cref="TemplateUsage.Feedback"/> template only if part of the user input was not understood.
        /// </summary>
        Auto,

        /// <summary>
        /// Provide feedback after every user input.
        /// </summary>
        Always,

        /// <summary>
        /// Never provide feedback.
        /// </summary>
        Never
    };

    /// <summary>
    /// Define the prompt used when asking about a field.
    /// </summary>
    /// <remarks>
    /// Prompts by default will come from \ref Templates.  
    /// This attribute allows you to override this with one more \ref patterns strings. 
    /// The actual prompt will be randomly selected from the alternatives you provide.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PromptAttribute : TemplateBaseAttribute
    {
        /// <summary>
        /// Define a prompt with one or more \ref patterns patterns to choose from randomly.
        /// </summary>
        /// <param name="patterns">Patterns to select from.</param>
        public PromptAttribute(params string[] patterns)
            : base(patterns)
        { }

        /// <summary>
        /// Define a prompt based on a <see cref="TemplateAttribute"/>.
        /// </summary>
        /// <param name="pattern">Template to use.</param>
        public PromptAttribute(TemplateAttribute pattern)
            : base(pattern)
        {
            IsLocalizable = false;
        }
    }

    /// <summary>
    /// All of the built-in templates.
    /// </summary>
    /// <remarks>
    /// A good way to understand these is to look at the default templates defined in <see cref="FormConfiguration.Templates"/>
    /// </remarks>
    public enum TemplateUsage
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,

        /// <summary>
        /// How to ask for an attachment collection. 
        /// </summary>
        AttachmentCollection,

        /// <summary>
        /// How to display attachment collection status.
        /// </summary>
        AttachmentCollectionDescription,

        /// <summary>
        /// What you can enter when entering an attachment collection.
        /// </summary>
        AttachmentCollectionHelp,

        /// <summary>
        /// How to display attachment content-type validator errors.
        /// </summary>
        AttachmentContentTypeValidatorError,

        /// <summary>
        /// How to display attachment content-type validator help.
        /// </summary>
        AttachmentContentTypeValidatorHelp,

        /// <summary>
        /// How to ask for an attachment collection. 
        /// </summary>
        AttachmentField,

        /// <summary>
        /// How to display an attachment status.
        /// </summary>
        AttachmentFieldDescription,

        /// <summary>
        /// What you can enter when entering an attachment collection.
        /// </summary>
        AttachmentFieldHelp,

        /// <summary>
        /// How to ask for a boolean.
        /// </summary>
        Bool,

        /// <summary>
        /// What you can enter when entering a bool.
        /// </summary>
        /// <remarks>
        /// Within this template {0} is the current choice if any and {1} is no preference if optional. 
        /// </remarks>
        BoolHelp,

        /// <summary>
        /// Clarify an ambiguous choice.
        /// </summary>
        /// <remarks>This template can use {0} to capture the term that was ambiguous.</remarks>
        Clarify,

        /// <summary>
        /// Default confirmation.
        /// </summary>
        Confirmation,

        /// <summary>
        /// Show the current choice.
        /// </summary>
        /// <remarks>
        /// This is how the current choice is represented as an option.  
        /// If you change this, you should also change <see cref="FormConfiguration.CurrentChoice"/>
        /// so that what people can type matches what you show.
        /// </remarks>
        CurrentChoice,

        /// <summary>
        /// How to ask for a <see cref="DateTime"/>.
        /// </summary>
        DateTime,

        /// <summary>
        /// What you can enter when entering a <see cref="DateTime"/>.
        /// </summary>
        /// <remarks>
        /// Within this template {0} is the current choice if any and {1} is no preference if optional. 
        /// </remarks>
        /// <remarks>
        /// This template can use {0} to get the current choice or {1} for no preference if field is optional.
        /// </remarks>
        DateTimeHelp,

        /// <summary>
        /// How to ask for a double.
        /// </summary>
        /// <remarks>
        /// Within this template if numerical limits are specified using <see cref="NumericAttribute"/>, 
        /// {0} is the minimum possible value and {1} is the maximum possible value.
        /// </remarks>
        Double,

        /// <summary>
        /// What you can enter when entering a double.
        /// </summary>
        /// <remarks>
        /// Within this template {0} is the current choice if any and {1} is no preference if optional. 
        /// If limits are specified through <see cref="NumericAttribute"/>, then {2} will be the minimum possible value 
        /// and {3} the maximum possible value.
        /// </remarks>
        /// <remarks>
        /// Within this template, {0} is current choice if any, {1} is no preference for optional  and {1} and {2} are min/max if specified.
        /// </remarks>
        DoubleHelp,

        /// <summary>
        /// What you can enter when selecting a single value from a numbered enumeration.
        /// </summary>
        /// <remarks>
        /// Within this template, {0} is the minimum choice. {1} is the maximum choice and {2} is a description of all the possible words.
        /// </remarks>
        EnumOneNumberHelp,

        /// <summary>
        ///  What you can enter when selecting multiple values from a numbered enumeration.
        /// </summary>
        /// <remarks>
        /// Within this template, {0} is the minimum choice. {1} is the maximum choice and {2} is a description of all the possible words.
        /// </remarks>
        EnumManyNumberHelp,

        /// <summary>
        /// What you can enter when selecting one value from an enumeration.
        /// </summary>
        /// <remarks>
        /// Within this template, {2} is a list of the possible values.
        /// </remarks>
        EnumOneWordHelp,

        /// <summary>
        /// What you can enter when selecting mutiple values from an enumeration.
        /// </summary>
        /// <remarks>
        /// Within this template, {2} is a list of the possible values.
        /// </remarks>
        EnumManyWordHelp,

        /// <summary>
        /// How to ask for one value from an enumeration.
        /// </summary>
        EnumSelectOne,

        /// <summary>
        /// How to ask for multiple values from an enumeration.
        /// </summary>
        EnumSelectMany,

        /// <summary>
        /// How to show feedback after user input.
        /// </summary>
        /// <remarks>
        /// Within this template, unmatched input is available through {0}, but it should be wrapped in an optional {?} in \ref patterns in case everything was matched. 
        /// </remarks>
        Feedback,

        /// <summary>
        /// What to display when asked for help.
        /// </summary>
        /// <remarks>
        /// This template controls the overall help experience.  {0} will be recognizer specific help and {1} will be command help.
        /// </remarks>
        Help,

        /// <summary>
        /// What to display when asked for help while clarifying. 
        /// </summary>
        /// <remarks>
        /// This template controls the overall help experience.  {0} will be recognizer specific help and {1} will be command help.
        /// </remarks>
        HelpClarify,

        /// <summary>
        /// What to display when asked for help while in a confirmation.
        /// </summary>
        /// <remarks>
        /// This template controls the overall help experience.  {0} will be recognizer specific help and {1} will be command help.
        /// </remarks>
        HelpConfirm,

        /// <summary>
        /// What to display when asked for help while navigating.
        /// </summary>
        /// <remarks>
        /// This template controls the overall help experience.  {0} will be recognizer specific help and {1} will be command help.
        /// </remarks>
        HelpNavigation,

        /// <summary>
        /// How to ask for an integer.
        /// </summary>
        /// <remarks>
        /// Within this template if numerical limits are specified using <see cref="NumericAttribute"/>, 
        /// {0} is the minimum possible value and {1} is the maximum possible value.
        /// </remarks>
        Integer,

        /// <summary>
        /// What you can enter while entering an integer.
        /// </summary>
        /// <remarks>
        /// Within this template, {0} is current choice if any, {1} is no preference for optional  and {1} and {2} are min/max if specified.
        /// </remarks>
        IntegerHelp,

        /// <summary>
        /// How to ask for a navigation.
        /// </summary>
        Navigation,

        /// <summary>
        /// Help pattern for navigation commands. 
        /// </summary>
        /// <remarks>
        /// Within this template, {0} has the list of possible field names.
        /// </remarks>
        NavigationCommandHelp,

        /// <summary>
        /// Navigation format for one line in navigation choices.
        /// </summary>
        NavigationFormat,

        /// <summary>
        /// What you can enter when navigating.
        /// </summary>
        /// <remarks>
        /// Within this template, if numeric choies are allowed {0} is the minimum possible choice 
        /// and {1} the maximum possible choice. 
        /// </remarks>
        NavigationHelp,

        /// <summary>
        /// How to show no preference in an optional field.
        /// </summary>
        NoPreference,

        /// <summary>
        /// Response when an input is not understood.
        /// </summary>
        /// <remarks>
        /// When no input is matched this template is used and gets {0} for what the user entered.
        /// </remarks>
        NotUnderstood,

        /// <summary>
        /// Format for one entry in status.
        /// </summary>
        StatusFormat,

        /// <summary>
        /// How to ask for a string.
        /// </summary>
        String,

        /// <summary>
        /// What to display when asked for help when entering a string. 
        /// </summary>
        /// <remarks>
        /// Within this template {0} is the current choice if any and {1} is no preference if optional. 
        /// </remarks>
        StringHelp,

        /// <summary>
        /// How to represent a value that has not yet been specified.
        /// </summary>
        Unspecified
    };

    /// <summary>
    /// Define a template for generating strings.
    /// </summary>
    /// <remarks>
    /// Templates provide a pattern that uses the template language defined in \ref patterns.  See <see cref="TemplateUsage"/> to see a description of all the different kinds of templates.
    /// You can also look at <see cref="FormConfiguration.Templates"/> to see all the default templates that are provided.  Templates can be overriden at the form, class/struct of field level.  
    /// They also support randomly selecting between templates which is a good way to introduce some variation in your responses.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class TemplateAttribute : TemplateBaseAttribute
    {
        /// <summary>
        /// What kind of template this is.
        /// </summary>
        public readonly TemplateUsage Usage;

        /// <summary>
        /// Specify a set of templates to randomly choose between for a particular usage.
        /// </summary>
        /// <param name="usage">How the template will be used.</param>
        /// <param name="patterns">The set of \ref patterns to randomly choose from.</param>
        public TemplateAttribute(TemplateUsage usage, params string[] patterns)
            : base(patterns)
        {
            Usage = usage;
        }

        #region Documentation
        /// <summary>   Initialize from another template. </summary>
        /// <param name="other">    The other template. </param>
        #endregion
        public TemplateAttribute(TemplateAttribute other)
            : base(other)
        {
            Usage = other.Usage;
        }
    }

    /// <summary>
    /// Define a field or property as optional.
    /// </summary>
    /// <remarks>
    /// An optional field is one where having no value is an acceptable response.  By default every field is considered required and must be filled in to complete the form.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OptionalAttribute : Attribute
    {
        /// <summary>
        /// Mark a field or property as optional.
        /// </summary>
        public OptionalAttribute()
        { }
    }

    /// <summary>
    /// Provide limits on the possible values in a numeric field or property.
    /// </summary>
    /// <remarks>
    /// By default the limits are the min and max of the underlying field type.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NumericAttribute : Attribute
    {
        /// <summary>
        /// Min possible value.
        /// </summary>
        public readonly double Min;

        /// <summary>
        /// Max possible value.
        /// </summary>
        public readonly double Max;

        /// <summary>
        /// Specify the range of possible values for a number field.
        /// </summary>
        /// <param name="min">Min value allowed.</param>
        /// <param name="max">Max value allowed.</param>
        public NumericAttribute(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }

    /// <summary>
    /// Provide a regular expression to validate a string field.
    /// </summary>
    /// <remarks>
    /// If the regular expression is not matched the <see cref="TemplateUsage.NotUnderstood"/> template will be used for feedback.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PatternAttribute : Attribute
    {
        public readonly string Pattern;

        /// <summary>
        /// Regular expression for validating the content of a string field.
        /// </summary>
        /// <param name="pattern">Regular expression for validation.</param>
        public PatternAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}

namespace Microsoft.Bot.Builder.FormFlow.Advanced
{
    /// <summary>
    /// Abstract base class used by all attributes that use \ref patterns.
    /// </summary>
    public abstract class TemplateBaseAttribute : FormFlowAttribute
    {
        private static Random _generator = new Random();

        /// <summary>
        /// When processing choices {||} in a \ref patterns string, provide a choice for the default value if present.
        /// </summary>
        public BoolDefault AllowDefault { get; set; }

        /// <summary>  
        /// Control case when showing choices in {||} references in a \ref patterns string. 
        /// </summary>
        public CaseNormalization ChoiceCase { get; set; }

        /// <summary>
        /// Format string used for presenting each choice when showing {||} choices in a \ref patterns string.
        /// </summary>
        /// <remarks>The choice format is passed two arguments, {0} is the number of the choice and {1} is the field name.</remarks>
        public string ChoiceFormat { get; set; }

        /// <summary>   
        /// When constructing inline lists of choices using {||} in a \ref patterns string, the string used before the last choice. 
        /// </summary>
        public string ChoiceLastSeparator { get; set; }

        /// <summary>  
        /// When constructing inline choice lists for {||} in a \ref patterns string controls whether to include parentheses around choices. 
        /// </summary>
        public BoolDefault ChoiceParens { get; set; }

        /// <summary>
        /// When constructing inline lists using {||} in a \ref patterns string, the string used between all choices except the last. 
        /// </summary>
        public string ChoiceSeparator { get; set; }

        /// <summary>
        /// How to display choices {||} when processed in a \ref patterns string.
        /// </summary>
        public ChoiceStyleOptions ChoiceStyle { get; set; }

        /// <summary>
        /// Control what kind of feedback the user gets after each input.
        /// </summary>
        public FeedbackOptions Feedback { get; set; }

        /// <summary>
        /// Control case when showing {&amp;} field name references in a \ref patterns string.
        /// </summary>
        public CaseNormalization FieldCase { get; set; }

        /// <summary>
        /// When constructing lists using {[]} in a \ref patterns string, the string used before the last value in the list.
        /// </summary>
        public string LastSeparator { get; set; }

        /// <summary>
        /// When constructing lists using {[]} in a \ref patterns string, the string used between all values except the last.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Control case when showing {} value references in a \ref patterns string.
        /// </summary>
        public CaseNormalization ValueCase { get; set; }

        internal bool AllowNumbers
        {
            get
            {
                // You can match on numbers only if they are included in Choices and choices are shown
                return ChoiceFormat.Contains("{0}") && Patterns.Any((pattern) => pattern.Contains("{||}"));
            }
        }

        /// <summary>
        /// The pattern to use when generating a string using <see cref="Advanced.IPrompt{T}"/>.
        /// </summary>
        /// <remarks>If multiple patterns were specified, then each call to this function will return a random pattern.</remarks>
        /// <returns>Pattern to use.</returns>
        public string Pattern()
        {
            var choice = 0;
            if (Patterns.Length > 1)
            {
                lock (_generator)
                {
                    choice = _generator.Next(Patterns.Length);
                }
            }
            return Patterns[choice];
        }

        /// <summary>
        /// All possible templates.
        /// </summary>
        /// <returns>The possible templates.</returns>
        public string[] Patterns { get; set; }

        /// <summary>
        /// Any default values in this template will be overridden by the supplied <paramref name="defaultTemplate"/>.
        /// </summary>
        /// <param name="defaultTemplate">Default template to use to override default values.</param>
        public void ApplyDefaults(TemplateBaseAttribute defaultTemplate)
        {
            if (AllowDefault == BoolDefault.Default) AllowDefault = defaultTemplate.AllowDefault;
            if (ChoiceCase == CaseNormalization.Default) ChoiceCase = defaultTemplate.ChoiceCase;
            if (ChoiceFormat == null) ChoiceFormat = defaultTemplate.ChoiceFormat;
            if (ChoiceLastSeparator == null) ChoiceLastSeparator = defaultTemplate.ChoiceLastSeparator;
            if (ChoiceParens == BoolDefault.Default) ChoiceParens = defaultTemplate.ChoiceParens;
            if (ChoiceSeparator == null) ChoiceSeparator = defaultTemplate.ChoiceSeparator;
            if (ChoiceStyle == ChoiceStyleOptions.Default) ChoiceStyle = defaultTemplate.ChoiceStyle;
            if (FieldCase == CaseNormalization.Default) FieldCase = defaultTemplate.FieldCase;
            if (Feedback == FeedbackOptions.Default) Feedback = defaultTemplate.Feedback;
            if (LastSeparator == null) LastSeparator = defaultTemplate.LastSeparator;
            if (Separator == null) Separator = defaultTemplate.Separator;
            if (ValueCase == CaseNormalization.Default) ValueCase = defaultTemplate.ValueCase;
        }

        /// <summary>
        /// Initialize with multiple patterns that will be chosen from randomly.
        /// </summary>
        /// <param name="patterns">Possible patterns.</param>
        public TemplateBaseAttribute(params string[] patterns)
        {
            Patterns = patterns;
            Initialize();
        }

        /// <summary>
        /// Initialize from another template.
        /// </summary>
        /// <param name="other">The template to copy from.</param>
        public TemplateBaseAttribute(TemplateBaseAttribute other)
        {
            Patterns = other.Patterns;
            AllowDefault = other.AllowDefault;
            ChoiceCase = other.ChoiceCase;
            ChoiceFormat = other.ChoiceFormat;
            ChoiceLastSeparator = other.ChoiceLastSeparator;
            ChoiceParens = other.ChoiceParens;
            ChoiceSeparator = other.ChoiceSeparator;
            ChoiceStyle = other.ChoiceStyle;
            FieldCase = other.FieldCase;
            Feedback = other.Feedback;
            LastSeparator = other.LastSeparator;
            Separator = other.Separator;
            ValueCase = other.ValueCase;
        }

        private void Initialize()
        {
            AllowDefault = BoolDefault.Default;
            ChoiceCase = CaseNormalization.Default;
            ChoiceFormat = null;
            ChoiceLastSeparator = null;
            ChoiceParens = BoolDefault.Default;
            ChoiceSeparator = null;
            ChoiceStyle = ChoiceStyleOptions.Default;
            FieldCase = CaseNormalization.Default;
            Feedback = FeedbackOptions.Default;
            LastSeparator = null;
            Separator = null;
            ValueCase = CaseNormalization.Default;
        }
    }
}