using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
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
            //var x = new Xfewfwe();
            //x.PropertyChanged += X_PropertyChanged;

            //var ro = x as ReactiveObject;
            ////ro.PropertyChanged += Ro_PropertyChanged;

            //var roex = x as ReactiveObjectEx;
            //roex.PropertyChanged += Roex_PropertyChanged;

            //var rs = x as Screen;
            //rs.PropertyChanged += Rs_PropertyChanged;

            //x.Name = "fewofjwejfwoe";

            Thread.CurrentThread.Name = "Main thread";

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

        //private static void Rs_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    MessageBox.Show("here from rs");
        //    //throw new NotImplementedException();
        //}

        //private static void Roex_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    MessageBox.Show("here from roex");
        //    //throw new NotImplementedException();
        //}

        //private static void Ro_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    MessageBox.Show("here from ro");
        //    //throw new NotImplementedException();
        //}

        //private static void X_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    MessageBox.Show("here from feawfawefaw");
        //    //throw new NotImplementedException();
        //}
    }

    //public class Xfewfwe : ReactiveScreen // ReactiveObject
    //{
    //    private string _name;
    //    public string Name
    //    {
    //        get => this._name;
    //        set => this.RaiseAndSetIfChanged(ref this._name, value);
    //    }
    //}
}