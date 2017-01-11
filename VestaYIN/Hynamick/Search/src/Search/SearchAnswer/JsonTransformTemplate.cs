// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTransformTemplate.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the JsonTransformTemplate.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    /// <summary>
    ///     Store Json template data
    /// </summary>
    public class JsonTransformTemplate
    {
        /// <summary>
        ///     template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     template content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Name: {this.Name}, Content: {this.Content.Substring(0, 50)}";
        }
    }
}