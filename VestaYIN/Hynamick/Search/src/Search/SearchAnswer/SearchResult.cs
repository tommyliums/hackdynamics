// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the SearchResult.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    using System.Collections.Generic;

    public class SearchResult
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Metas;
    }
}