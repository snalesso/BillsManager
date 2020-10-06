using System;
using System.Threading.Tasks;

namespace Billy.Core.Domain.Persistence
{
    public interface IUnitOfWork : IDisposable // TODO: IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }

    //interface Repo<T>
    //    where T : class
    //{

    //    Task<K> GetAll<K>();


    //}
}