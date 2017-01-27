using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterWebApi.Exceptions;
using TwitterWebApi.ExternalServices.TwitterSearch;
using TwitterWebApi.Models;

namespace TwitterWebApi.Controllers
{
    public class TwitterSearchController : Controller
    {
        private readonly ITwitterSearchService _twitterSearchService;

        public TwitterSearchController(ITwitterSearchService twitterSearchService)
        {
            _twitterSearchService = twitterSearchService;
        }

        [HttpGet]
        [Route("api/search/{handle}")]
        public async Task<IActionResult> Get(string handle, int pageSize =10, ulong? sinceId = null)
        {
            try
            {
                SearchResult searchResult = await _twitterSearchService.SearchAsync(handle, pageSize,
                    sinceId);

                return Json(searchResult);
            }
            catch (ServerException ex)
            {
                Exception e = ex.GetBaseException();
                return StatusCode(500, new
                {
                    message = e.Message,
                    starckTrace = e.StackTrace
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    starckTrace = ex.StackTrace
                });
            }
            catch (NoDataException ex)
            {
                return NotFound(new
                {
                    message = ex.Message,
                    starckTrace = ex.StackTrace
                });
            }
            catch (Exception ex)
            {
                Exception e = ex.GetBaseException();
                return StatusCode(500, new
                {
                    message = e.Message,
                    starckTrace = e.StackTrace
                });
            }
        }
    }
}