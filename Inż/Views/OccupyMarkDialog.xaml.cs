using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Inż.utils;
using Logic.Model;
using Logic.utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    /// Interaction logic for OccupyMarkDialog.xaml
    /// </summary>
    public partial class OccupyMarkDialog : Window
    {
        private readonly IIageSrc _src;
        private readonly Contour _contour;

        public OccupyMarkDialog(IIageSrc src,Contour contour)
        {
            _src = src;
            _contour = contour;
            InitializeComponent();
            Redraw();
        }

        private void Redraw()
        {
            var frame = _src.GetFrame();
            var mask = Gu.GetMask(_contour, frame.GetSizes(), new Scalar(150, 150, 150, 150));
            PreviewControl.Source = Gu.AddLayers(frame, mask).ToBitmapSource();
        }

        private void OccupiedControl_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void FreeControl_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OccupyMarkDialog_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                    case Key.D1:
                    case Key.NumPad1:
                    DialogResult = true;
                    break;
                    case Key.D2:
                    case Key.NumPad2:
                    DialogResult = false;
                    break;
            }
        }
    }
}
