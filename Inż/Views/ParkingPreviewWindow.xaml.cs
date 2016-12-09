using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CsvHelper;
using Inż.utils;
using LiteDB;
using Logic;
using Logic.Classifiers;
using Logic.Model;
using Logic.utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;
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
        private readonly IIageSrc _camera;
        private readonly IClassifier _svm;

        public ParkingPreviewWindow(DbContext db, IIageSrc camera, IClassifier svm)
        {
            _db = db;
            _camera = camera;
            _svm = svm;
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
            //frame = frame.FastNlMeansDenoisingColored(3, 10);

            Mat edges = frame.DetectEdges()
                .CvtColor(ColorConversionCodes.GRAY2BGR);
            var masks =
                _db.Contours.FindAll()
                    .Where(c => c.Pts.Any())
                    .Select(contour => new {contour, pred = _svm.Predict(frame.CalculateFeatures(contour))})
                    .Select(
                        a =>
                            Gu.GetMask(a.contour, frame.GetSizes(),
                                a.pred == 1 ? Scalar.Red : Scalar.Blue))
                    .ToList();

            masks.Insert(0, (int) ImgTypeSlider.Value == 0 ? frame : edges);
            ImagePreview.Source = Gu.AddLayers(masks.ToArray()).ToBitmapSource();
        }


        private void EditCounturButton_OnClick(object sender, RoutedEventArgs e)
        {
            var counturEditorWindow = IoC.Resolve<CounturEditorWindow>();
            counturEditorWindow.Contours = _db.Contours.FindAll().ToList();
            if (counturEditorWindow.ShowDialog() == true)
            {
                _db.Contours
                    .FindAll()
                    .Select(c => c.Id)
                    .ToList()
                    .ForEach(id => _db.Contours.Delete(id));

                _db.Contours.Insert(counturEditorWindow.Contours);
            }
        }

       
    }
}