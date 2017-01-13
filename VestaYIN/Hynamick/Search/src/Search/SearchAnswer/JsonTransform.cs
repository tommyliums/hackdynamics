// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTransform.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the JsonTransform.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonTransform
    {
        /// <summary>
        ///     Action type : ApplyTemplate
        /// </summary>
        public const string ApplyTemplate = "ApplyTemplate";

        /// <summary>
        ///     Action type : SingleIntValue
        /// </summary>
        public const string SingleIntValue = "SingleIntValue";

        /// <summary>
        ///     Action type : SingleBoolValue
        /// </summary>
        public const string SingleBoolValue = "SingleBoolValue";

        /// <summary>
        ///     Action type : SingleStringValue
        /// </summary>
        public const string SingleStringValue = "SingleStringValue";

        /// <summary>
        ///     Action type : SingleDecimalValue
        /// </summary>
        public const string SingleDecimalValue = "SingleDecimalValue";

        /// <summary>
        ///     Action type : SingleDoubleValue
        /// </summary>
        public const string SingleDoubleValue = "SingleDoubleValue";

        /// <summary>
        ///     Action type : SingleLongValue
        /// </summary>
        public const string SingleLongValue = "SingleLongValue";

        /// <summary>
        ///     Action type : SingleFloatValue
        /// </summary>
        public const string SingleFloatValue = "SingleFloatValue";

        /// <summary>
        ///     Action type : SingleDateValue
        /// </summary>
        public const string SingleDateValue = "SingleDateValue";

        /// <summary>
        ///     Action type : SingleGuidValue
        /// </summary>
        public const string SingleGuidValue = "SingleGuidValue";

        /// <summary>
        ///     The result json response format
        /// </summary>
        private readonly string jsonResultTemplate;

        /// <summary>
        ///     Templates to transform the original response
        /// </summary>
        private readonly Dictionary<string, JsonTransformTemplate> jsonTemplates;

        /// <summary>
        ///     Extract action from transform config, can be overrided as it's public members
        /// </summary>
        public Regex ExtractActionExtractRegex = new Regex(@"#[\s]*({[^\{\}]*})[\s]*(,?)", RegexOptions.Compiled);

        /// <summary>
        ///     Extract template from transform config, can be overrided as it's public members
        /// </summary>
        public Regex ExtractResultBodyRegex = new Regex("@source([^@]*)", RegexOptions.Compiled);

        /// <summary>
        ///     Extract template from transform config, can be overrided as it's public members
        /// </summary>
        public Regex ExtractTemplateExtractRegex = new Regex("@template([^@]*)", RegexOptions.Compiled);

        /// <summary>
        ///     Initializes static members of the <see cref="JsonTransform" /> class.
        /// </summary>
        /// <param name="templateFileContent"></param>
        public JsonTransform(string templateFileContent)
        {
            //Extract json source string
            var sourceMatch = this.ExtractResultBodyRegex.Match(templateFileContent);
            if (sourceMatch.Success)
            {
                this.jsonResultTemplate = sourceMatch.Groups[1].Value.Trim();
                this.jsonTemplates = new Dictionary<string, JsonTransformTemplate>();

                //Extract template
                foreach (Match templateMatch in this.ExtractTemplateExtractRegex.Matches(templateFileContent))
                {
                    var template = JsonConvert.DeserializeObject<JsonTransformTemplate>(templateMatch.Groups[1].Value);
                    this.jsonTemplates[template.Name] = template;
                }
            }
            else
            {
                var exception =
                    new ArgumentException(
                        $"Fail to find result body in given template file content: {templateFileContent.Substring(0, 30)}");
                throw exception;
            }
        }

        /// <summary>
        ///     Select jPath value from given source JToken.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="source">Input JToken instance.</param>
        /// <param name="jPath">Json path.</param>
        /// <param name="noQuote">
        ///     For referene type of T, whether surround double quote. For generate combine value, set this to
        ///     true.
        /// </param>
        /// <returns>Selected value base on JPath.</returns>
        public static string SelectValue<T>(JToken source, string jPath, bool noQuote = false)
        {
            var value = source.SelectToken(jPath);
            if (value == null)
            {
                return typeof(T).GetTypeInfo().IsValueType
                    ? JsonConvert.SerializeObject(default(T))
                    : JsonConvert.SerializeObject(null);
            }

            if (value.GetType() != typeof(JValue))
            {
                return JsonConvert.SerializeObject(value);
            }

            return noQuote ? (string)value : JsonConvert.SerializeObject(value);
        }

        /// <summary>
        ///     Transform the json result by the template, return deserialized result
        /// </summary>
        /// <typeparam name="T">the type result will be deserialized</typeparam>
        /// <param name="sourceResult">Source json token</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>deserialized result</returns>
        public T Transform<T>(JToken sourceResult, params JsonConverter[] converters)
        {
            var transformedResult = this.ExtractActionContent(sourceResult, this.jsonResultTemplate);
            ////var transformedResult = this.extractActionExtractRegex.Replace(this.jsonResultTemplate, m =>
            ////{
            ////    var action = JsonConvert.DeserializeObject<JsonTransformAction>(m.Groups[1].Value);
            ////    return this.Execute(action, sourceResult);
            ////});
            return JsonConvert.DeserializeObject<T>(transformedResult, converters);
        }

        private string ExtractActionContent(JToken source, string content)
        {
            return this.ExtractActionExtractRegex.Replace(content, m =>
            {
                var action = JsonConvert.DeserializeObject<JsonTransformAction>(m.Groups[1].Value);
                var response = this.Execute(action, source);
                // In action extract regex, it will capture suffix comma. And append captured comma to response.
                // This feature is useful for conditional capture.
                // If configured action captures null value, node will skip and tailing comma will be removed.
                return response != null && m.Groups.Count == 3 && m.Groups[2].Value == ","
                    ? string.Concat(response, ",")
                    : response;
            });
        }

        /// <summary>
        ///     Action execute
        /// </summary>
        /// <param name="action">Json transform action</param>
        /// <param name="source">Json source</param>
        /// <returns>result string</returns>
        private string Execute(JsonTransformAction action, JToken source)
        {
            switch (action.Type)
            {
                case SingleStringValue:
                    return SelectValue<string>(source, action.Select, action.NoQuote);
                case ApplyTemplate:
                    {
                        JsonTransformTemplate template;
                        if (this.jsonTemplates.TryGetValue(action.TemplateName, out template))
                        {
                            return this.ProcessApplyTemplate(source, action.Select, this.jsonTemplates[action.TemplateName]);
                        }

                        throw new KeyNotFoundException(
                            $"Template name {action.TemplateName} is not found in the template file");
                    }
                case SingleIntValue:
                    return SelectValue<int>(source, action.Select, action.NoQuote);
                case SingleBoolValue:
                    return SelectValue<bool>(source, action.Select, action.NoQuote);
                case SingleDateValue:
                    return SelectValue<DateTime>(source, action.Select, action.NoQuote);
                case SingleDoubleValue:
                    return SelectValue<double>(source, action.Select, action.NoQuote);
                case SingleDecimalValue:
                    return SelectValue<decimal>(source, action.Select, action.NoQuote);
                case SingleLongValue:
                    return SelectValue<long>(source, action.Select, action.NoQuote);
                case SingleFloatValue:
                    return SelectValue<float>(source, action.Select, action.NoQuote);
                case SingleGuidValue:
                    return SelectValue<Guid>(source, action.Select, action.NoQuote);
                default:
                    throw new NotSupportedException($"Given action type {action.Type} wasn't recognized.");
            }
        }

        /// <summary>
        ///     Applies the template and return result string
        /// </summary>
        /// <param name="source">the root json token</param>
        /// <param name="jPath">Json select path</param>
        /// <param name="template">template</param>
        /// <returns>result string</returns>
        private string ProcessApplyTemplate(JToken source, string jPath, JsonTransformTemplate template)
        {
            var items = source.SelectTokens(jPath);
            var result = new StringBuilder();
            foreach (var token in items)
            {
                var transformedResult = this.ExtractActionContent(token, template.Content);
                ////var transformedResult = this.extractActionExtractRegex.Replace(template.Content, m =>
                ////{
                ////    var action = JsonConvert.DeserializeObject<JsonTransformAction>(m.Groups[1].Value);
                ////    return this.Execute(action, token);
                ////});

                if (!string.IsNullOrWhiteSpace(transformedResult) && transformedResult != "null")
                {
                    result.Append(transformedResult).Append(',');
                }
            }

            return result.Length == 0 ? null : result.ToString(0, result.Length - 1);
        }
    }
}