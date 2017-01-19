using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services.TwitterSearch
{
    public class InMemoryTwitterSearchService : ITwitterSearchService
    {
        public async Task<SearchResult> SearchAsync(string handle, int pageSize = 10, ulong? sinceId = null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return new SearchResult
            {
                Statuses = new List<Tweet>
                {
                    new Tweet
                    {
                        Text = "random text",
                        Name = "randome name"
                    }
                },
                SinceId = sinceId,
                LastId = 0,
                PageSize = pageSize
            };
        }
    }
}