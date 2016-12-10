using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PropertyChanged;

namespace PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class MainWindowVM 
    {
        public MainWindowVM()
        {
            string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp" };

            var files = Directory.EnumerateFiles(Properties.Settings.Default.DataSetPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .Select(path => new ImageVM(path));

            Images = new ObservableCollection<ImageVM>(files);
        }

        public ObservableCollection<ImageVM> Images { get; set; }

        public ImageVM SelectedImage { get; set; }
    }
}