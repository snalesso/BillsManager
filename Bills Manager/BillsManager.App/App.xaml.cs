using BillsManager.App.Bootstrappers;
using System.Windows;

namespace BillsManager.App
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AutofacBootstrapper bootstrapper;

        public App()
        {
            this.bootstrapper = new AutofacBootstrapper();
        }
    }
}