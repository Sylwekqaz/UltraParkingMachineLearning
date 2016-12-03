using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    /// Interaction logic for SVMPreview.xaml
    /// </summary>
    public partial class SVMPreview : Window
    {
        private readonly SVM _svm;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private List<Tuple<double, double, bool>> doubles;

        public SVMPreview(SVM svm, List<Tuple<double, double, bool>> doubles)
        {
            _svm = svm;
            this.doubles = doubles;

            InitializeComponent();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += Draw;
            backgroundWorker.RunWorkerAsync();
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarControl.Value = e.ProgressPercentage;
        }


        private void Draw(object sender, DoWorkEventArgs e)
        {
            
            var image = Mat.Zeros(30, 50, MatType.CV_8UC3).ToMat();
            var matIndexer = image.GetGenericIndexer<Vec3b> ();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var vec = new Mat(new[] {1, 2}, MatType.CV_32FC1, new[] { x/100.0, y / 100.0});
                    var predict = _svm.Predict(vec);
                    Debug.WriteLine(predict);
                    matIndexer[y, x] = predict == 1 ? new Vec3b(255, 0, 0) : new Vec3b(0, 0, 255); 
                }
                backgroundWorker.ReportProgress(100*(y+1)/image.Height);
            }

            foreach (var tuple in doubles)
            {
                matIndexer[ (int) (tuple.Item2*100), (int)(tuple.Item1 * 100)] = tuple.Item3 ? new Vec3b(255, 255, 255) : new Vec3b(0, 0, 0);
            }

            ImageControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                var imageSource = image
                    //.CvtColor(ColorConversionCodes.GRAY2BGR)
                    .ToBitmapSource();
                ImageControl.Source = imageSource;
            }));
        }
    }
}
