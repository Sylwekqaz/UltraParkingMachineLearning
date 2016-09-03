using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Inż.Model;
using Inż.utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly bool _initializing = true;

        private readonly DbContext _db;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly FrameSource _camera = Cv2.CreateFrameSource_Camera(1);

        public MainWindow(DbContext db)
        {
            _db = db;
            InitializeComponent();
            _initializing = false;

            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(1000);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_initializing) return;
            Redraw();
        }


        private void Redraw()
        {
            var frame = new Mat();
            _camera.NextFrame(frame);

            var pts = _db.Contours.FindAll().ToArray();
            var mask = Gu.GetMask(pts, frame.GetSizes());
            switch ((int) ImgTypeSlider.Value)
            {
                case 0:
                    ImagePreview.Source = Gu.AddLayers(frame, mask).ToBitmapSource();
                    break;
                case 1:
                    ImagePreview.Source = Gu.AddLayers(Gu.Canny(frame), mask).ToBitmapSource();
                    break;
            }
        }


        private void AddCountur_Click(object sender, RoutedEventArgs e)
        {
            _db.Contours.Insert(new Contour());
        }

        private void CleatLastPoint_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = _db.Contours.FindAll().LastOrDefault();
            if (!(lastSet?.Pts?.Any() ?? false)) return;
            lastSet.Pts.RemoveAt(lastSet.Pts.Count - 1);
            _db.Contours.Update(lastSet);
        }

        private void CleatLastCountur_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = _db.Contours.FindAll().LastOrDefault();
            if (lastSet != null) _db.Contours.Delete(lastSet.Id);
        }

        private void PreviewImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_db.Contours.Count() == 0)
            {
                _db.Contours.Insert(new Contour());
            }

            var position = e.GetPointRelativeToSource((Image) sender);
            var contour = _db.Contours.FindAll().Last();
            contour.Pts.Add(position);
            _db.Contours.Update(contour);
        }
    }
}