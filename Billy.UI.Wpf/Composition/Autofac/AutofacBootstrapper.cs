using Autofac;
using Billy.Billing.Application;
using Billy.Core.Domain.Billing.Persistence.SQL.MSSQLServer;
using Billy.Domain.Billing.Persistence;
using Billy.Domain.Billing.Persistence.SQL.MSSQLServer;
using Billy.Domain.Billing.Persistence.SQL.MSSQLServer.Dapper;
//using Billy.UI.Wpf.Composition.Autofac.Modules;
using Billy.UI.Wpf.Presentation;
using Billy.UI.Wpf.Presentation.Billing;
using Billy.UI.Wpf.Services;
using Caliburn.Micro;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Billy.UI.Wpf.Composition.Autofac
{
    // Autofac Documentation:   http://autofac.readthedocs.org/en/latest/index.html
    // Autofac source code:     https://github.com/autofac/Autofac

    // TODO: separate configuration from DisplayRootViewFor
    internal sealed class AutofacBootstrapper : CustomBootstrapperBase<ShellViewModel>
    {
        #region ctor

        public AutofacBootstrapper()
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

            //var xx = new StyledWindow();
            //var rgaerg = xx is Window;

            //builder.RegisterType<CustomWindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<StyledWindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
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
            builder.RegisterInstance(new SqlConnection(@$"Server=.;" + "Integrated Security=SSPI;" + "Database=Billy;"))
                .As<IDbConnection>()
                .As<DbConnection>()
                .As<SqlConnection>()
                .OnActivating(x =>
                {
                    x.Instance.Open();
                })
                .SingleInstance();

            // SQLite3
            //builder.RegisterType<DapperSQLite3SuppliersRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<SQLite3BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

            // MS SQL Server
            //builder.Register(
            //    ctx =>
            //    {
            //        var ctxInternal = ctx.Resolve<IComponentContext>();

            //        return (DbConnection conn, DbTransaction tran =null) {
            //            return new DapperMSSQLServerSuppliersRepository(conn, tran);
            //    }
            //    })
            //    //.As<ISuppliersService>()
            //    .InstancePerLifetimeScope();

            // TODO: configure modules to register components groups for faster MSSQLS + Dapper, SQLite3 + Dapper, etc ...
            //builder.RegisterType<MSSQLServerBillingConnectionFactory>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MSSQLServerBillingUnitOfWorkFactory>().As<IBillingUnitOfWorkFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DapperMSSQLServerSuppliersRepository>()
                .As<ISuppliersRepository>()
                .As<MSSQLServerSuppliersRepository>()
                .InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork2>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

            // APPLICATION SERVICES
            //builder.Register(
            //    ctx =>
            //    {
            //        var ctxInternal = ctx.Resolve<IComponentContext>();

            //        return new LocalSuppliersService(
            //            () => ctxInternal.Resolve<IBillingUnitOfWork>());
            //    })
            builder.RegisterType<LocalSuppliersService>()
                .As<ISuppliersService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LocalBillingService>().As<IBillingService>().InstancePerLifetimeScope();

            // tracks
            //builder.RegisterType<FakeTracksInMemoryRepository>().As<ITracksRepository>().InstancePerLifetimeScope();
            //builder.Register(c => new NewtonsoftJsonTracksSerializer()).As<EntitySerializer<Track, uint>>().InstancePerLifetimeScope();
            //builder.RegisterType<SerializingTracksRepository>().As<ITracksRepository>().As<ITrackFactory>().InstancePerLifetimeScope();
            //builder.RegisterType<FakeTracksRepository>().As<ITracksRepository>().As<ITrackFactory>().InstancePerLifetimeScope();

            //builder.RegisterType<NewtonsoftJsonTracksSerializer>()
            //    .As<EntitySerializer<Track, uint>>().InstancePerLifetimeScope();
            //builder.RegisterType<SerializingTracksRepository>()
            //    .As<ITracksRepository>()
            //    .As<ITrackFactory>().InstancePerLifetimeScope();

            //builder.RegisterType<NewtonsoftJsonPlaylistsSerializer>()
            //    .As<EntitySerializer<PlaylistBase, uint>>().InstancePerLifetimeScope();
            //builder.RegisterType<SerializingPlaylistBasesRepository>()
            //    .As<IPlaylistsRepository>()
            //    .As<IPlaylistFactory>().InstancePerLifetimeScope();

            //builder.RegisterType<BillingViewModelsProxy>().InstancePerLifetimeScope();

            // ViewModels & Views

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<ShellView>()/*.As<IViewFor<ShellViewModel>>()*/.InstancePerLifetimeScope();
            builder.RegisterType<SuppliersViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AddSupplierViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<EditSupplierViewModel>().AsSelf().InstancePerDependency();
            //builder.Register(
            //    ctx =>
            //    {
            //        var ctxInternal = ctx.Resolve<IComponentContext>();

            //        return new LocalSuppliersService(
            //            () => ctxInternal.Resolve<IBillingUnitOfWork>());
            //    })
            //    .As<ISuppliersService>()
            //    .SingleInstance();

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

            //builder.RegisterType<EditTrackTagsViewModel>().AsSelf().InstancePerDependency();

            //builder.Register<Func<Track, EditTrackViewModel>>(
            //    ctx =>
            //    {
            //        var ctxInternal = ctx.Resolve<IComponentContext>();

            //        return (Track track) => new EditTrackViewModel(
            //            ctxInternal.Resolve<IReadLibraryService>(),
            //            ctxInternal.Resolve<IWriteLibraryService>(),
            //            track,
            //            ctxInternal.Resolve<Func<Track, EditTrackTagsViewModel>>());
            //    }).AsSelf().InstancePerDependency();
        }

        private IEnumerable<Assembly> assemblies;
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return this.assemblies ??= new[]
                {
                    typeof(ShellViewModel).Assembly,
                    typeof(ShellView).Assembly
                }
                .Distinct();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //if (Debugger.IsAttached)
            //    return;

            //e.Handled = true;

            //Application.Current.Shutdown();
        }

        #endregion
    }
}