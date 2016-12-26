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
    public class QaDialog : IDialog<string>
    {
        private static readonly SearchAnswer.SearchHandler handler = new SearchAnswer.SearchHandler();

        private static readonly int DefaultAnswerCount = int.Parse(ConfigurationManager.AppSettings["DefaultAnswerCount"]);

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var queryString = this.ParseQuestion(message.Text);
            var resultEntities = await handler.SearchAsync(queryString, DefaultAnswerCount);
            string answerText = this.BuildAnswer(resultEntities);
            await context.PostAsync(answerText);
            context.Wait(MessageReceivedAsync);
        }

        private string BuildAnswer(SearchResponse resultEntities)
        {
            if (resultEntities.Error != null)
            {
                return $"出现点儿问题{resultEntities.Error.Code}。Details: {resultEntities.Error.Message}";
            }

            if (resultEntities.Results == null || resultEntities.Results.Count == 0)
            {
                return "没有找到合适的答案，请换其他问题试试！";
            }

            var builder = new StringBuilder();
            builder.Append("找到的问题：<br>");
            builder.Append(resultEntities.Results[0].Question).Append("<br>");
            builder.Append(resultEntities.Results[0].Answer).Append("<br>");
            if (resultEntities.Results.Count > 1)
            {
                builder.Append("<br><br>您可能感兴趣：<br>");
                for (var index = 1; index < resultEntities.Results.Count; index++)
                {
                    builder.Append($"{resultEntities.Results[index].Question}<br>");
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