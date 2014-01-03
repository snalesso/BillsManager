using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using BillsManager.Services.Providers;
using BillsManager.Views;
using BillsManager.ViewModels;
using BillsManager.ViewModels.Factories;
using Caliburn.Micro;
using Autofac;

namespace BillsManager.App
{
    public class AutofacBootstrapper : Bootstrapper<IShell>
    {
        ContainerBuilder container;

        protected override void Configure()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "View",
                DefaultSubNamespaceForViewModels = "ViewModel"
            };

            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            this.container = new ContainerBuilder();


            // Caliburn.Micro components
            this.container.RegisterType<OriginalWindowManager>().As<IWindowManager>().SingleInstance();
            //this.container.Singleton<IWindowManager, BorderlessWindowManager>();
            this.container.Singleton<IEventAggregator, EventAggregator>();

            // Services
            //this.container.Singleton<IDialogService, DialogService>();
            //this.container.Singleton<IBillsProvider, XMLBillsProvider>();
            //this.container.Singleton<ISuppliersProvider, XMLSuppliersProvider>();
            //this.container.Singleton<IBackupsProvider, XMLBackupsProvider>();
            this.container.Singleton<IDBsProvider, XMLDBsProvider>();

            // Factories
            this.container.Singleton<IFactory<BillsViewModel>, BillsViewModelFactory>();
            this.container.Singleton<IFactory<SuppliersViewModel>, SuppliersViewModelFactory>();
            this.container.Singleton<IFactory<BackupsViewModel>, BackupsViewModelFactory>();

            // ViewModels
            this.container.Singleton<IShell, ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = this.container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            //    ResourceDictionary resource = new ResourceDictionary
            //        {
            //            Source = new Uri("/BillsManager.View;component/Dictionaries/Styles/Values.xaml", UriKind.RelativeOrAbsolute)
            //        };

            //    SolidColorBrush foregroundBrush = resource.MergedDictionaries.;

            this.DisplayRootViewFor<IShell>(settings: new Dictionary<string, object>
            {
                { "WindowState", WindowState.Maximized},
                { "SizeToContent", SizeToContent.Manual},
                { "Width", 1350},
                { "Height", 750}
            });
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>(base.SelectAssemblies());

            assemblies.Add(Assembly.GetAssembly(typeof(ShellView)));

            return assemblies;
        }
    }
}