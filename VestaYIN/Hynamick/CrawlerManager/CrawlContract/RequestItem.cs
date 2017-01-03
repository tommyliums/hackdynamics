using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CrawlContract
{
    public class RequestItem
    {
        public const string DefaultMethod = "GET";

        public RequestItem(string url)
            : this(url, DefaultMethod, null)
        {
        }

        public RequestItem(string url, string method, string body)
        {
            EnsureUrl(url);
            this.Url = url;
            this.Method = EnsureMethod(method);
            this.Body = body;
        }

        public string Method { get; set; }

        public string Url { get; set; }

        public string Body { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public override string ToString()
        {
            return $"{this.Method}: {this.Url}";
        }

        public static void EnsureUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
        }

        public static string EnsureMethod(string method)
        {
            if (!string.Equals("GET", method, StringComparison.OrdinalIgnoreCase)
                && !string.Equals("POST", method, StringComparison.OrdinalIgnoreCase)
                && !string.Equals("PUT", method, StringComparison.OrdinalIgnoreCase))
            {
                return DefaultMethod;
            }

            return method;
        }

        public static void EnsureItem(RequestItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            EnsureUrl(item.Url);

            item.Method = EnsureMethod(item.Method);
        }
    }
}
