using Autofac;
using BillsManager.App.Modules;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using IContainer = Autofac.IContainer;

namespace BillsManager.App.Bootstrappers
{
    public class BootstrapperEx<TRootViewModel> : Bootstrapper<TRootViewModel>
    {
        protected IContainer Container { get; private set; }

        public bool EnforceNamespaceConvention { get; set; }

        public bool AutoSubscribeEventAggegatorHandlers { get; set; }

        public Type ViewModelBaseType { get; set; }

        public Func<IWindowManager> CreateWindowManager { get; set; }

        public Func<IEventAggregator> CreateEventAggregator { get; set; }

        protected override void Configure()
        {
            ConfigureBootstrapper();

            if (CreateWindowManager == null)
                throw new ArgumentNullException("CreateWindowManager");

            if (CreateEventAggregator == null)
                throw new ArgumentNullException("CreateEventAggregator");

            var builder = new ContainerBuilder();

            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))
                //  must be in a namespace ending with ViewModels
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("ViewModels"))
                //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
              .Where(type => type.GetInterface(this.ViewModelBaseType.Name) != null)
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type that ends with View
              .Where(type => type.Name.EndsWith("View"))
                //  must be in a namespace that ends in Views
              .Where(type => !(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("Views"))
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register<IWindowManager>(c => CreateWindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => CreateEventAggregator()).InstancePerLifetimeScope();

            if (AutoSubscribeEventAggegatorHandlers)
                builder.RegisterModule<EventAggregationAutoSubscriptionModule>();

            ConfigureContainer(builder);

            Container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                object obj;
                if (Container.TryResolve(service, out obj))
                    return obj;
            }
            else
            {
                object obj;
                if (Container.TryResolveNamed(key, service, out obj))
                    return obj;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { service })) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected virtual void ConfigureBootstrapper()
        {
            EnforceNamespaceConvention = true;
            AutoSubscribeEventAggegatorHandlers = false;
            ViewModelBaseType = typeof(IScreen);
            CreateWindowManager = () => (IWindowManager)new MinimalWindowManager();
            CreateEventAggregator = () => (IEventAggregator)new EventAggregator();
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<TRootViewModel>(new Dictionary<string, object>
            {
                { "SizeToContent", SizeToContent.Manual},
                { "Width", 1350},
                { "Height", 750},
                { "StartupLocation", WindowStartupLocation.CenterScreen},
                { "WindowState", WindowState.Maximized}
            });
        }
    }
}