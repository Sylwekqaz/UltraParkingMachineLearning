using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Logic.Model;
using Newtonsoft.Json;
using PropertyChanged;

namespace PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class ImageVM
    {
        public ImageVM(string imagePath)
        {
            ImagePath = imagePath;

            if (File.Exists(JsonPath))
            {
                ParkingSlots =
                    new ObservableCollection<ParkingSlot>(
                        JsonConvert.DeserializeObject<List<ParkingSlot>>(File.ReadAllText(JsonPath)));
            }
            else
            {
                ParkingSlots = new ObservableCollection<ParkingSlot>();
            }

        }

        public string ImagePath { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(ImagePath);
        public string JsonPath => Path.ChangeExtension(ImagePath, ".json");
        public ObservableCollection<ParkingSlot> ParkingSlots { get; set; }
    }
}