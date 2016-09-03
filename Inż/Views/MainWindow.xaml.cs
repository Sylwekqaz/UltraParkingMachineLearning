using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Inż.Model;
using Inż.utils;
using Ninject;
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
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly FrameSource frameSourceCamera = Cv2.CreateFrameSource_Camera(1);

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
            Redraw();
        }


        private void Redraw(bool full = false)
        {
            var frame = new Mat();
            frameSourceCamera.NextFrame(frame);

            var pts = Db.Contours.FindAll().ToArray();
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
            Db.Contours.Insert(new Contour());
        }

        private void CleatLastPoint_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = Db.Contours.FindAll().Last();
            if (!lastSet.Pts.Any()) return;
            lastSet.Pts.RemoveAt(lastSet.Pts.Count - 1);
            Db.Contours.Update(lastSet);
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

            var position = e.GetPointRelativeToSource((Image) sender);
            var contour = Db.Contours.FindAll().Last();
            contour.Pts.Add(position);
            Db.Contours.Update(contour);

            Redraw(true);
        }
    }
}