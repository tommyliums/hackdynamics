namespace Hynamick.QnA.Infrastructure.ElasticSearch.Models
{
    public class ElasticSearchResponse
    {
        public string Query
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

        public int Elapse
        {
            get;
            set;
        }

        public int SearchElapse
        {
            get;
            set;
        }

        public double MaxScore
        {
            get;
            set;
        }

        public ElasticSearchError Error
        {
            get;
            set;
        }

        public ElasticSearchResult[] Results
        {
            get;
            set;
        }
    }
}
