// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the ServiceSettings.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.Configurations
{
    public class ServiceSettings : IServiceSettings
    {
        public string ServiceUrl { get; set; }
        public string SearchTemplatePath { get; set; }
        public string SearchTransformFilePath { get; set; }
    }
}