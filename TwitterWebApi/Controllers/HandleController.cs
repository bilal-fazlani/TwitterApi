using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TwitterWebApi.Controllers
{
    public class HandleController : Controller
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _datebase;
        private readonly IMongoCollection<Handle> _collection;

        public HandleController(IConfigurationRoot configurationRoot)
        {
            _client = new MongoClient(configurationRoot["mongoConnectionString"]);
            _datebase = _client.GetDatabase("twitterdeck");
            _collection = _datebase.GetCollection<Handle>("handle");
        }

        [HttpGet]
        [Route("/api/handle/list")]
        public async Task<IActionResult> List()
        {
            var handles = (await _collection
                    .AsQueryable()
                    .ToListAsync())
                .Select(x => new {name = x.name, _id = x._id.ToString()});

            return Json(handles);
        }

        [HttpPost]
        [Route("/api/handle")]
        public async Task<IActionResult> Add([FromBody]Handle handle)
        {
            await _collection.InsertOneAsync(handle);
            return StatusCode(((int) HttpStatusCode.Created), handle._id.ToString());
        }

        [HttpDelete]
        [Route("/api/handle/{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            ObjectId objectId = ObjectId.Parse(id);
            await _collection.FindOneAndDeleteAsync(x => x._id == objectId);
            return StatusCode((int) HttpStatusCode.OK);
        }
    }
}