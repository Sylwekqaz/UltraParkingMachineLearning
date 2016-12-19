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
        }

      

        public ObservableCollection<ImageVM> Images { get; set; }

        public ImageVM SelectedImage { get; set; }
        public ParkingSlotVM SelectedSlot { get; set; }


        public RelayCommand<object> AddContour { get; }
        public RelayCommand<ParkingSlotVM> DeleteContour { get; }

        private void AddSlotHandler(object o)
        {
            SelectedImage.ParkingSlots.Add(new ParkingSlotVM());
        }

        private void DeleteSlotHandler(ParkingSlotVM parkingSlotVM)
        {
            SelectedImage.ParkingSlots.Remove(parkingSlotVM);
        }

        private bool CanAddContour(object o)
        {
            return SelectedImage != null;
        }
    }
}