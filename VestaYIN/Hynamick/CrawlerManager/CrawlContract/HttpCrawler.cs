using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrawlContract
{
    public class HttpCrawler : ICrawler
    {
        private HttpClient client;

        public HttpCrawler(IDictionary<string, string> headers = null)
        {
            this.client = new HttpClient();
            if (headers == null)
            {
                return;
            }

            AddHeaders(this.client.DefaultRequestHeaders, headers);
        }

        public async Task<ResponseItem[]> CrawlAsync(RequestItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items.Length == 0)
            {
                return new ResponseItem[0];
            }

            var responseItems = new List<ResponseItem>();
            foreach (var item in items)
            {
                ResponseItem responseItem;
                try
                {
                    RequestItem.EnsureItem(item);
                    responseItem = await this.InternalCrawlAsync(item);
                }
                catch (ArgumentNullException ex)
                {
                    responseItem = new ResponseItem(item)
                    {
                        Exception = ex
                    };
                }

                responseItems.Add(responseItem);
            }

            return responseItems.ToArray();
        }

        public async Task<ResponseItem> CrawlAsync(RequestItem item)
        {
            try
            {
                RequestItem.EnsureItem(item);
            }
            catch (ArgumentNullException ex)
            {
                return new ResponseItem(item)
                {
                    Exception = ex
                };
            }

            return await InternalCrawlAsync(item);
        }

        protected virtual async Task<ResponseItem> InternalCrawlAsync(RequestItem requestItem)
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod(requestItem.Method), requestItem.Url);
            AddHeaders(requestMessage.Headers, requestItem.Headers);
            if (requestItem.Body != null)
            {
                requestMessage.Content = new StringContent(requestItem.Body);
            }

            var responseItem = new ResponseItem(requestItem);
            try
            {
                var responseMessage = await this.client.SendAsync(requestMessage);
                responseItem.StatusCode = responseMessage.StatusCode;
                responseItem.ResponseUrl = responseMessage.RequestMessage.RequestUri.AbsoluteUri;
                responseItem.Body = await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                responseItem.Exception = ex;
            }

            return responseItem;
        }

        private void AddHeaders(HttpRequestHeaders requestHeaders, IDictionary<string, string> headers)
        {
            if (requestHeaders != null && headers != null)
            {
                foreach (var header in headers)
                {
                    requestHeaders.Add(header.Key, header.Value);
                }
            }
        }
    }
}
