// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTransformAction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the JsonTransformAction.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    /// <summary>
    /// Support ApplyTemplate and SingleValue two types action.
    /// </summary>
    public class JsonTransformAction
    {
        /// <summary>
        /// Action type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// template name, only used when type is ApplyTemplate.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// the root path where action be executed.
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// Gets or sets whether reference type disable double quote surround. For reference type like sting, if set to true, no
        /// quote will added.
        /// </summary>
        public bool NoQuote { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return
                $"Type: {this.Type}, TemplateName: {this.TemplateName}, Select: {this.Select}, NoQuote: {this.NoQuote}";
        }
    }
}