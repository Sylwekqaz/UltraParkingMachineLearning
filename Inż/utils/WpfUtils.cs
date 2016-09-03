using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Inż.utils
{
    public static class WpfUtils
    {
        public static Point GetPointRelativeToSource(this MouseButtonEventArgs e, Image image)
        {
            return new Point
            {
                X = e.GetPosition(image).X*image.Source.Width/image.ActualWidth,
                Y = e.GetPosition(image).Y*image.Source.Height/image.ActualHeight
            };
        }
    }
}