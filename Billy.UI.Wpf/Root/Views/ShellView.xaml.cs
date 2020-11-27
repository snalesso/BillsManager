using Billy.UI.Wpf.Root.ViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Billy.UI.Wpf.Root.Views
{
    public class ShellViewUserControl : ReactiveUserControl<ShellViewModel> { }
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : ShellViewUserControl
    {
        public ShellView()
        {
            this.InitializeComponent();
        }
    }
}