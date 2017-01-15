// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchError.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the SearchError.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SearchAnswer.Core
{
    public class SearchError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}