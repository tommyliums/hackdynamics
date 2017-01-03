using System.Threading.Tasks;

namespace CrawlContract
{
    public interface ICrawler
    {
        Task<ResponseItem> CrawlAsync(RequestItem item);

        Task<ResponseItem[]> CrawlAsync(RequestItem[] items);
    }
}
