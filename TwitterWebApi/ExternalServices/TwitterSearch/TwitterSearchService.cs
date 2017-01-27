using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TwitterWebApi.Exceptions;
using TwitterWebApi.Models;

namespace TwitterWebApi.ExternalServices.TwitterSearch
{
    public class TwitterSearchService : ITwitterSearchService
    {
        private readonly TwitterContext _twitterCtx;

        public TwitterSearchService(IConfiguration configuration)
        {
            SingleUserAuthorizer auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = configuration["consumerKey"],
                    ConsumerSecret = configuration["consumerSecret"],
                    AccessToken = configuration["accessToken"],
                    AccessTokenSecret = configuration["accessTokenSecret"]
                }
            };

            _twitterCtx = new TwitterContext(auth);
        }

        public async Task<SearchResult> SearchAsync(string handle, int pageSize =10, ulong? sinceId = null)
        {
            if (string.IsNullOrWhiteSpace(handle))
                throw new ValidationException("handle is empty");

            try
            {
                IQueryable<Search> query = from s in _twitterCtx.Search
                    where s.Type == SearchType.Search && s.Query == $"\"#{handle}\"" &&
                          s.Count == pageSize && s.ResultType == ResultType.Mixed
                    select s;

                if (sinceId.HasValue) query = query.Where(x => x.SinceID == sinceId.Value);

                Search search = await query.SingleOrDefaultAsync();

                if (search == null)
                    throw new NoDataException("No search results found");

                File.WriteAllText("lastresponse.json", JsonConvert.SerializeObject(search, Formatting.Indented));

                List<Tweet> statuses = search.Statuses
                    .Select(Mapper.Map<Tweet>).ToList();

                return new SearchResult
                    {
                        SinceId = sinceId,
                        LastId = statuses.LastOrDefault()?.StatusId,
                        PageSize = pageSize,
                        Statuses = statuses
                    };
            }
            catch (Exception ex)
            {
                throw new ServerException("Server error", ex);
            }
        }
    }
}