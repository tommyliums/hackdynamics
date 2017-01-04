using System.Text.RegularExpressions;

namespace Common.Html2Markdown.Replacement
{
    internal class PatternReplacer : IReplacer
    {
        public string Pattern { get; set; }

        public string Replacement { get; set; }
        public string Replace(string html)
        {
            var regex = new Regex(Pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return regex.Replace(html, Replacement);
        }
    }
}
