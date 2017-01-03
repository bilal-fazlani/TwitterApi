using Microsoft.AspNetCore.Mvc;

namespace TwitterWebApi.Controllers
{
    public class HandleController : Controller
    {
        [HttpGet]
        [Route("/api/handle/list")]
        public IActionResult List()
        {
            return null;
        }

        [HttpPost]
        [Route("/api/handle")]
        public IActionResult Add(Handle handle)
        {
            return null;
        }

        [HttpDelete]
        [Route("/api/handle")]
        public IActionResult Remove(Handle handle)
        {
            return null;
        }
    }

    public class Handle
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}