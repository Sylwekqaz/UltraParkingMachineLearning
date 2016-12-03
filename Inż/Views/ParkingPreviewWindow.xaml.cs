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
                    .Select(contour => new {contour, saturation =Gu.SaturationTreshold(contour, frame) ,treshold = Gu.EdgeTreshold(contour, frame)})
                    //.Select(arg => {Debug.WriteLine($"Sat: {arg.saturation} \tEdge: {arg.treshold}");return arg;})
                    .Select(arg => new { arg.contour, mat = new Mat(new []{1,2},MatType.CV_32FC1,new []{ arg.saturation,arg.treshold})})
                    .Select(arg => new { arg.contour, pred = _svm.Predict(arg.mat)})
                    .Select(
                        a => 
                            Gu.GetMask(a.contour, frame.GetSizes(),
                                a.pred >0? Scalar.Red : Scalar.Blue))
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
                    .Select(c=>c.Id)
                    .ToList()
                    .ForEach(id => _db.Contours.Delete(id));

                _db.Contours.Insert(counturEditorWindow.Contours);
            }
        }

        private void BuildModel()
        {
            var csv = new CsvReader(new StreamReader(@"..\..\Images\DataSet\features.csv"));
            csv.Configuration.Delimiter = ";";
            var trainData = new List<Tuple<double, double, bool>>();
            while (csv.Read())
            {
                var tuple = new Tuple<double, double, bool>(csv.GetField<double>(0), csv.GetField<double>(1),
                    csv.GetField<bool>(2));
                trainData.Add(tuple);
            }

            var labels = new Mat(new[] {trainData.Count, 1}, MatType.CV_32SC1,
                trainData.Select(t => t.Item3 ? 1 : 0).ToArray());

            double[,] doubles = new double[trainData.Count,2];
            for (int i = 0; i < doubles.GetLength(0); i++)
            {
                doubles[i, 0] = trainData[i].Item1;
                doubles[i, 1] = trainData[i].Item2;
            }

            var trainingMat = new Mat(new[] {trainData.Count, 2}, MatType.CV_32FC1,
                doubles);

            _svm = SVM.Create();
            _svm.Type = SVM.Types.CSvc;
            _svm.C = 0.1;
            _svm.KernelType = SVM.KernelTypes.Linear;

            _svm.Train(trainingMat, SampleTypes.RowSample, labels);
            var svmPreview = new SVMPreview(_svm, trainData);
            svmPreview.Show();
        }
    }
}