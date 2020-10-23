using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using Autofac;
using Billy.Billing.Persistence;
using Billy.Billing.Persistence.Dapper;
using Billy.Billing.Persistence.SQL.SQLite3;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System;

namespace Billy.UI.Wpf.Composition.Autofac.Modules
{
    internal class Billing_SQLite3_Dapper : Module
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
            builder.RegisterType<SQLite3BillingUnitOfWorkFactory>()
                .As<IBillingUnitOfWorkFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SQLite3DapperSuppliersRepository>()
                //.AsSelf()
                .As<ISuppliersRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DapperSQLite3BillsRepository>()
                //.AsSelf()
                .As<IBillsRepository>()
                .InstancePerLifetimeScope();

            builder.Register(
                ctx =>
                {
                    static ISuppliersRepository factoryMethod(SQLiteConnection conn, SQLiteTransactionBase trans) => new DapperSQLite3SuppliersRepository(conn, trans);
                    return (Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository>)factoryMethod;
                })
                .InstancePerDependency();
            builder.Register(
                ctx =>
                {
                    static IBillsRepository factoryMethod(SQLiteConnection conn, SQLiteTransactionBase trans) => new DapperSQLite3BillsRepository(conn, trans);
                    return (Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository>)factoryMethod;
                })
                .InstancePerDependency();
        }
    }
}
