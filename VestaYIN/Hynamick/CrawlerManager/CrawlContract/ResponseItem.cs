using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlContract
{
    public class ResponseItem
    {
        public ResponseItem(RequestItem requestItem)
        {
            RequestItem.EnsureItem(requestItem);

            this.Requestitem = requestItem;
        }

        public RequestItem Requestitem { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ResponseUrl { get; set; }

        public string Body { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            return $"{this.StatusCode}: {this.ResponseUrl}";
        }
    }
}
