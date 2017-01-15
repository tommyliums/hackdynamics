// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResponse.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the SearchResponse.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    using System.Collections.Generic;

    public class SearchResponse
    {
        public string Query { get; set; }

        public int TotalCount { get; set; }

        public int ResultCount { get; set; }

        public long Elapse { get; set; }

        public long SearchElaspe { get; set; }

        public float MaxScore { get; set; }

        public SearchError Error { get; set; }

        public List<SearchResult> Results { get; set; }
    }
}