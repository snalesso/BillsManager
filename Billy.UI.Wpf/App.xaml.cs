using Billy.UI.Wpf.Composition.Autofac.Caliburn.Micro;

namespace Billy.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly AutofacCaliburnMicroBootstrapper  _bootstrapper;

        public App()
        {
            // this method is not available if App.xaml StartupUri is not set
            this.InitializeComponent();

            this._bootstrapper = new AutofacCaliburnMicroBootstrapper();
            /* bootstrapper.Initialize() needs to be placed here because CM's bootstrapper internally subscribes to:
             * Application.Startup
             * Application.DispatcherUnhandledException
             * Application.Exit += OnExit
             */
            this._bootstrapper.Initialize();
        }
    }
}