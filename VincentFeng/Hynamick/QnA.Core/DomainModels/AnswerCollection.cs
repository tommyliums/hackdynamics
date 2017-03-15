using System.Collections.Generic;

namespace Hynamick.QnA.Core.DomainModels
{
    public class AnswerCollection
    {
        public IEnumerable<Answer> Answers
        {
            get;
            set;
        }

        public int TotalCount
        {
            get;
            set;
        }

        public int ResultCount
        {
            get;
            set;
        }
    }
}
