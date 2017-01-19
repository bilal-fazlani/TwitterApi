using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinqToTwitter;
using Newtonsoft.Json;
using TwitterWebApi.Exceptions;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services.TwitterSearch
{
    public class InMemoryTwitterSearchService : ITwitterSearchService
    {
        public async Task<SearchResult> SearchAsync(string handle, int pageSize = 10, ulong? sinceId = null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            Search search = JsonConvert.DeserializeObject<Search>(File.ReadAllText("fakeresponse.json"));

            List<Tweet> statuses = search.Statuses
                .Select(Mapper.Map<Tweet>).ToList();

            Console.WriteLine("FAKE RESPONSE");

            return new SearchResult
            {
                SinceId = sinceId,
                LastId = statuses.LastOrDefault()?.StatusID,
                PageSize = pageSize,
                Statuses = statuses
            };
        }
    }
}