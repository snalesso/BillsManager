using Autofac;
using Billy.Billing.Persistence;
using Billy.Billing.Persistence.SQL.MSSQLServer;
using Billy.Billing.Persistence.SQL.MSSQLServer.Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Billy.UI.Wpf.Composition.Autofac.Modules
{
    internal class Billing_MSSQLServer : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new SqlConnection(@$"Server=.;" + "Integrated Security=SSPI;" + "Database=Billy;"))
                .As<IDbConnection>()
                .As<DbConnection>()
                .As<SqlConnection>()
                .OnActivating(connectionAEA => connectionAEA.Instance.Open())
                .SingleInstance();
            builder.RegisterType<MSSQLServerBillingUnitOfWorkFactory>().As<IBillingUnitOfWorkFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DapperMSSQLServerSuppliersRepository>()
                .AsSelf()
                .As<ISuppliersRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
