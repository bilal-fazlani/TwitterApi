using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinqToTwitter;
using Newtonsoft.Json;
using TwitterWebApi.Models;

namespace TwitterWebApi.ExternalServices.TwitterSearch
{
    public class InMemoryTwitterSearchService : ITwitterSearchService
    {
        public async Task<SearchResult> SearchAsync(string handle, int pageSize = 10, ulong? sinceId = null)
        {
            Search search = JsonConvert.DeserializeObject<Search>(File.ReadAllText("lastresponse.json"));

            List<Tweet> statuses = search.Statuses
                .Select(Mapper.Map<Tweet>).ToList();

            Console.WriteLine("/====================================================\\");
            Console.WriteLine("|------------------- FAKE RESPONSE -------------------|");
            Console.WriteLine("\\=====================================================/");

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