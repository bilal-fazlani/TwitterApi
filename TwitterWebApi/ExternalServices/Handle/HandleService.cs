using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TwitterWebApi.ExternalServices.Handle
{
    public class HandleService : IHandleService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _datebase;
        private readonly IMongoCollection<Models.Handle> _collection;

        public HandleService(IConfiguration configurationRoot)
        {
            _client = new MongoClient(configurationRoot["mongoConnectionString"]);
            _datebase = _client.GetDatabase("twitterdeck");
            _collection = _datebase.GetCollection<Models.Handle>("handle");
        }

        public async Task AddHandle(Models.Handle handle)
        {
            await _collection.InsertOneAsync(handle);
        }

        public async Task<IEnumerable<Models.Handle>> Gethandles()
        {
            IEnumerable<Models.Handle> handles = (await _collection
                    .AsQueryable()
                    .ToListAsync())
                .Select(x => new Models.Handle {name = x.name, _id = x._id});

            return handles;
        }

        public async Task RemoveHandle(string id)
        {
            ObjectId objectId = ObjectId.Parse(id);
            await _collection.FindOneAndDeleteAsync(x => x._id == objectId);
        }
    }
}