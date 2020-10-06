using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Billy.UI.Wpf
{
    internal static class EntryPoint
    {
        private static readonly bool _wasCreatedNew;
        private static readonly Mutex _mutex = new Mutex(true, $"SergioNalesso.{nameof(Billy)}.{nameof(Mutex)}!! :D", out _wasCreatedNew);

        [STAThread]
        public static void Main()
        {
            if (_wasCreatedNew && _mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();
                app.Run();

                _mutex.ReleaseMutex();
                _mutex.Close();
            }
            else
            {
                MessageBox.Show($"{nameof(Billy)} is already running!", "Already running", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}