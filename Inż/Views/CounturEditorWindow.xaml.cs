using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Inż.utils;
using Logic.Model;
using Logic.utils;
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

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly IIageSrc _camera;

        public CounturEditorWindow(IIageSrc camera)
        {
            _camera = camera;
            InitializeComponent();
            _initializing = false;
            Contours = new List<Contour>();

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

            
            var mask = Gu.GetMask(Contours, frame.GetSizes(), new Scalar(150, 150, 150, 150));
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

        public List<Contour> Contours { get; set; }


        private void AddCountur_Click(object sender, RoutedEventArgs e)
        {
            Contours.Add(new Contour());
        }

        private void CleatLastPoint_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = Contours.LastOrDefault();
            if (!(lastSet?.Pts?.Any() ?? false)) return;
            lastSet.Pts.RemoveAt(lastSet.Pts.Count - 1);
        }

        private void CleatLastCountur_Click(object sender, RoutedEventArgs e)
        {
            var lastSet = Contours.LastOrDefault();
            if (lastSet != null) Contours.Remove(lastSet);
        }

        private void PreviewImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Contours.Any())
            {
                Contours.Add(new Contour());
            }

            var position = e.GetPointRelativeToSource((Image) sender);
            var contour = Contours.Last();
            contour.Pts.Add(position);
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}