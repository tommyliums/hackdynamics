// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the Constants.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.SearchAnswer.Tests
{
    using System;
    using System.IO;
    using System.Net.Http;

    using Hynamick.Search.SearchAnswer;

    using Microsoft.Azure;

    public static class Constants
    {
        public static readonly HttpClient Client = new HttpClient();

        public static readonly string ServiceUrl = CloudConfigurationManager.GetSetting("SearchUrl");

        public static readonly string LocaleMappingPath = CloudConfigurationManager.GetSetting("LocaleMappingPath");

        public static readonly string SearchTemplatePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
            CloudConfigurationManager.GetSetting("SearchTemplateFile"));

        public static readonly string TransformFilePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
            CloudConfigurationManager.GetSetting("TransformFilePath"));

        public static ISearchHandler GetSearchHandler()
        {
            return new SearchHandler(Client, ServiceUrl, LocaleMappingPath, SearchTemplatePath, TransformFilePath);
        }
    }
}