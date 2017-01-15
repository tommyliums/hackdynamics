// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocaleHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the LocaleHelper.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CrawlerManager
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public static class LocaleHelper
    {
        private static readonly Dictionary<string, string> LocaleMapping;

        static LocaleHelper()
        {
            var localeMappingPath = ConfigurationManager.AppSettings["Hw.Faq.LocaleMapping"];
            LocaleMapping = InitializeLocaleMapping(localeMappingPath);
        }

        private static Dictionary<string, string> InitializeLocaleMapping(string localeMappingPath)
        {
            var localeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = File.ReadAllLines(localeMappingPath);
            foreach (var items in lines.Select(line => line.Split('\t')))
            {
                localeMapping[items[0]] = items[1];
            }

            return localeMapping;
        }

        public static string GetLocaleSuffix(string locale)
        {
            string localeSuffix;
            if (!LocaleMapping.TryGetValue(locale, out localeSuffix))
            {
                localeSuffix = "chinese_s";
            }

            return localeSuffix;
        }
    }
}