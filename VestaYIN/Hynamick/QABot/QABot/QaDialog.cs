using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Text;
using Hynamick.SearchAnswer;

namespace Hynamick.QaBot
{
    [Serializable]
    public class QaDialog : IDialog<object>
    {
        public static readonly SearchAnswer.SearchHandler Handler = new SearchAnswer.SearchHandler();

        public static readonly int DefaultAnswerCount = int.Parse(ConfigurationManager.AppSettings["DefaultAnswerCount"]);


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var queryString = this.ParseQuestion(message.Text);
            var resultEntities = await Handler.SearchAsync(queryString, DefaultAnswerCount);
            IDictionary<string, string> actions;
            string answerText = BuildText(queryString, resultEntities, out actions);

            await context.PostAsync(answerText, "zh-cn");
            context.Wait(MessageReceivedAsync);
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

        public static string BuildText(string query, SearchResponse resultEntities, out IDictionary<string, string> actions)
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
            builder.Append($"**问题**：{resultEntities.Results[0].Question}<br>**答案**：{resultEntities.Results[0].Answer}");

            if (resultEntities.ResultCount > 1)
            {
                builder.Append("<br>**其它相关**:<br>");
                for (var index = 1; index < resultEntities.ResultCount; index++)
                {
                    builder.Append($"{index}. {resultEntities.Results[index].Question}<br>");
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