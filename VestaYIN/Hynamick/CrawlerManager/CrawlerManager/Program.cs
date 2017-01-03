using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new HwFaqManager();
            var successCount = manager.CrawlItems().Result;
            Console.WriteLine(successCount);
        }
    }
}
