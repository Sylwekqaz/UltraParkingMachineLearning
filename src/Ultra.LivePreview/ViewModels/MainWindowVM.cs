using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using PropertyChanged;
using Ultra.Contract.Model;
using Ultra.IO;
using Ultra.MachineLearning;
using Ultra.MachineLearning.Classifiers;
using Ultra.PrepareData.Utils;
using Ultra.PrepareData.ViewModels;

namespace Ultra.LivePreview.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowVM
    {
        private const string ContourDataPath = @"ContourData.jpg";

        public MainWindowVM()
        {
            AddContour = new RelayCommand<object>(AddSlotHandler, o => ParkingSlots != null);
            DeleteContour = new RelayCommand<ParkingSlotVM>(DeleteSlotHandler);
            SaveToFile = new RelayCommand<object>(SaveToFileHandler);
            MakeScreenShot = new RelayCommand<object>(MakeScreenShotHandler, _ => !string.IsNullOrEmpty(TrainDataPath));

            LoadContours();

            Camera = Cv2.CreateFrameSource_Camera(0);

            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(1000);
            _dispatcherTimer.Start();
        }

        private void MakeScreenShotHandler(object o1)
        {
            var frame = new Mat();
            Camera.NextFrame(frame);
            frame.SaveImage(Path.Combine(TrainDataPath, $@"{Guid.NewGuid()}.jpg"));
        }

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        public FrameSource Camera { get; set; }
        public BitmapSource CameraFrame { get; set; }


        public ObservableCollection<ParkingSlotVM> ParkingSlots { get; set; }
        public ParkingSlotVM SelectedSlot { get; set; }


        public RelayCommand<object> AddContour { get; }
        public RelayCommand<ParkingSlotVM> DeleteContour { get; }

        public RelayCommand<object> SaveToFile { get; }
        public RelayCommand<object> MakeScreenShot { get; }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var frame = new Mat();
            Camera.NextFrame(frame);
            CameraFrame = frame.ToBitmapSource();

            if (SvmClassifier == null)
                return;
            if (!IsStarted)
                return;

            foreach (var slot in ParkingSlots)
            {
                var contour = new Contour(slot.Pts
                    .Select(point => new Contour.Point()
                    {
                        X = point.X,
                        Y = point.Y,
                    }));
                var features = frame.CalculateFeatures(contour, false);
                Debug.WriteLine(features);
                slot.IsOccupied = SvmClassifier.Predict(features);
            }
        }


        private void AddSlotHandler(object o)
        {
            ParkingSlots.Add(new ParkingSlotVM());
            SelectedSlot = ParkingSlots.LastOrDefault();
        }

        private void DeleteSlotHandler(ParkingSlotVM parkingSlotVM)
        {
            var index = ParkingSlots.IndexOf(parkingSlotVM);
            ParkingSlots.Remove(parkingSlotVM);
            SelectedSlot = index == -1
                ? null
                : ParkingSlots.Count <= index
                    ? ParkingSlots.LastOrDefault()
                    : ParkingSlots[index];
        }

        public void SaveToFileHandler(object o)
        {
            var data = ParkingSlots
                .Select(slot => new ParkingSlot()
                {
                    IsOccupied = slot.IsOccupied,
                    Contour = new Contour(slot.Pts
                        .Select(point => new Contour.Point()
                        {
                            X = point.X,
                            Y = point.Y,
                        }))
                });

            FeatureLoader.SaveSlots(ContourDataPath, data);
        }

        public void LoadContours()
        {
            var parkingSlots = FeatureLoader.LoadSlots(ContourDataPath);
            ParkingSlots = new ObservableCollection<ParkingSlotVM>(parkingSlots.Select(ps => new ParkingSlotVM(ps)));
            SelectedSlot = ParkingSlots?.FirstOrDefault();
        }

        public void LoadTrainData(string trainDataPath)
        {
            TrainDataPath = trainDataPath;
            if (!Directory.GetFiles(trainDataPath).Any())
                return;

            var features = FeatureLoader.GetObservations(trainDataPath);
            SvmClassifier = SVMClassifier.Create(features);
        }

        private string TrainDataPath { get; set; }
        private SVMClassifier SvmClassifier { get; set; }

        public bool IsStarted { get; set; }
    }
}