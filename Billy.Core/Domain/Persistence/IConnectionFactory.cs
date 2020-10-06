using System.Data;
using System.Threading.Tasks;

namespace Billy.Core.Domain.Persistence
{
    public interface IConnectionFactory<TConnection>
        where TConnection : IDbConnection
    {
        Task<TConnection> CreateAsync();
        TConnection Create();
    }
}
