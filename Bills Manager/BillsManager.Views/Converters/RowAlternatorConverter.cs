using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace BillsManager.Views.Converters
{
    public class RowAlternatorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as DependencyObject;
            var listControl = ItemsControl.ItemsControlFromItemContainer(item) as ItemsControl;
            var index = listControl.ItemContainerGenerator.IndexFromContainer(item, false);

            if (index % 2 == 0)
                return this.Color1;
            else
                return this.Color2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [ConstructorArgument("color1")]
        public Brush Color1 { get; set; }

        [ConstructorArgument("color2")]
        public Brush Color2 { get; set; }

        #endregion
    }
}
