using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace ContourMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BackgroundMat = new Mat("Images/test1a.jpg");
            BackgroundImage.Source = BackgroundMat.ToBitmapSource();
        }

        public Mat BackgroundMat { get; set; }
        public Mat MaskMat { get; set; }
        public Mat TempMat { get; set; }

        private void TempImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}