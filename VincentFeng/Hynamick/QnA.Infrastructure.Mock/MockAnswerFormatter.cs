using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;

namespace Hynamick.QnA.Infrastructure.Mock
{
    public class MockAnswerFormatter : IAnswerFormatter
    {
        public string Format(Answer answer)
        {
            return answer.Text;
        }
    }
}
