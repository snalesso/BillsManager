using Autofac;
using Billy.Billing.ViewModels;
using Billy.Billing.Views;
using Billy.UI.Wpf.Composition.Autofac.Modules;
using Billy.UI.Wpf.Root.ViewModels;
using Billy.UI.Wpf.Root.Views;
using System.Collections.Generic;
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

            //builder.RegisterModule<Billing_MSSQLServer>();
            builder.RegisterModule<Billing_SQLite3_Dapper>();

            builder.RegisterModule<Application_Services>();

            builder.RegisterModule<UI_Components>();
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