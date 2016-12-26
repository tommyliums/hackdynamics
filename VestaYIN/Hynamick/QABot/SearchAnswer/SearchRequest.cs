using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hynamick.SearchAnswer
{
    public class SearchRequest
    {
        public string Url { get; set; }

        public string Body { get; set; }

        public string Method { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
