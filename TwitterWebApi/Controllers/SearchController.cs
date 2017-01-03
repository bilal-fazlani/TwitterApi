using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TwitterWebApi.Controllers
{
    public class SearchController : Controller
    {
        private readonly TwitterContext _twitterCtx;

        public SearchController(IConfigurationRoot configuration)
        {
            IConfigurationRoot configuration1 = configuration;
            SingleUserAuthorizer auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = configuration1["consumerKey"],
                    ConsumerSecret = configuration1["consumerSecret"],
                    AccessToken = configuration1["accessToken"],
                    AccessTokenSecret = configuration1["accessTokenSecret"]
                }
            };

            _twitterCtx = new TwitterContext(auth);
        }

        [HttpGet]
        [Route("api/search/{handle}")]
        public async Task<IActionResult> Get(string handle, int pageSize =10, ulong? sinceId = null)
        {
            if (string.IsNullOrWhiteSpace(handle))
                return BadRequest("handle is empty");

            try
            {
                IQueryable<Search> query = from s in _twitterCtx.Search
                where s.Type == SearchType.Search && s.Query == $"\"#{handle}\"" &&
                      s.Count == pageSize
                select s;

                if (sinceId.HasValue) query = query.Where(x => x.SinceID == sinceId.Value);

                Search search = await query.SingleOrDefaultAsync();

                if (search == null)
                    return NotFound("No search results found");


                var statuses = search.Statuses
                    .Select(x => new
                    {
                        x.Text,
                        x.User?.Name,
                        x.StatusID,
                        x.CreatedAt
                    }).ToList();

                return Json(
                            new
                            {
                                SinceId = sinceId,
                                LastId = statuses.LastOrDefault()?.StatusID,
                                PageSize = pageSize,
                                Statuses = statuses
                            });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}