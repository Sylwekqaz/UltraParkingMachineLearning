using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WinForms = System.Windows.Forms;
using Ultra.PrepareData.ViewModels;

namespace Ultra.LivePreview.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData(askForSave: false);
            SetScale();
        }


        private void LoadDataMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData(bool askForSave = true)
        { 
            if (askForSave)
            {
                var result = MessageBox.Show("Czy chcesz zapisać dane", "Możliwa utrata danych", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        ((MainWindowVM) DataContext).SaveToFileHandler(null);
                        break;
                    case MessageBoxResult.No:
                        //nothing to do, just continue
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            using (var fbd = new WinForms.FolderBrowserDialog() {SelectedPath = Directory.GetCurrentDirectory()})
            {
                if (fbd.ShowDialog() == WinForms.DialogResult.OK)
                {
                    ((ViewModels.MainWindowVM) DataContext).LoadTrainData(fbd.SelectedPath);
                }
            }
        }

        private void SetScale()
        {
            var child = VisualTreeHelper.GetChild(Viewbox, 0) as ContainerVisual;
            var scaleTransform = child?.Transform as ScaleTransform;

            Manipulator.Scale = scaleTransform?.ScaleX ?? 1;

        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetScale();
            Task.Delay(1000).ContinueWith(_ => Dispatcher.Invoke(SetScale));
        }
    }
}
