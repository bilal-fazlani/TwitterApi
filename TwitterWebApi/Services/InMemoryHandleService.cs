using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TwitterWebApi.Services
{
    public class InMemoryHandleService : IHandleService
    {
        private readonly Dictionary<ObjectId, Handle> _data = new Dictionary<ObjectId, Handle>();

        public async Task AddHandle(Handle handle)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            handle._id = ObjectId.GenerateNewId();
            _data.Add(handle._id, handle);
        }

        public Task<IEnumerable<Handle>> Gethandles()
        {
            return Task.FromResult(_data.Select(x => x.Value));
        }

        public Task RemoveHandle(string id)
        {
            _data.Remove(ObjectId.Parse(id));
            return Task.CompletedTask;
        }
    }
}