// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QaDialog.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the QaDialog.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.QaBot
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Hynamick.Search.SearchAnswer;

    using Microsoft.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class QaDialog : IDialog<object>
    {
        public static readonly SearchHandler Handler = InitializeSearchHandler();

        private static SearchHandler InitializeSearchHandler()
        {
            var Client = new HttpClient();
            var ServiceUrl = CloudConfigurationManager.GetSetting("SearchUrl");
            var localeMappingPath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
               CloudConfigurationManager.GetSetting("LocaleMappingPath"));
            var SearchTemplatePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
                CloudConfigurationManager.GetSetting("SearchTemplateFile"));
            var TransformFilePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
                CloudConfigurationManager.GetSetting("TransformFilePath"));
            return new SearchHandler(Client, ServiceUrl, localeMappingPath, SearchTemplatePath, TransformFilePath);
        }

        public static readonly int DefaultAnswerLength =
            int.Parse(ConfigurationManager.AppSettings["DefaultAnswerLength"]);

        public static readonly int DefaultAnswerCount = int.Parse(ConfigurationManager.AppSettings["DefaultAnswerCount"]);

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var queryString = this.ParseQuestion(message.Text);
            var resultEntities = await Handler.SearchAsync("HW", "zh-cn", queryString, DefaultAnswerCount);
            IDictionary<string, string> actions;
            var answerText = BuildText(queryString, resultEntities, out actions);

            await context.PostAsync(answerText, "zh-cn");
            context.Wait(this.MessageReceivedAsync);
        }

        public static Dictionary<string, string> BuildActions(SearchResponse resultEntities)
        {
            if (resultEntities.Error != null || resultEntities.Results == null || resultEntities.Results.Count <= 1)
            {
                return null;
            }

            var actions = new Dictionary<string, string>();

            return actions;
        }

        public static string BuildText(string query, SearchResponse resultEntities,
            out IDictionary<string, string> actions)
        {
            actions = new Dictionary<string, string>();
            if (resultEntities.Error != null)
            {
                return $"出现点儿问题{resultEntities.Error.Code}。Details: {resultEntities.Error.Message}";
            }

            if (resultEntities.Results == null || resultEntities.Results.Count == 0)
            {
                return "没有找到合适的答案，问其它东西试试！";
            }

            var builder = new StringBuilder();
            builder.Append($"**关于*{query}*的建议答案**: <br>");
            var answer = resultEntities.Results[0].Answer;
            if (answer.Length > DefaultAnswerLength)
            {
                answer = $"{answer.Substring(0, DefaultAnswerLength)}...   ([详情]({resultEntities.Results[0].Url}))";
            }

            builder.Append(
                $"**问题**：[{resultEntities.Results[0].Question}]({resultEntities.Results[0].Url})<br>**答案**：{answer}<br>");

            if (resultEntities.ResultCount > 1)
            {
                builder.Append("<br>**其它相关**:<br>");
                for (var index = 1; index < resultEntities.ResultCount; index++)
                {
                    builder.Append(
                        $"{index}. [{resultEntities.Results[index].Question}]({resultEntities.Results[index].Url})<br>");
                    actions[index.ToString()] = resultEntities.Results[index].Question;
                }
            }

            return builder.ToString();
        }

        private string ParseQuestion(string text)
        {
            return text;
        }
    }
}