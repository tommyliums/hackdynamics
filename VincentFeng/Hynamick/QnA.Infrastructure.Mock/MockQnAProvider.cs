using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using System.Collections.Generic;

namespace Hynamick.QnA.Infrastructure.Mock
{
    public class MockQnAProvider : IQnAProvider
    {
        public Answer Answer(Question question)
        {
            Answer answer = new Answer();

            if (this.qnas.ContainsKey(question.Text))
            {
                answer.Text = this.qnas[question.Text];
            }
            else
            {
                answer.Text = UnknownAnswer;
            }

            return answer;
        }

        private Dictionary<string, string> qnas = new Dictionary<string, string>()
        {
            { "Hi", "Hello" },
            { "Hello", "Hello" },
        };

        private const string UnknownAnswer = "Unknown";
    }
}
