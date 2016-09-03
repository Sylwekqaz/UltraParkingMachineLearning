using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Inż.Model;
using Newtonsoft.Json;
using Ninject;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly bool _initializing = true;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly FrameSource frameSourceCamera = Cv2.CreateFrameSource_Camera(1);
        private readonly ImageSubset subset = new ImageSubset();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _initializing = false;

                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(1000);
                dispatcherTimer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Inject]
        public DbContext Db { get; set; }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_initializing) return;
            Recalculate();
            Redraw();
        }

        private void Recalculate()
        {
            frameSourceCamera.NextFrame(subset.Org);
            var temp = new Mat();
            Cv2.Canny(subset.Org, temp, 120, 150);
            Cv2.CvtColor(temp, subset.Edges, ColorConversionCodes.GRAY2BGR);
        }

        private void Redraw(bool full = false)
        {
            if (subset.Mask == null)
            {
                full = true;
            }
            if (full)
            {
                #region redrawMask

                var sizes = new[] {subset.Org.Size(0), subset.Org.Size(1)};
                subset.Mask = new Mat(sizes, subset.Org.Type(), new Scalar(0, 0, 0, 0));
                var pts = Db.Contours.FindAll()
                    .Where(c => c.Pts.Count > 0)
                    .Select(c => c.Pts.Select(p => new Point() {X = (int) p.X, Y = (int) p.Y}))
                    .ToArray();

                Cv2.FillPoly(subset.Mask, pts, new Scalar(150, 150, 150, 150));

                #endregion
            }

            var tempMat = new Mat();
            Cv2.Add(subset[(int) ImgTypeSlider.Value], subset.Mask, tempMat);

            ImagePreview.Source = tempMat.ToBitmapSource();
        }

        private void AddCountur_Click(object sender, RoutedEventArgs e)
        {
            Db.Contours.Insert(new Contour());
        }

        private void CleatLastPoint_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = Db.Contours.FindAll().Last().Pts;
            if (!lastSet.Any()) return;
            lastSet.RemoveAt(lastSet.Count - 1);
            Redraw(true);
        }

        private void CleatLastCountur_Click(object sender, RoutedEventArgs e)
        {
            if (Db.Contours.Count() == 0) return;
            var lastSet = Db.Contours.FindAll().Last();
            Db.Contours.Delete(lastSet.Id);
            Redraw(true);
        }

        private void PreviewImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Db.Contours.Count() == 0)
            {
                Db.Contours.Insert(new Contour());
            }

            var position = new Contour.Point
            {
                X = (int) (e.GetPosition(ImagePreview).X*subset.Org.Width/ImagePreview.ActualWidth),
                Y = (int) (e.GetPosition(ImagePreview).Y*subset.Org.Height/ImagePreview.ActualHeight)
            };
            var contour = Db.Contours.FindAll().Last();
            contour.Pts.Add(position);
            Db.Contours.Update(contour);

            Redraw(true);
        }
    }
}