using System.Threading.Tasks;
using TwitterWebApi.Models;

namespace TwitterWebApi.ExternalServices.TwitterSearch
{
    public interface ITwitterSearchService
    {
        Task<SearchResult> SearchAsync(string handle, int pageSize =10, ulong? sinceId = null);
    }
}