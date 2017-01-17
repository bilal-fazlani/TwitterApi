using System.Threading.Tasks;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services.TwitterSearch
{
    public interface ITwitterSearchService
    {
        Task<SearchResult> SearchAsync(string handle, int pageSize =10, ulong? sinceId = null);
    }
}