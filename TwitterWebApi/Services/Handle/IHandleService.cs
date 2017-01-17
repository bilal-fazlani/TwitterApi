using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterWebApi.Services.Handle
{
    public interface IHandleService
    {
        Task AddHandle(Models.Handle handle);

        Task<IEnumerable<Models.Handle>> Gethandles();

        Task RemoveHandle(string id);
    }
}