// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the SearchResult.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SearchAnswer.Core
{
    using System;

    public class SearchResult
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
    }
}