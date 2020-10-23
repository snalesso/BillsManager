using Autofac;
using Billy.Billing.Services;
using Billy.UI.Wpf.Common.Services;
using Caliburn.Micro;

namespace Billy.UI.Wpf.Composition.Autofac.Modules
{
    internal class Application_Services : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<MaterialDesignWindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<WindowsDialogService>().As<IDialogService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalSuppliersService>().As<ISuppliersService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalBillingService>().As<IBillingService>().InstancePerLifetimeScope();
        }
    }
}
