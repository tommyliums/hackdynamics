using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using System.Text;

namespace Hynamick.QnA.Infrastructure.AnswerFormatters
{
    public class SimpleAnswerFormatter : IAnswerFormatter
    {
        public string Format(Answer answer)
        {
            StringBuilder formattedAnswer = new StringBuilder();

            formattedAnswer.AppendLine($"Relevance: {answer.Relevance}");
            formattedAnswer.AppendLine($"Answer: {answer.Text}");

            return formattedAnswer.ToString();
        }
    }
}
