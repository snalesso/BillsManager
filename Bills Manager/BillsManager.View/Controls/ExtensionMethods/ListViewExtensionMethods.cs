using System.Windows.Controls;
using System.Windows.Media;

namespace BillsManager.Views.Controls.ExtensionMethods
{
    public static class ListViewExtensionMethods
    {
        public static ScrollViewer GetScrollViewer(this ListView listView)
        {
            Decorator scroll_border = listView. as Decorator;
            if (scroll_border is Decorator)
            {
                ScrollViewer scroll = scroll_border.Child as ScrollViewer;
                if (scroll is ScrollViewer)
                {
                    return scroll;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}