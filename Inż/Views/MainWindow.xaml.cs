using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Inż.Model;
using Newtonsoft.Json;
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

        private List<List<Point>> PolysList = new List<List<Point>>();

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
                var pts = PolysList.Where(list => list.Count > 0);
                Cv2.FillPoly(subset.Mask, pts, new Scalar(150, 150, 150, 150));

                #endregion
            }

            var tempMat = new Mat();
            Cv2.Add(subset[(int) ImgTypeSlider.Value], subset.Mask, tempMat);

            ImagePreview.Source = tempMat.ToBitmapSource();
        }

        private void AddCountur_Click(object sender, RoutedEventArgs e)
        {
            PolysList.Add(new List<Point>());
        }

        private void CleatLastPoint_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = PolysList.Last();
            if (!lastSet.Any()) return;
            lastSet.RemoveAt(lastSet.Count - 1);
            Redraw(true);
        }

        private void CleatLastCountur_Click(object sender, RoutedEventArgs e)
        {
            if (!PolysList.Any()) return;
            PolysList.RemoveAt(PolysList.Count - 1);
            Redraw(true);
        }

        private void PreviewImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PolysList.Count == 0)
            {
                PolysList.Add(new List<Point>());
            }

            var position = new Point
            {
                X = (int) (e.GetPosition(ImagePreview).X*subset.Org.Width/ImagePreview.ActualWidth),
                Y = (int) (e.GetPosition(ImagePreview).Y*subset.Org.Height/ImagePreview.ActualHeight)
            };
            PolysList.Last().Add(position);
            Redraw(true);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(PolysList.ToArray());
            File.WriteAllText(@"ctr.json", json);
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var r = new StreamReader("ctr.json"))
                {
                    var json = r.ReadToEnd();
                    PolysList = JsonConvert.DeserializeObject<List<List<Point>>>(json);
                }
                Redraw(true);
            }
            catch (Exception exception)
            {
            }
        }
    }
}