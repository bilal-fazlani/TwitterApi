using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TwitterWebApi.Services.Handle
{
    public class InMemoryHandleService : IHandleService
    {
        private readonly Dictionary<ObjectId, Models.Handle> _data = new Dictionary<ObjectId, Models.Handle>();

        public async Task AddHandle(Models.Handle handle)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            handle._id = ObjectId.GenerateNewId();
            _data.Add(handle._id, handle);
        }

        public Task<IEnumerable<Models.Handle>> Gethandles()
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