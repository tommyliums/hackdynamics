using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAnswer.Core
{
    public interface ISearchHandler
    {
        Task<SearchResponse> Search(SearchRequest searchRequest);

        SearchRequest BuildSearchRequest(string source, string locale, string query, int count);

        Task<string> GetSourceResponse(SearchRequest searchRequest);

        SearchResponse TransformSearchResponse(string sourceResponse);
    }
}
