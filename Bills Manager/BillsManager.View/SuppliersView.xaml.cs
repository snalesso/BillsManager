using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BillsManager.View
{
    /// <summary>
    /// Logica di interazione per SuppliersView.xaml
    /// </summary>
    public partial class SuppliersView : UserControl
    {
        public SuppliersView()
        {
        //    FrameworkElement.LanguageProperty.OverrideMetadata(
        //       typeof(FrameworkElement),
        //       new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            InitializeComponent();
        }
    }
}
