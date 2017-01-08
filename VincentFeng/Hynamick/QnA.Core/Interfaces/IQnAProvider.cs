using Hynamick.QnA.Core.DomainModels;

namespace Hynamick.QnA.Core.Interfaces
{
    public interface IQnAProvider
    {
        Answer Answer(Question question);
    }
}
