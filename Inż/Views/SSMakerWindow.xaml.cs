using System;
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
    public partial class SSMakerWindow : Window
    {
        private readonly bool _initializing = true;

        private readonly DbContext _db;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        private readonly IIageSrc _camera;

        public SSMakerWindow(DbContext db, IIageSrc camera)
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
            var frame = _camera.GetFrame();

            switch ((int) ImgTypeSlider.Value)
            {
                case 0:
                    ImagePreview.Source = frame.ToBitmapSource();
                    break;
                case 1:
                    ImagePreview.Source = frame.DetectEdges().ToBitmapSource();
                    break;
            }
        }


        private void MakeSS_Click(object sender, RoutedEventArgs e)
        {
            _camera.GetFrame().SaveImage($@"..\..\Images\DataSet\{Guid.NewGuid()}.png");
        }
    }
}