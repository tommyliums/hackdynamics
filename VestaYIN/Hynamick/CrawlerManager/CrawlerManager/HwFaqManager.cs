using CrawlContract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerManager
{
    class HwFaqManager
    {
        public static string UrlPattern = ConfigurationManager.AppSettings["Hw.Faq.UrlPattern"];
        public static int MinFaqId = int.Parse(ConfigurationManager.AppSettings["Hw.Faq.MinFaqId"]);
        public static int MaxFaqId = int.Parse(ConfigurationManager.AppSettings["Hw.Faq.MaxFaqId"]);
        public static string CacheFolder = ConfigurationManager.AppSettings["Hw.Faq.CacheFolder"];

        private HttpCrawler crawler;

        public async Task<int> CrawlItems()
        {
            int successCount = 0, processedCount = 0, alreadyExistCount = 0, emptyCount = 0, errorCount = 0;

            if (this.crawler == null)
            {
                this.crawler = new HttpCrawler();
            }

            if (!Directory.Exists(CacheFolder))
            {
                Directory.CreateDirectory(CacheFolder);
            }

            for (var faqId = MaxFaqId; faqId >= MinFaqId; faqId--)
            {
                var path = Path.Combine(CacheFolder, $"{faqId}.json");
                if (File.Exists(path))
                {
                    alreadyExistCount++;
                    continue;
                }

                var url = string.Format(UrlPattern, faqId);
                var requestItem = new RequestItem(url);
                var responseItem = await this.crawler.CrawlAsync(requestItem);
                if (responseItem.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonFaq = JToken.Parse(responseItem.Body);
                    var respondFaqId = (string)jsonFaq["faqId"];
                    if (respondFaqId != null)
                    {
                        File.WriteAllText(path, responseItem.Body);
                        processedCount++;
                    }
                    else
                    {
                        emptyCount++;
                    }

                }
                else
                {
                    AppendErrorFaq(faqId, responseItem);
                    errorCount++;
                }

                if (faqId % 100 == 0)
                {
                    Trace.TraceInformation($"Processed: {processedCount}, exists: {alreadyExistCount}, empty: {emptyCount}, error: {errorCount}");
                }
            }

            return successCount;
        }

        private void AppendErrorFaq(int faqId, ResponseItem responseItem)
        {
            var path = Path.Combine(CacheFolder, "Error.txt");
            using (var writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine("============================");
                writer.WriteLine($"FaqId: {faqId}, StatusCode: {responseItem.StatusCode}, Url: {responseItem.Requestitem.Url}");
                if (responseItem.Exception != null)
                {
                    writer.WriteLine($"Exception Type: {responseItem.Exception}, Message: {responseItem.Exception.Message}");
                }
            }
        }
    }
}
