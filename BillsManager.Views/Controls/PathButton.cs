using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BillsManager.Views.Controls
{
    public class PathButton : Button
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data",
        typeof(Geometry),
        typeof(PathButton));

        [EditorBrowsable]
        [Category("Common")]
        public Geometry Data
        {
            get { return (Geometry)this.GetValue(DataProperty); }
            set { this.SetValue(DataProperty, value); }
        }
    }
}