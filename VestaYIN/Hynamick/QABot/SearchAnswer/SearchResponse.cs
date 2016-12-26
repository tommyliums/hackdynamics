using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hynamick.SearchAnswer
{
    public class SearchResponse
    {
        public string Query { get; set; }

        public int TotalCount { get; set; }

        public int ResultCount { get; set; }

        public long ProcessElapse { get; set; }

        public long SearchElaspe { get; set; }

        public SearchError Error { get; set; }

        public List<SearchResult> Results { get; set; }
    }
}
