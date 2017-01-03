using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbotX.Core;
using AbotX.Crawler;
using AbotX.Poco;

using log4net.Config;
using Abot.Crawler;
using Abot.Poco;
using System.Net;
using System.IO;
using BotCrawler.Core;

namespace BotCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var urisToCrawl = GetSiteToCrawl(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"FAQ\CrawlUrls.txt"));
            var crawlRuleContent = GetCrawlRuleFileContent(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"FAQ\CrawlRules.txt"));
            var decisionMaker = new CrawlDecisionMakerWithCrawlRules(crawlRuleContent);

            XmlConfigurator.Configure();

            var config = AbotXConfigurationSectionHandler.LoadFromXml().Convert();
            config.IsJavascriptRenderingEnabled = true;
            config.JavascriptRenderingWaitTimeInMilliseconds = 3000;
            config.MaxConcurrentSiteCrawls = 1;
            config.MaxConcurrentThreads = 2;

            var impls = new ImplementationOverride(config);
            impls.CrawlDecisionMaker = decisionMaker;
            var crawler = new CrawlerX(config, impls);

            crawler.PageCrawlStarting += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompleted += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowed += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowed += crawler_PageLinksCrawlDisallowed;

            foreach (var uriToCrawl in urisToCrawl)
            {
                var result = crawler.Crawl(uriToCrawl);
            }

            Console.Read();
        }

        /// <summary>
        /// The get site to crawl.
        /// </summary>
        /// <param name="userInput">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IList</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static IList<Uri> GetSiteToCrawl(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ApplicationException("Url list file to crawl is as a required parameter");
            }

            if (!File.Exists(userInput))
            {
                throw new ApplicationException("Url list file can not be found");
            }

            var lines = File.ReadAllLines(userInput);

            return lines.Select(line => new Uri(line)).ToList();
        }

        /// <summary>
        /// The get crawl rule file content.
        /// </summary>
        /// <param name="userInput">
        /// The user Input.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static byte[] GetCrawlRuleFileContent(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ApplicationException("Crawl rule file path is as a required parameter");
            }

            if (!File.Exists(userInput))
            {
                throw new ApplicationException("Crawl rule file can not be found");
            }

            return File.ReadAllBytes(userInput);
        }

        static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }

        static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
