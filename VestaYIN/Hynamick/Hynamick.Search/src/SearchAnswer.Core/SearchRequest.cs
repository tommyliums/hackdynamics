// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchRequest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the SearchRequest.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SearchAnswer.Core
{
    using System.Collections.Generic;

    public class SearchRequest
    {
        public string Url { get; set; }
        public string Body { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}