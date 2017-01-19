using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using TwitterWebApi.Exceptions;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services.TwitterSearch
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
                          s.Count == pageSize
                    select s;

                if (sinceId.HasValue) query = query.Where(x => x.SinceID == sinceId.Value);

                Search search = await query.SingleOrDefaultAsync();

                if (search == null)
                    throw new NoDataException("No search results found");


                List<Tweet> statuses = search.Statuses
                    .Select(x => new Tweet
                    {
                        Text = x.Text,
                        Name = x.User?.Name,
                        StatusID = x.StatusID,
                        CreatedAt = x.CreatedAt,
                        ProfilePicUrl = x.User?.ProfileImageUrlHttps
                    }).ToList();

                return new SearchResult
                    {
                        SinceId = sinceId,
                        LastId = statuses.LastOrDefault()?.StatusID,
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