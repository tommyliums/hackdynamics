using Common.Html2Markdown;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerManager
{
    class Program
    {
        static void Main(string[] args)
        {
            HwFaqPostData();
        }

        private static void HwFaqPostData()
        {
            const string dataFolder = @"D:\Code";
            var poster = new HwFaqDataPoster();
            var folders = Directory.EnumerateDirectories(dataFolder);
            foreach (var folder in folders)
            {
                var docCount = poster.ProcessDataAsync(folder).Result;
                Console.WriteLine(docCount);
            }
        }

        private static void HwFaqCrawl()
        {
            var manager = new HwFaqManager();
            var successCount = manager.CrawlItems().Result;
            Console.WriteLine(successCount);
        }

        private static void HwFaqHtmlAnswer2MarkDown()
        {
            var converter = new Converter();

            const string folder = @"D:\Git\hackdynamics\VestaYIN\Hynamick\Hw.Faq";
            var directories = Directory.EnumerateDirectories(folder);
            int processed = 0, noAnswer = 0, noChange = 0;
            foreach (var directory in directories)
            {
                var files = Directory.EnumerateFiles(directory);

                foreach (var file in files)
                {
                    processed++;
                    if (processed % 100 == 0)
                    {
                        Trace.TraceInformation($"Processed: {processed}, No Answer: {noAnswer}, No Change: {noChange}");
                    }

                    var fileContent = File.ReadAllText(file);
                    var jToken = JToken.Parse(fileContent);
                    var answer = (string)jToken["answer"];
                    if (string.IsNullOrWhiteSpace(answer))
                    {
                        noAnswer++;
                        continue;
                    }

                    var markdownAnswer = converter.Convert(answer);
                    jToken["answer"] = markdownAnswer;
                    if (answer.Equals(markdownAnswer))
                    {
                        noChange++;
                        continue;
                    }

                    File.WriteAllText(file, JsonConvert.SerializeObject(jToken, Formatting.Indented));
                }
            }
        }
    }
}
