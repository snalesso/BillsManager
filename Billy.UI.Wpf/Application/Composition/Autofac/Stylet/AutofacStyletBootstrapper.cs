//using Autofac;
//using Billy.Billing.Application;
//using Billy.Billing.Persistence.SQL.MSSQLServer;
//using Billy.Billing.Persistence;
//using Billy.Billing.Persistence.SQL.MSSQLServer.Dapper;
//using Billy.UI.Wpf.Presentation;
//using Billy.UI.Wpf.Presentation.Billing;
//using Billy.UI.Wpf.Services;
//using Microsoft.Data.SqlClient;
//using Stylet;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Reflection;
//using System.Text;

//namespace Billy.UI.Wpf.Composition.Autofac.Stylet
//{
//    internal class AutofacStyletBootstrapper<TRootViewModel> : BootstrapperBase
//        where TRootViewModel : class
//    {
//        private IContainer _container;

//        private object _rootViewModel;
//        protected virtual object RootViewModel
//        {
//            get { return this._rootViewModel ??= this.GetInstance(typeof(TRootViewModel)); }
//        }

//        protected override void ConfigureBootstrapper()
//        {
//            var builder = new ContainerBuilder();

//            this.DefaultConfigureIoC(builder);
//            this.ConfigureIoC(builder);

//            this._container = builder.Build();
//        }

//        /// <summary>
//        /// Carries out default configuration of the IoC container. Override if you don't want to do this
//        /// </summary>
//        protected virtual void DefaultConfigureIoC(ContainerBuilder builder)
//        {
//            var viewManagerConfig = new ViewManagerConfig()
//            {
//                ViewFactory = this.GetInstance,
//                ViewAssemblies = new List<Assembly>()
//                {
//                    this.GetType().Assembly
//                }
//            };
//            builder.RegisterInstance<IViewManager>(new ViewManager(viewManagerConfig));

//            builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
//            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
//            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
//            builder.RegisterType<MessageBoxViewModel>().As<IMessageBoxViewModel>().ExternallyOwned(); // Not singleton!
//            //builder.RegisterAssemblyTypes(this.GetType().Assembly).ExternallyOwned();
//        }

//        /// <summary>
//        /// Override to add your own types to the IoC container.
//        /// </summary>
//        protected virtual void ConfigureIoC(ContainerBuilder builder) { }

//        public override object GetInstance(Type type)
//        {
//            return this._container.Resolve(type);
//        }

//        protected override void Launch()
//        {
//            base.DisplayRootView(this.RootViewModel);
//        }

//        // TODO: review
//        public override void Dispose()
//        {
//            ScreenExtensions.TryDispose(this._rootViewModel);

//            if (this._container != null)
//                this._container.Dispose();

//            base.Dispose();
//        }
//    }

//    internal sealed class Bootstrapper : AutofacStyletBootstrapper<ShellViewModel>
//    {
//        protected override void ConfigureIoC(ContainerBuilder builder)
//        {
//            base.ConfigureIoC(builder);

//            // MODULES

//            //builder.RegisterModule<EventAggregationAutoSubscriptionModule>(); // TODO: review: automatic behavior with no counterpart for unsubscription

//            // CORE COMPONENTS

//            builder.RegisterType<StyledWindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
//            builder.RegisterType<WindowsDialogService>().As<IDialogService>().InstancePerLifetimeScope();

//            // INFRASTRUCTURE

//            // DOMAIN SERVICES

//            //builder.Register(x =>
//            //{
//            //    var conn = new SQLiteConnection(
//            //        //@"Data Source=.\persons.sqlite3.db"
//            //        @"Data Source=:memory:;"
//            //        + "Version=3;"
//            //        + "DateTimeFormat=Ticks;");
//            //    // TODO: delay opening as late as possible
//            //    conn.Open();

//            //    return conn;

//            //})
//            builder.RegisterInstance(new SqlConnection(@$"Server=.;" + "Integrated Security=SSPI;" + "Database=Billy;"))
//                .As<IDbConnection>()
//                .As<DbConnection>()
//                .As<SqlConnection>()
//                .OnActivating(x =>
//                {
//                    x.Instance.Open();
//                })
//                .SingleInstance();

//            // SQLite3

//            //builder.RegisterType<DapperSQLite3SuppliersRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
//            //builder.RegisterType<SQLite3BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

//            // MS SQL Server

//            // TODO: configure modules to register components groups for faster MSSQLS + Dapper, SQLite3 + Dapper, etc ...
//            //builder.RegisterType<MSSQLServerBillingConnectionFactory>().AsSelf().InstancePerLifetimeScope();
//            builder.RegisterType<MSSQLServerBillingUnitOfWorkFactory>().As<IBillingUnitOfWorkFactory>().InstancePerLifetimeScope();
//            builder.RegisterType<DapperMSSQLServerSuppliersRepository>()
//                .As<ISuppliersRepository>()
//                .As<MSSQLServerSuppliersRepository>()
//                .InstancePerLifetimeScope();
//            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
//            //builder.RegisterType<MSSQLServerBillingUnitOfWork2>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
//            //builder.RegisterType<BillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();
//            //builder.RegisterType<MSSQLServerBillingUnitOfWork>().As<IBillingUnitOfWork>().InstancePerLifetimeScope();

//            // APPLICATION SERVICES

//            builder.RegisterType<LocalSuppliersService>().As<ISuppliersService>().InstancePerLifetimeScope();
//            builder.RegisterType<LocalBillingService>().As<IBillingService>().InstancePerLifetimeScope();

//            // PRESENTATION

//            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
//            builder.RegisterType<SuppliersViewModel>().AsSelf().InstancePerLifetimeScope();
//            builder.RegisterType<AddSupplierViewModel>().AsSelf().InstancePerDependency();
//            builder.RegisterType<EditSupplierViewModel>().AsSelf().InstancePerDependency();

//            //builder.Register<Func<Track, TrackViewModel>>(
//            //    ctx =>
//            //    {
//            //        var ctxInternal = ctx.Resolve<IComponentContext>();
//            //        return (Track track) => new TrackViewModel(
//            //            track,
//            //            ctxInternal.Resolve<IAudioPlaybackEngine>(),
//            //            ctxInternal.Resolve<IDialogService>(),
//            //            ctxInternal.Resolve<Func<Track, EditTrackTagsViewModel>>());
//            //    }).AsSelf().InstancePerLifetimeScope();
//        }
//    }
//}
