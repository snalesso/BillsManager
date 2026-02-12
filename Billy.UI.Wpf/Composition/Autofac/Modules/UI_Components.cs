using Autofac;
using Billy.Billing.ViewModels;
using Billy.UI.Wpf.Root.ViewModels;

namespace Billy.UI.Wpf.Composition.Autofac.Modules
{
    internal class UI_Components : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<ShellView>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SuppliersViewModel>().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<SuppliersView>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SupplierViewModel>().AsSelf().InstancePerDependency();
            builder.RegisterType<AddSupplierViewModel>().AsSelf().InstancePerDependency();
            //builder.RegisterType<AddSupplierView>().AsSelf().InstancePerDependency();
            builder.RegisterType<SupplierEditorViewModel>().AsSelf().InstancePerDependency();
            //builder.RegisterType<SupplierEditor>().AsSelf().InstancePerDependency();
            builder.RegisterType<EditSupplierViewModel>().AsSelf().InstancePerDependency();
            //builder.RegisterType<EditSupplierView>().AsSelf().InstancePerDependency();

            builder.RegisterType<BillsFiltersViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<BillsViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<BillViewModel>().AsSelf().InstancePerDependency();
        }
    }
}
