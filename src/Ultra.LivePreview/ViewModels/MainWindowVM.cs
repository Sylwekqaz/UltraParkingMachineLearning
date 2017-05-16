using System;
using System.Collections.ObjectModel;
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
using Ultra.PrepareData.Utils;
using Ultra.PrepareData.ViewModels;

namespace Ultra.LivePreview.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowVM
    {
        public MainWindowVM()
        {
            AddContour = new RelayCommand<object>(AddSlotHandler);
            DeleteContour = new RelayCommand<ParkingSlotVM>(DeleteSlotHandler);
            SaveToFile = new RelayCommand<object>(SaveToFileHandler);

            Camera = Cv2.CreateFrameSource_Camera(0);

            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(1000);
            _dispatcherTimer.Start();
        }

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(); // get progress every second
        public FrameSource Camera { get; set; }
        public BitmapSource CameraFrame { get; set; }



        public ObservableCollection<ParkingSlotVM> ParkingSlots { get; set; }
        public ParkingSlotVM SelectedSlot { get; set; }


        public RelayCommand<object> AddContour { get; }
        public RelayCommand<ParkingSlotVM> DeleteContour { get; }

        public RelayCommand<object> SaveToFile { get; }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var frame = new Mat();
            Camera.NextFrame(frame);
            CameraFrame = frame.ToBitmapSource();
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
//            foreach (var imageVM in Images)
//            {
//                var data = imageVM.ParkingSlots
//                    .Select(slot => new ParkingSlot()
//                    {
//                        IsOccupied = slot.IsOccupied,
//                        Contour = new Contour(slot.Pts
//                            .Select(point => new Contour.Point()
//                            {
//                                X = point.X,
//                                Y = point.Y,
//                            }))
//                    });
//
//                FeatureLoader.SaveSlots(imageVM.ImagePath,data);
//            }
        }

        public void LoadFromFile(string directoryPath)
        {
//            var files = FeatureLoader.GetPhotos(directoryPath)
//                .Select(path => new ImageVM(path));
//
//            Images = new ObservableCollection<ImageVM>(files);
//
//            SelectedImage = Images.FirstOrDefault();
//            SelectedSlot = SelectedImage?.ParkingSlots?.FirstOrDefault();
        }
    }
}