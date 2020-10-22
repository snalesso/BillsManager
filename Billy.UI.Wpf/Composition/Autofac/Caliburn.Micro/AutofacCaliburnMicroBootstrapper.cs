using Autofac;
using Billy.Billing.Persistence;
using Billy.Billing.Persistence.SQL.MSSQLServer;
using Billy.Billing.Persistence.SQL.MSSQLServer.Dapper;
using Billy.Billing.Persistence.SQL.SQLite3;
using Billy.Billing.Services;
using Billy.Billing.ViewModels;
using Billy.Billing.Views;
using Billy.UI.Wpf.Common.Services;
using Billy.UI.Wpf.Composition.Autofac.Modules;
using Billy.UI.Wpf.Root.ViewModels;
using Billy.UI.Wpf.Root.Views;
using global::Caliburn.Micro;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Billy.UI.Wpf.Composition.Autofac.Caliburn.Micro
{
    // Autofac Documentation:   http://autofac.readthedocs.org/en/latest/index.html
    // Autofac source code:     https://github.com/autofac/Autofac

    // TODO: separate configuration from DisplayRootViewFor
    internal sealed class AutofacCaliburnMicroBootstrapper : CustomBootstrapperBase<ShellViewModel>
    {
        #region ctor

        public AutofacCaliburnMicroBootstrapper()
        {
            // TODO settings file
            this.RootViewDisplaySettings = new Dictionary<string, object>
            {
                { nameof(Window.Width), 1500 },
                { nameof(Window.Height), 850 },
                { nameof(Window.WindowState), WindowState.Normal },
                { nameof(Window.WindowStartupLocation), WindowStartupLocation.CenterScreen },
                { nameof(Window.SizeToContent), SizeToContent.Manual },
            };
        }

        #endregion

        #region methods

        protected override void RegisterComponents(ContainerBuilder builder)
        {
            base.RegisterComponents(builder);

            // MODULES

            //builder.RegisterModule<EventAggregationAutoSubscriptionModule>(); // TODO: review: automatic behavior with no counterpart for unsubscription

            // CORE COMPONENTS

            builder.RegisterType<MaterialDesignWindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<WindowsDialogService>().As<IDialogService>().InstancePerLifetimeScope();

            // INFRASTRUCTURE

            // DOMAIN SERVICES

            //builder.Register(x =>
            //{
            //    var conn = new SQLiteConnection(
            //        //@"Data Source=.\persons.sqlite3.db"
            //        @"Data Source=:memory:;"
            //        + "Version=3;"
            //        + "DateTimeFormat=Ticks;");
            //    // TODO: delay opening as late as possible
            //    conn.Open();

            //    return conn;

            //})
            //builder.RegisterInstance(new SqlConnection(@$"Server=.;" + "Integrated Security=SSPI;" + "Database=Billy;"))
            //    .As<IDbConnection>()
            //    .As<DbConnection>()
            //    .As<SqlConnection>()
            //    .OnActivating(connectionAEA => connectionAEA.Instance.Open())
            //    .SingleInstance();<
            //builder.RegisterType<MSSQLServerBillingUnitOfWorkFactory>().As<IBillingUnitOfWorkFactory>().InstancePerLifetimeScope();
            //builder.RegisterType<DapperMSSQLServerSuppliersRepository>()
            //    .As<ISuppliersRepository>()
            //    .As<MSSQLServerSuppliersRepository>()
            //    .InstancePerLifetimeScope();

            //builder.RegisterInstance(new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString))
            //    .As<IDbConnection>()
            //    .As<DbConnection>()
            //    .As<SQLiteConnection>()
            //    .OnActivating(connectionAEA => connectionAEA.Instance.Open())
            //    .SingleInstance();

            // SQLite3

            //builder.RegisterType<DapperSQLite3SuppliersRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<SQLite3BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

            // MS SQL Server
            //builder.RegisterModule<Billing_MSSQLServer>();
            builder.RegisterModule<Billing_SQLite3>();

            // TODO: configure modules to register components groups for faster MSSQLS + Dapper, SQLite3 + Dapper, etc ...
            //builder.RegisterType<MSSQLServerBillingConnectionFactory>().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork2>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

            // APPLICATION SERVICES

            builder.RegisterType<LocalSuppliersService>().As<ISuppliersService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalBillingService>().As<IBillingService>().InstancePerLifetimeScope();

            // PRESENTATION

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ShellView>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SuppliersViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SuppliersView>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SupplierViewModel>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<AddSupplierViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<AddSupplierView>().AsSelf().InstancePerDependency();

            builder.RegisterType<SupplierEditorViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<SupplierEditor>().AsSelf().InstancePerDependency();
            builder.RegisterType<EditSupplierViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<EditSupplierView>().AsSelf().InstancePerDependency();
            builder.RegisterType<SupplierEditor>().AsSelf().InstancePerDependency();

            //builder.Register<Func<Track, TrackViewModel>>(
            //    ctx =>
            //    {
            //        var ctxInternal = ctx.Resolve<IComponentContext>();
            //        return (Track track) => new TrackViewModel(
            //            track,
            //            ctxInternal.Resolve<IAudioPlaybackEngine>(),
            //            ctxInternal.Resolve<IDialogService>(),
            //            ctxInternal.Resolve<Func<Track, EditTrackTagsViewModel>>());
            //    }).AsSelf().InstancePerLifetimeScope();
        }

        private IEnumerable<Assembly> assemblies;
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return this.assemblies ??= new[]
                {
                    typeof(ShellViewModel).Assembly,
                    typeof(ShellView).Assembly,
                    typeof(EditSupplierViewModel).Assembly,
                    typeof(EditSupplierView).Assembly
                }
                .Distinct();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
                return;

            e.Handled = true;

            Application.Current.Shutdown();
        }

        #endregion
    }
}