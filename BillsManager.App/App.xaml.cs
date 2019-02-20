using BillsManager.App.Composition;
using BillsManager.App.Composition.Bootstrapping;
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
        private readonly AutofacBootstrapper _bootstrapper;

        public App()
        {
            this._bootstrapper = new AutofacBootstrapper();
        }

        #region single instance

        // Single Instance by Microsoft: http://www.codeproject.com/Articles/84270/WPF-Single-Instance-Application

        private const string Unique = "BillsManagerByNalessoSergio_PEACE";
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

        #endregion
    }
}