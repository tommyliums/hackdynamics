// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the IServiceSettings.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.Configurations
{
    public interface IServiceSettings
    {
        string ServiceUrl { get; set; }
        string SearchTemplatePath { get; set; }
        string SearchTransformFilePath { get; set; }
    }
}