using System.IO;

namespace PrepareData.ViewModels
{
    public class ImageVM
    {
        public string ImagePath { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(ImagePath);
    }
}