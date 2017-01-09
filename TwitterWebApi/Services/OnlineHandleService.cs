using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TwitterWebApi.Services
{
    public class OnlineHandleService : IHandleService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _datebase;
        private readonly IMongoCollection<Handle> _collection;

        public OnlineHandleService(IConfigurationRoot configurationRoot)
        {
            _client = new MongoClient(configurationRoot["mongoConnectionString"]);
            _datebase = _client.GetDatabase("twitterdeck");
            _collection = _datebase.GetCollection<Handle>("handle");
        }

        public async Task AddHandle(Handle handle)
        {
            await _collection.InsertOneAsync(handle);
        }

        public async Task<IEnumerable<Handle>> Gethandles()
        {
            IEnumerable<Handle> handles = (await _collection
                    .AsQueryable()
                    .ToListAsync())
                .Select(x => new Handle {name = x.name, _id = x._id});

            return handles;
        }

        public async Task RemoveHandle(string id)
        {
            ObjectId objectId = ObjectId.Parse(id);
            await _collection.FindOneAndDeleteAsync(x => x._id == objectId);
        }
    }
}