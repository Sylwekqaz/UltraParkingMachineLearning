using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PropertyChanged;
using Ultra.Contract.Model;
using Ultra.IO;

namespace Ultra.PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class ImageVM
    {
        public ImageVM(string imagePath)
        {
            ImagePath = imagePath;
            var slots = FeatureLoader.LoadSlots(imagePath).Select(ps => new ParkingSlotVM(ps));
            ParkingSlots = new ObservableCollection<ParkingSlotVM>(slots);
        }

        public string ImagePath { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(ImagePath);
        public string JsonPath => Path.ChangeExtension(ImagePath, ".json");
        public ObservableCollection<ParkingSlotVM> ParkingSlots { get; set; }
    }
}