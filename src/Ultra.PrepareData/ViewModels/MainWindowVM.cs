using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PropertyChanged;
using Ultra.Contract.Model;
using Ultra.PrepareData.Utils;

namespace Ultra.PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowVM
    {
        public MainWindowVM()
        {
            LoadFromFile(Properties.Settings.Default.DataSetPath);

            AddContour = new RelayCommand<object>(AddSlotHandler, CanAddContour);
            DeleteContour = new RelayCommand<ParkingSlotVM>(DeleteSlotHandler);
            SaveToFile = new RelayCommand<object>(SaveToFileHandler);
            MarkEmptySlot = new RelayCommand<ParkingSlotVM>(o => MarkSlot(o, occupied: false));
            MarkOccupiedSlot = new RelayCommand<ParkingSlotVM>(o => MarkSlot(o, occupied: true));
            MoveNext = new RelayCommand<object>(o =>
            {
                var i = Images.IndexOf(SelectedImage);
                SelectedImage = Images[++i % Images.Count];
            });
            MovePrev = new RelayCommand<object>(o =>
            {
                var i = Images.IndexOf(SelectedImage);
                SelectedImage = Images[(--i + Images.Count) % Images.Count];
            });
        }


        public ObservableCollection<ImageVM> Images { get; set; }

        public ImageVM SelectedImage { get; set; }
        public ParkingSlotVM SelectedSlot { get; set; }


        public RelayCommand<object> AddContour { get; }
        public RelayCommand<ParkingSlotVM> DeleteContour { get; }

        public RelayCommand<object> SaveToFile { get; }

        public RelayCommand<ParkingSlotVM> MarkEmptySlot { get; }

        public RelayCommand<ParkingSlotVM> MarkOccupiedSlot { get; }

        public RelayCommand<object> MoveNext { get; }
        public RelayCommand<object> MovePrev { get; }


        private void AddSlotHandler(object o)
        {
            SelectedImage.ParkingSlots.Add(new ParkingSlotVM());
            SelectedSlot = SelectedImage.ParkingSlots.LastOrDefault();
        }

        private void DeleteSlotHandler(ParkingSlotVM parkingSlotVM)
        {
            var index = SelectedImage.ParkingSlots.IndexOf(parkingSlotVM);
            SelectedImage.ParkingSlots.Remove(parkingSlotVM);
            SelectedSlot = index == -1
                ? null
                : SelectedImage.ParkingSlots.Count <= index
                    ? SelectedImage.ParkingSlots.LastOrDefault()
                    : SelectedImage.ParkingSlots[index];
        }

        private bool CanAddContour(object o)
        {
            return SelectedImage != null;
        }

        private void SaveToFileHandler(object o)
        {
            foreach (var imageVM in Images)
            {
                var data = imageVM.ParkingSlots
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

                var json = JsonConvert.SerializeObject(data);
                File.WriteAllText(imageVM.JsonPath, json);
            }
        }

        private void LoadFromFile(string directoryPath)
        {
            string[] extensions = {".png", ".jpg", ".jpeg", ".bmp"};

            var files = Directory.EnumerateFiles(directoryPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .Select(path => new ImageVM(path));

            Images = new ObservableCollection<ImageVM>(files);

            SelectedImage = Images.FirstOrDefault();
            SelectedSlot = SelectedImage?.ParkingSlots?.FirstOrDefault();
        }

        private void MarkSlot(ParkingSlotVM slotVM, bool occupied)
        {
            if (slotVM == null) return;
            slotVM.IsOccupied = occupied;
        }
    }
}