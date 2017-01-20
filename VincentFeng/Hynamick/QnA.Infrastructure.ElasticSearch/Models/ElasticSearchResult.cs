using System;

namespace Hynamick.QnA.Infrastructure.ElasticSearch.Models
{
    public class ElasticSearchResult
    {
        public string Question
        {
            get;
            set;
        }

        public string Answer
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public object Metas
        {
            get;
            set;
        }
    }
}
