// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the SearchHandler.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    public class SearchHandler : ISearchHandler
    {
        public readonly HttpClient Client;
        public readonly string SearchTemplate;
        public readonly string SearchUrl;
        public readonly Dictionary<string, string> LocaleMapping;
        public readonly Dictionary<string, JsonTransform> Transforms;

        public SearchHandler(HttpClient client, string searchUrl, string localeMappingPath, string searchTemplatePath, string transformFilePath)
        {
            this.Client = client;
            this.SearchUrl = searchUrl;
            this.LocaleMapping = InitializeLocaleMapping(localeMappingPath);
            this.SearchTemplate = File.ReadAllText(searchTemplatePath);
            this.Transforms = new Dictionary<string, JsonTransform>();
            var templateFileContent = File.ReadAllText(transformFilePath);
            foreach (var locale in this.LocaleMapping.Keys)
            {
                this.Transforms[locale] = new JsonTransform(templateFileContent, this.LocaleMapping[locale]);
            }
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

        public virtual async Task<SearchResponse> SearchAsync(string source, string locale, string query, int count)
        {
            var watcher = Stopwatch.StartNew();

            SearchResponse searchResponse;
            try
            {
                var searchRequest = this.BuildSearchRequest(source, locale, query, count);
                var sourceResponse = await this.GetSourceResponseAsync(searchRequest);
                searchResponse = this.TransformSearchResponse(sourceResponse, locale);
                searchResponse.ResultCount = searchResponse.Results.Count;
                searchResponse.Query = query;
            }
            catch (Exception ex)
            {
                searchResponse = new SearchResponse
                {
                    Query = query,
                    TotalCount = -1,
                    ResultCount = -1,
                    SearchElaspe = -1,
                    MaxScore = -1,
                    Error = new SearchError
                    {
                        Code = 20001,
                        Message = ex.ToString()
                    }
                };
            }

            watcher.Stop();
            searchResponse.Elapse = watcher.ElapsedMilliseconds;
            return searchResponse;
        }

        public virtual SearchRequest BuildSearchRequest(string source, string locale, string query, int count)
        {
            var fieldSuffix = this.GetFieldSuffix(locale);
            var body = this.SearchTemplate.Replace("##query_string##", query);
            body = body.Replace("##result_count##", count.ToString());
            body = body.Replace("##field_suffix##", fieldSuffix);

            return new SearchRequest
            {
                Url = string.Format(this.SearchUrl, source, locale),
                Body = body,
                Method = "POST"
            };
        }

        private string GetFieldSuffix(string locale)
        {
            string localeSuffix;
            if (!this.LocaleMapping.TryGetValue(locale, out localeSuffix))
            {
                localeSuffix = "chinese_s";
            }

            return localeSuffix;
        }

        public virtual async Task<string> GetSourceResponseAsync(SearchRequest searchRequest)
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod(searchRequest.Method), searchRequest.Url)
            {
                Content = new StringContent(searchRequest.Body)
            };

            if (searchRequest.Headers != null && searchRequest.Headers.Count > 0)
            {
                foreach (var header in searchRequest.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await this.Client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public SearchResponse TransformSearchResponse(string sourceResponse, string locale)
        {
            var jToken = JToken.Parse(sourceResponse);
            return this.Transforms[locale].Transform<SearchResponse>(jToken);
        }
    }
}