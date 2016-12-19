using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PrepareData.Utils;
using PropertyChanged;

namespace PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowVM
    {
        public MainWindowVM()
        {
            string[] extensions = {".png", ".jpg", ".jpeg", ".bmp"};

            var files = Directory.EnumerateFiles(Properties.Settings.Default.DataSetPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .Select(path => new ImageVM(path));

            Images = new ObservableCollection<ImageVM>(files);

            SelectedImage = Images.FirstOrDefault();
            SelectedSlot = SelectedImage?.ParkingSlots.FirstOrDefault();

            AddContour = new RelayCommand<object>(AddSlotHandler, CanAddContour);
            DeleteContour = new RelayCommand<ParkingSlotVM>(DeleteSlotHandler);
            SaveToFile = new RelayCommand<object>(SaveToFileHandler);
            MarkEmptySlot = new RelayCommand<ParkingSlotVM>(o => MarkSlot(o,occupied: false));
            MarkOccupiedSlot = new RelayCommand<ParkingSlotVM>(o => MarkSlot(o,occupied: true));
        }

       


        public ObservableCollection<ImageVM> Images { get; set; }

        public ImageVM SelectedImage { get; set; }
        public ParkingSlotVM SelectedSlot { get; set; }


        public RelayCommand<object> AddContour { get; }
        public RelayCommand<ParkingSlotVM> DeleteContour { get; }

        public RelayCommand<object> SaveToFile { get; }

        public RelayCommand<ParkingSlotVM> MarkEmptySlot { get; }

        public RelayCommand<ParkingSlotVM> MarkOccupiedSlot { get; }

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
            //todo implement me 
            return;
        }

        private void MarkSlot(ParkingSlotVM slotVM, bool occupied)
        {
            if (slotVM == null) return;
            slotVM.IsOccupied = occupied;
        }
    }
}