using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterWebApi.Services
{
    public interface IHandleService
    {
        Task AddHandle(Handle handle);

        Task<IEnumerable<Handle>> Gethandles();

        Task RemoveHandle(string id);
    }
}