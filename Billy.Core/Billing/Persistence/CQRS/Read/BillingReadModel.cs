using Billy.Billing.Application.DTOs;
using Billy.Billing.Models;
using Billy.Domain.CQRS.Query;
using Billy.Domain.Persistence;
using Billy.Domain.Persistence.SQL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.CQRS.Read
{
    public abstract class BillingReadModel :
        IQueryHandler<SupplierSummariesQuery, IReadOnlyCollection<SupplierSummaryDto>>
        , IQueryHandler<SupplierHeadersQuery, IReadOnlyCollection<SupplierHeaderDto>>
        , IQueryHandler<DetailedBillsQuery, IReadOnlyCollection<DetailedBillDto>>
    {
        private readonly DbConnection _connection;
        protected DbConnection Connection => this._connection;

        private readonly DbTransaction _transaction;
        protected DbTransaction Transaction => this._transaction?.Connection != null ? this._transaction : null;

        public BillingReadModel(DbConnection connection)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public abstract Task<IReadOnlyCollection<DetailedBillDto>> HandleAsync(DetailedBillsQuery query);
        public abstract Task<IReadOnlyCollection<SupplierSummaryDto>> HandleAsync(SupplierSummariesQuery query);
        public abstract Task<IReadOnlyCollection<SupplierHeaderDto>> HandleAsync(SupplierHeadersQuery query);
    }

    public class DapperSQLite3BillingReadModel : BillingReadModel
    {
        public DapperSQLite3BillingReadModel(SQLiteConnection connection) : base(connection)
        {
        }

        public override async Task<IReadOnlyCollection<DetailedBillDto>> HandleAsync(DetailedBillsQuery query)
        {
            try
            {
                var cmd = new CommandDefinition(
                    $"select bill.*, supplier.{nameof(Supplier.Id)}, supplier.{nameof(Supplier.Name)}" +
                    $"from {nameof(Bill)} bill left join {nameof(Supplier)} supplier " +
                    $"on bill.{nameof(Bill.SupplierId)} = supplier.{nameof(Supplier.Id)}",
                    this.Transaction);

                var detailedBills = new List<DetailedBillDto>();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(this.Connection, cmd).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var detailedBillDto = new DetailedBillDto()
                        {
                            Id = await reader.GetSafeAsync<long>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            AdditionalCosts = await reader.GetSafeAsync<double>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            Agio = await reader.GetSafeAsync<double>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            Amount = await reader.GetSafeAsync<double>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            Code = await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            DueDate = await reader.GetSafeAsync<DateTime>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            Notes = await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            PaymentDate = await reader.GetSafeAsync<DateTime?>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            RegistrationDate = await reader.GetSafeAsync<DateTime>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            ReleaseDate = await reader.GetSafeAsync<DateTime>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                            SupplierHeader = new SupplierHeaderDto()
                            {
                                Id = await reader.GetSafeAsync<long>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false),
                                Name = await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Bill), nameof(Bill.Id))).ConfigureAwait(false)
                            }
                        };
                    }
                }

                return detailedBills.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Task<IReadOnlyCollection<SupplierSummaryDto>> HandleAsync(SupplierSummariesQuery query)
        {
            throw new NotImplementedException();
        }

        public override Task<IReadOnlyCollection<SupplierHeaderDto>> HandleAsync(SupplierHeadersQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
