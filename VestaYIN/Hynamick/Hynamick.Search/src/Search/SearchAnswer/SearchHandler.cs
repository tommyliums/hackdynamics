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
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    public class SearchHandler : ISearchHandler
    {
        public readonly HttpClient Client;
        public readonly string SearchTemplate;
        public readonly string SearchUrl;
        public readonly JsonTransform Transform;

        public SearchHandler(HttpClient client, string searchUrl, string searchTemplatePath, string transformFilePath)
        {
            this.Client = client;
            this.SearchUrl = searchUrl;
            this.SearchTemplate = File.ReadAllText(searchTemplatePath);
            this.Transform = new JsonTransform(File.ReadAllText(transformFilePath));
        }

        public virtual async Task<SearchResponse> SearchAsync(string source, string locale, string query, int count)
        {
            var watcher = Stopwatch.StartNew();

            SearchResponse searchResponse;
            try
            {
                var searchRequest = this.BuildSearchRequest(source, locale, query, count);
                var sourceResponse = await this.GetSourceResponseAsync(searchRequest);
                searchResponse = this.TransformSearchResponse(sourceResponse);
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
            searchResponse.ProcessElapse = watcher.ElapsedMilliseconds;
            return searchResponse;
        }

        public virtual SearchRequest BuildSearchRequest(string source, string locale, string query, int count)
        {
            var body = this.SearchTemplate.Replace("##query_string##", query);
            body = body.Replace("##result_count##", count.ToString());

            return new SearchRequest
            {
                Url = this.SearchUrl,
                Body = body,
                Method = "POST"
            };
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

        public SearchResponse TransformSearchResponse(string sourceResponse)
        {
            var jToken = JToken.Parse(sourceResponse);
            return this.Transform.Transform<SearchResponse>(jToken);
        }
    }
}