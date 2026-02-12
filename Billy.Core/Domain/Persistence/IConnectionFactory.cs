using System.Data;
using System.Threading.Tasks;

namespace Billy.Domain.Persistence
{
    public interface IConnectionFactory<TConnection>
        where TConnection : IDbConnection
    {
        Task<TConnection> CreateAsync();
        TConnection Create();
    }
}
