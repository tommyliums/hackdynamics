using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAnswer.Core
{
    using System.IO;
    using System.Net.Http;

    public class SearchHandler : ISearchHandler
    {
        public readonly string SearchUrl;

        public readonly HttpClient Client;
        public readonly string SearchTemplate;

        public SearchHandler(HttpClient client, string searchUrl, string searchTemplatePath)
        {
            this.Client = client;
            this.SearchUrl = searchUrl;
            this.SearchTemplate = File.ReadAllText(searchTemplatePath);
        }

        public virtual Task<SearchResponse> Search(SearchRequest request)
        {

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

        public virtual async Task<string> GetSourceResponse(SearchRequest searchRequest)
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
            throw new NotImplementedException();
        }
    }
}
