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
    public partial class ParkingPreviewWindow : Window
    {
        private readonly bool _initializing = true;

        private readonly DbContext _db;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly FrameSource _camera = Cv2.CreateFrameSource_Camera(1);

        public ParkingPreviewWindow(DbContext db)
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

        private void EditCounturButton_OnClick(object sender, RoutedEventArgs e)
        {
            IoC.Resolve<CounturEditorWindow>().Show();
        }
    }
}