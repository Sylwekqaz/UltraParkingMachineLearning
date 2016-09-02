using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace ContourMaker
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                ReinitializeMats();
                PolysList = new List<List<Point>>();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(1000);
                dispatcherTimer.Start();
                Redraw(full: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Redraw();
        }

        DispatcherTimer dispatcherTimer = new DispatcherTimer(); // get progress every second
        private FrameSource frameSourceCamera;
        private int[] Sizes { get; set; }

        public Mat BackgroundBitmap { get; set; }
        public Mat MaskBitmap { get; set; }
        public Mat TempBitmap { get; set; }
        public List<List<Point>> PolysList { get; set; }


        private void ReinitializeMats()
        {
            BackgroundBitmap = new Mat();
            frameSourceCamera = Cv2.CreateFrameSource_Camera(1);

            frameSourceCamera.NextFrame(BackgroundBitmap);
            Sizes = new[] {BackgroundBitmap.Size(0), BackgroundBitmap.Size(1)};
            MaskBitmap = new Mat(Sizes, BackgroundBitmap.Type(), new Scalar(0, 0, 0, 0));
            TempBitmap = new Mat(Sizes, BackgroundBitmap.Type(), new Scalar(0, 0, 0, 0));
        }

        private void Redraw(bool full = false)
        {
            if (full)
            {
                #region redrawMask

                MaskBitmap = new Mat(Sizes, BackgroundBitmap.Type(), new Scalar(0, 0, 0, 0));
                var pts = PolysList.Where(list => list.Count > 0);
                Cv2.FillPoly(MaskBitmap, pts, new Scalar(150, 150, 150, 150));

                #endregion
            }

            frameSourceCamera.NextFrame(BackgroundBitmap);
            var tempMat = new Mat();
            var tempMat2 = new Mat();
            Cv2.Add(BackgroundBitmap, MaskBitmap, tempMat);
            Cv2.Add(tempMat, TempBitmap, tempMat2);

            PreviewImage.Source = tempMat2.ToBitmapSource();
        }

        private void PreviewImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PolysList.Count == 0)
            {
                PolysList.Add(new List<Point>());
            }

            Point position = new Point()
            {
                X = (int) (e.GetPosition(PreviewImage).X*BackgroundBitmap.Width/PreviewImage.ActualWidth),
                Y = (int) (e.GetPosition(PreviewImage).Y*BackgroundBitmap.Height/PreviewImage.ActualHeight),
            };
            PolysList.Last().Add(position);
            Redraw(full: true);
        }

        private void AddCountur_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PolysList.Add(new List<Point>());
        }

        private void CleatLastPoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var lastSet = PolysList.Last();
            if (!lastSet.Any()) return;
            lastSet.RemoveAt(lastSet.Count - 1);
            Redraw(full: true);
        }

        private void CleatLastCountur_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!PolysList.Any()) return;
            PolysList.RemoveAt(PolysList.Count - 1);
            Redraw(full: true);
        }
    }
}