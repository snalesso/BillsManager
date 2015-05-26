using BillsManager.App.Bootstrappers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BillsManager.App
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private readonly AutofacBootstrapper boot;

        public App()
        {
            this.boot = new AutofacBootstrapper();
        }

        private const string Unique = "2u-39Fàè+ùEW)8||3kdj+ò*éç*°23r4eè23?kc2dK?M5faJW$/243F;8722%f0^aw[93jrfwelr";
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...
            // TODO: bring to front
            return true;
        }
    }
}