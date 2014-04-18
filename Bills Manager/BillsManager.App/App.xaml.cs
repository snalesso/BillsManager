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
        public App()
        {
            new AutofacBootstrapper();
        }

        private const string Unique = "My_Unique_Application_String";
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
            return true;
        }
    }
}