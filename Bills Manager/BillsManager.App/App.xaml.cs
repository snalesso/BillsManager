using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace BillsManager.App
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AppBootstrapper bootstrapper;

        public App()
        {
            this.bootstrapper = new AppBootstrapper();
        }
    }
}
