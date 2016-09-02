using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Inż.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly bool _initializing = true;
        private FrameSource _frameSourceCamera;
        private ImageSubset _subset;
        DispatcherTimer dispatcherTimer = new DispatcherTimer(); // get progress every second

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _initializing = false;

                _frameSourceCamera = Cv2.CreateFrameSource_Camera(1);

                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(1000);
                dispatcherTimer.Start();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_initializing) return;
            Recalculate();
            ImagePreview.Source = _subset[(int)ImgTypeSlider.Value].ToBitmapSource();
        }

        private void Recalculate()
        {
            _subset = new ImageSubset();
            _frameSourceCamera.NextFrame(_subset.Org);

            Cv2.CvtColor(_subset.Org, _subset.Gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(_subset.Gray, _subset.Edges, ASlider.Value, BSlider.Value);
        }
    }
}