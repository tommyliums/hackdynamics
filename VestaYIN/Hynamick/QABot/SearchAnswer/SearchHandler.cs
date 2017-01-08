using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hynamick.SearchAnswer
{
    public class SearchHandler
    {
        public static readonly string SearchUrl = ConfigurationManager.AppSettings["SearchUrl"];

        public static readonly string SearchTemplate = LoadSearchTemplate();
        public static readonly HttpClient Client = InitializeHttpClient();
        public static readonly JsonTransform Transform = InitializeJsonTransform();

        private static JsonTransform InitializeJsonTransform()
        {
            var transformFilePath = ConfigurationManager.AppSettings["TransformFilePath"];
            var content = File.ReadAllText(Path.Combine(Utilities.CurrentDirectory, transformFilePath));
            var transform = new JsonTransform(content);
            return transform;
        }

        private static HttpClient InitializeHttpClient()
        {
            return new HttpClient();
        }

        private static string LoadSearchTemplate()
        {
            var templatePath = ConfigurationManager.AppSettings["SearchTemplateFile"];
            return File.ReadAllText(Path.Combine(Utilities.CurrentDirectory, templatePath));
        }

        public async Task<SearchResponse> SearchAsync(string query, int count)
        {
            var searchQuery = BuildSearchQuery(query, count);
            SearchResponse response;
            try
            {
                var responseMessage = await GetResponseMessage(searchQuery);
                response = ParseResponse(responseMessage);
            }
            catch (Exception ex)
            {
                response = new SearchResponse
                {
                    Query = query,
                    Error = new SearchError
                    {
                        Code = 20001,
                        Message = ex.ToString()
                    }
                };
            }

            response.Query = query;
            response.ResultCount = response.Results.Count;
            return response;
        }

        private SearchRequest BuildSearchQuery(string query, int count)
        {
            var body = SearchTemplate.Replace("##query_string##", query);
            body = body.Replace("##result_count##", count.ToString());

            return new SearchRequest
            {
                Url = SearchUrl,
                Body = body,
                Method = "POST"
            };
        }

        private async Task<string> GetResponseMessage(SearchRequest searchQuery)
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod(searchQuery.Method), searchQuery.Url)
            {
                Content = new StringContent(searchQuery.Body)
            };

            if (searchQuery.Headers != null && searchQuery.Headers.Count > 0)
            {
                foreach (var header in searchQuery.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await Client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private SearchResponse ParseResponse(string responseContent)
        {
            var jToken = JToken.Parse(responseContent);
            return Transform.Transform<SearchResponse>(jToken);
        }
    }
}