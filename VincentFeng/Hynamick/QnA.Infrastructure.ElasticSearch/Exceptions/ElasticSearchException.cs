using System;

namespace Hynamick.QnA.Infrastructure.ElasticSearch.Exceptions
{
    public class ElasticSearchException : Exception
    {
        public ElasticSearchException(
            string searchUrl,
            string searchSource,
            string query,
            int searchResultCount,
            string localeName,
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            this.SearchUrl = searchUrl;
            this.SearchSource = searchSource;
            this.Query = query;
            this.SearchResultCount = searchResultCount;
            this.LocaleName = localeName;
        }

        public string SearchUrl
        {
            get;
            private set;
        }

        public string SearchSource
        {
            get;
            private set;
        }

        public string Query
        {
            get;
            private set;
        }

        public int SearchResultCount
        {
            get;
            private set;
        }

        public string LocaleName
        {
            get;
            private set;
        }
    }
}
