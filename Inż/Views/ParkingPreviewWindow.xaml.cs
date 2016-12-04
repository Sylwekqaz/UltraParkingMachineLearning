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
using Inż.Model;
using Inż.utils;
using LiteDB;
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
        private SVM _svm;
        private SVMPreview _svmPreview;

        public ParkingPreviewWindow(DbContext db, IIageSrc camera)
        {
            _db = db;
            _camera = camera;
            InitializeComponent();
            _initializing = false;

            BuildModel();

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
                    .Select(contour =>
                    {
                        float[] sample =
                        {
                            (float) Gu.SaturationTreshold(contour, frame),
                            (float) Gu.EdgeTreshold(contour, frame)
                        };
                        return new {contour, mat = new Mat(1, 2, MatType.CV_32FC1, sample)};
                    })
                    .Select(arg => new { arg.contour, pred = _svm.Predict(arg.mat)})
                    .Select(
                        a => 
                            Gu.GetMask(a.contour, frame.GetSizes(),
                                a.pred ==1? Scalar.Red : Scalar.Blue))
                    .ToList();

            masks.Insert(0, (int) ImgTypeSlider.Value == 0 ? frame : edges);
            ImagePreview.Source = Gu.AddLayers(masks.ToArray()).ToBitmapSource();


            var points = _db.Contours.FindAll()
                .Where(c => c.Pts.Any())
                .Select(contour => new Point2f()
                {
                    X= (float) Gu.SaturationTreshold(contour, frame),
                    Y = (float) Gu.EdgeTreshold(contour, frame)
                }).ToList();
            _svmPreview.PreviewPoints(points);
        }


        private void EditCounturButton_OnClick(object sender, RoutedEventArgs e)
        {
            var counturEditorWindow = IoC.Resolve<CounturEditorWindow>();
            counturEditorWindow.Contours = _db.Contours.FindAll().ToList();
            if (counturEditorWindow.ShowDialog() == true)
            {
                _db.Contours
                    .FindAll()
                    .Select(c=>c.Id)
                    .ToList()
                    .ForEach(id => _db.Contours.Delete(id));

                _db.Contours.Insert(counturEditorWindow.Contours);
            }
        }

        private void BuildModel()
        {
            Point2f[] points;
            int[] responses;
            using (var csv = new CsvReader(new StreamReader(@"..\..\Images\DataSet\features.csv")))
            {
                List<Point2f> p = new List<Point2f>();
                List<int> r = new List<int>();
                csv.Configuration.Delimiter = ";";
                while (csv.Read())
                {
                    p.Add(new Point2f()
                    {
                        X = csv.GetField<float>(0),
                        Y = csv.GetField<float>(1)
                    });
                    r.Add(csv.GetField<bool>(2) ? 1 : 2);
                }
                points = p.ToArray();
                responses = r.ToArray();
            }
            var dataMat = new Mat(points.Length, 2, MatType.CV_32FC1, points);
            var resMat = new Mat(responses.Length, 1, MatType.CV_32SC1, responses);

            _svm = SVM.Create();
            _svm.Type = SVM.Types.CSvc;
            _svm.KernelType = SVM.KernelTypes.Rbf;
            _svm.TermCriteria = TermCriteria.Both(1000, 0.000001);
            _svm.Degree = 100.0;
            _svm.Gamma = 100.0;
            _svm.Coef0 = 1.0;
            _svm.C = 1.0;
            _svm.Nu = 0.5;
            _svm.P = 0.1;

            _svm.Train(dataMat, SampleTypes.RowSample, resMat);
            _svmPreview = new SVMPreview(_svm, points, responses);
            _svmPreview.Show();
        }
    }
}