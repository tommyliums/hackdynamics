namespace Hynamick.Search.SearchAnswer
{
    using System.Threading.Tasks;

    public interface ISearchHandler
    {
        Task<SearchResponse> Search(string source, string locale, string query, int count);

        SearchRequest BuildSearchRequest(string source, string locale, string query, int count);

        Task<string> GetSourceResponse(SearchRequest searchRequest);

        SearchResponse TransformSearchResponse(string sourceResponse);
    }
}
