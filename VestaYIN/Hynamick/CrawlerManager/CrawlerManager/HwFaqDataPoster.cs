using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CrawlContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrawlerManager
{
    class HwFaqDataPoster
    {
        public static string ServiceUrl = ConfigurationManager.AppSettings["ElasticSearch.ServiceUrl"];
        public static string UrlPattern = ConfigurationManager.AppSettings["Hw.Faq.UrlPattern"];
        public static string IndexName = ConfigurationManager.AppSettings["Hw.Faq.IndexName"].ToLowerInvariant();

        private readonly HttpCrawler crawler = new HttpCrawler();

        public async Task<int> ProcessDataAsync(string dataFolder)
        {
            var files = Directory.GetFiles(dataFolder);
            var builder = new StringBuilder();
            var putUrl = ServiceUrl + $"/{IndexName}/_bulk";
            var docCount = 0;
            foreach (var file in files)
            {
                try
                {
                    var jsonContent = JToken.Parse(File.ReadAllText(file));
                    var jObject = new JObject();
                    var locale = jsonContent["languageName"];
                    var localeSuffix = LocaleHelper.GetLocaleSuffix((string)locale);
                    var faqId = (int)jsonContent["faqId"];
                    jObject["FaqId"] = faqId;
                    jObject[$"Q_{localeSuffix}"] = jsonContent["title"];
                    jObject[$"A_{localeSuffix}"] = jsonContent["answer"];
                    jObject[$"Type_{localeSuffix}"] = jsonContent["faqType"];
                    jObject[$"Keyword_{localeSuffix}"] = MakeArray((string)jsonContent["keywords"]);
                    jObject["Language"] = locale;
                    jObject["LastModified"] = new DateTime(1970, 1, 1).AddMilliseconds((long)jsonContent["postDate"]);
                    jObject["Url"] = string.Format(UrlPattern, faqId);
                    builder.AppendFormat(
                        "{{\"index\":{{\"_index\":\"{0}\",\"_type\":\"docs\",\"_id\":\"{1}\"}} }}\r\n", IndexName, faqId);
                    builder.AppendLine(JsonConvert.SerializeObject(jObject));
                    docCount++;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
                if (docCount % 1000 != 0 && builder.Length <= 2 * 1000 * 1000) continue;

                var requestItem = new RequestItem(putUrl, "PUT", builder.ToString());
                var responseItem = await this.crawler.CrawlAsync(requestItem);
                Trace.TraceInformation($"Put {docCount}: {responseItem}");
                builder.Clear();
            }

            if (builder.Length > 0)
            {
                var requestItem = new RequestItem(putUrl, "PUT", builder.ToString());
                var responseItem = await this.crawler.CrawlAsync(requestItem);
                Trace.TraceInformation($"Put {docCount}: {responseItem}");
            }

            return docCount;
        }

        private static JArray MakeArray(string keyword)
        {
            var items = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (items == null)
            {
                return null;
            }

            var jsonArray = new JArray();
            foreach (var item in items)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
        }
    }
}
