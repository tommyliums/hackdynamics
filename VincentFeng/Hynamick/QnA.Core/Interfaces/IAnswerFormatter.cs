using Hynamick.QnA.Core.DomainModels;

namespace Hynamick.QnA.Core.Interfaces
{
    public interface IAnswerFormatter
    {
        string Format(AnswerCollection answers);
    }
}
