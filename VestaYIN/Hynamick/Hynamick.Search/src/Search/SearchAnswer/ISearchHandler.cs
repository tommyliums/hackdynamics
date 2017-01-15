// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the ISearchHandler.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    using System.Threading.Tasks;

    public interface ISearchHandler
    {
        Task<SearchResponse> SearchAsync(string source, string locale, string query, int count);

        SearchRequest BuildSearchRequest(string source, string locale, string query, int count);

        Task<string> GetSourceResponseAsync(SearchRequest searchRequest);

        SearchResponse TransformSearchResponse(string sourceResponse);
    }
}