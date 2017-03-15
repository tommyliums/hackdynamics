using Hynamick.QnA.Core.DomainModels;

namespace Hynamick.QnA.Core.Interfaces
{
    public interface IQnAProvider
    {
        AnswerCollection Answer(Question question);
    }
}
