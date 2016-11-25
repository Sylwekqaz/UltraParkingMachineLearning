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
    ///     Interaction logic for CounturEditorWindow.xaml
    /// </summary>
    public partial class CounturEditorWindow : Window
    {
        private readonly bool _initializing = true;

        private readonly DbContext _db;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly IIageSrc _camera;

        public CounturEditorWindow(DbContext db, IIageSrc camera)
        {
            _db = db;
            _camera = camera;
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
            var frame= _camera.GetFrame();

            var pts = _db.Contours.FindAll().ToArray();
            var mask = Gu.GetMask(pts, frame.GetSizes(), new Scalar(150, 150, 150, 150));
            switch ((int) ImgTypeSlider.Value)
            {
                case 0:
                    ImagePreview.Source = Gu.AddLayers(frame, mask).ToBitmapSource();
                    break;
                case 1:
                    ImagePreview.Source = Gu.AddLayers(frame.DetectEdges().CvtColor(ColorConversionCodes.GRAY2BGR), mask).ToBitmapSource();
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