using Autofac;
using Billy.Billing.Persistence;
using Billy.Billing.Persistence.Dapper;
using Billy.Billing.Persistence.SQL.SQLite3;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Billy.UI.Wpf.Composition.Autofac.Modules
{
    internal class Billing_SQLite3 : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString))
                .As<IDbConnection>()
                .As<DbConnection>()
                .As<SQLiteConnection>()
                .OnActivating(connectionAEA => connectionAEA.Instance.Open())
                .SingleInstance();
            builder.RegisterType<SQLite3BillingUnitOfWorkFactory>().As<IBillingUnitOfWorkFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SQLite3DapperSuppliersRepository>()
                .AsSelf()
                .As<ISuppliersRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
