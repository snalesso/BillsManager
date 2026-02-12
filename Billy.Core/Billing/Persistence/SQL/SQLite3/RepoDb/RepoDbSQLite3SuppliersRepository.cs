//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using RepoDb;
//using System.Data.SQLite;
//using System.Data.SQLite.Generic;
//using RepoDb.Extensions;
//using Billy.Billing.Models;
//using Billy.Billing.Persistence;
//using System.Reactive.Disposables;

//namespace Billy.Billing.Persistence.SQL.SQLite3.RepoDb
//{
//    public class RepoDbSQLite3SuppliersRepository : BaseRepository<Supplier, SQLiteConnection>, ISuppliersRepository
//    {
//        private readonly SQLiteConnection _connection;

//        public RepoDbSQLite3SuppliersRepository(SQLiteConnection connection) : base(connection.ConnectionString)
//        {
//            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
//        }

//        public async Task<Supplier> CreateAndAddAsync(NewSupplier newSupplier)
//        {
//            await this._connection.OpenAsync();

//            var x = await this._connection.InsertAsync<NewSupplier, Supplier>(newSupplier);

//            return x;
//        }

//        public Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IReadOnlyList<Supplier>> GetSuppliersAsync()
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveAsync(uint id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveAsync(IEnumerable<uint> ids)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveAsync(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveAsync(IEnumerable<int> ids)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync(uint id, ISupplier supplier)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes)
//        {
//            throw new NotImplementedException();
//        }

//        Task<IReadOnlyCollection<Supplier>> ISuppliersRepository.GetAllAsync()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
