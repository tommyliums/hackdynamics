using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hynamick.SearchAnswer
{
    public class SearchResult
    {
        public string Question { get; set; }

        public string Answer { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public DateTime LastModified { get; set; }
    }
}
