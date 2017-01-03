namespace BotCrawler.Core
{
    using Abot.Core;
    using Abot.Poco;
    using CrawlRules;

    /// <summary>
    /// The crawl decision maker with crawl rules.
    /// </summary>
    public class CrawlDecisionMakerWithCrawlRules : CrawlDecisionMaker
    {
        /// <summary>
        /// The crawl rules.
        /// </summary>
        private readonly BaseCrawlRules crawlRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrawlDecisionMakerWithCrawlRules"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CrawlDecisionMakerWithCrawlRules(byte[] content)
        {
            var parser = new SimpleCrawlRuleRulesParser();
            this.crawlRules = parser.ParseContent(content);
        }

        /// <summary>
        /// The should crawl page.
        /// </summary>
        /// <param name="pageToCrawl">
        /// The page to crawl.
        /// </param>
        /// <param name="crawlContext">
        /// The crawl context.
        /// </param>
        /// <returns>
        /// The <see cref="CrawlDecision"/>.
        /// </returns>
        public override CrawlDecision ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
        {
            var crawlDecision = base.ShouldCrawlPage(pageToCrawl, crawlContext);

            if (!crawlDecision.Allow)
            {
                return crawlDecision;
            }

            string reason;
            if (!this.crawlRules.IsAllowed(pageToCrawl.Uri.ToString(), out reason))
            {
                return new CrawlDecision { Allow = false, Reason = reason };
            }

            return new CrawlDecision { Allow = true };
        }
    }
}
