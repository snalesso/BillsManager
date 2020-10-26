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
//        }
//    }
//}
