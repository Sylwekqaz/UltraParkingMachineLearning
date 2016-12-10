using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using Inż.utils;
using Inż.Views;
using LiteDB;
using Logic.Classifiers;
using Logic.Model;
using Logic.utils;
using Newtonsoft.Json;
using Ninject;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Inż
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            InitializeDi();

            SetContourOnImages(@"..\..\..\DataSet\", "*.png");
            SetOccupiedOnImages(@"..\..\..\DataSet\", "*.png");
            GetFeatures(@"..\..\..\DataSet\", "*.png", @"..\..\..\DataSet\features.csv");

            MainWindow = IoC.Resolve<ParkingPreviewWindow>();
            MainWindow.Show(); 

            base.OnStartup(e);
        }

        private void GetFeatures(string folderPath, string pattern ,string jsonlocation)
        {
            // sat edge isOcc
            using (var csv = new CsvWriter(new StreamWriter(jsonlocation, append: false)))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasHeaderRecord = true;

                var files = Directory.EnumerateFiles(folderPath, pattern);
                var imageFeatureses = files.Select(filePath =>
                {
                    var jsonFilePath = Path.ChangeExtension(filePath, ".json");
                    var json = File.ReadAllText(jsonFilePath);
                    var slots = JsonConvert.DeserializeObject<List<ParkingSlot>>(json);
                    var image = new Mat(filePath);
                    return new {slots, image};
                }).SelectMany(
                        arg => arg.slots.Select(slot => arg.image.CalculateFeatures(slot.Contour, slot.IsOccupied)));

                csv.WriteRecords(imageFeatureses);
                
            }
        }

        private static void InitializeDi()
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<LiteDatabase>()
                .ToMethod(context => new LiteDatabase("Inz.db"))
                .InSingletonScope();

            kernel.Bind<DbContext>()
                .ToSelf()
                .InSingletonScope();

            kernel.Bind<IIageSrc>()
                //.ToMethod(context => new ImageSrc(@"..\..\Images\test1a.jpg"))
                .ToMethod(context => new CameraSource(1))
                .InTransientScope();

            kernel.Bind<IClassifier>()
                .ToMethod(context =>
                {
                    List<ImageFeatures> imageFeatureses;
                    using (var csv = new CsvReader(new StreamReader(@"..\..\..\DataSet\features.csv")))
                    {
                        csv.Configuration.Delimiter = ";";
                        csv.Configuration.HasHeaderRecord = true;

                        imageFeatureses = csv.GetRecords<ImageFeatures>().ToList();
                    }

                    return SMVClassifier.Create(imageFeatureses);
                })
                .InSingletonScope();

            kernel.Bind<CounturEditorWindow>().ToSelf();
            kernel.Bind<ParkingPreviewWindow>().ToSelf();

            IoC.Initialize(kernel);
        }

        private void SetContourOnImages(string folderPath,string pattern)
        {
            var files = Directory.EnumerateFiles(folderPath, pattern);
            foreach (string filePath in files)
            {
                var window = new CounturEditorWindow(new ImageSrc(filePath));
                var jsonFilePath = Path.ChangeExtension(filePath, ".json");
                if (File.Exists(jsonFilePath))
                {
                    var text = File.ReadAllText(jsonFilePath);
                    window.Contours = JsonConvert.DeserializeObject<List<ParkingSlot>>(text)
                        .Select(ps=>ps.Contour)
                        .ToList();
                }
                if (window.ShowDialog() == true)
                {
                    var parkingSlots = window.Contours
                        .Select(c=>new ParkingSlot() {Contour = c})
                        .ToList();
                    var json = JsonConvert.SerializeObject(parkingSlots);
                    File.WriteAllText(jsonFilePath, json);
                }
            }
        }

        private void SetOccupiedOnImages(string folderPath, string pattern)
        {
            var files = Directory.EnumerateFiles(folderPath, pattern);
            foreach (string filePath in files)
            {
                var jsonFilePath = Path.ChangeExtension(filePath, ".json");
                var json = File.ReadAllText(jsonFilePath);
                var slots = JsonConvert.DeserializeObject<List<ParkingSlot>>(json);
                foreach (var slot in slots)
                {
                    var window = new OccupyMarkDialog(new ImageSrc(filePath),slot.Contour );
                    if (window.ShowDialog() == true)
                    {
                        slot.IsOccupied = window.DialogResult.Value;
                    }
                }
                json = JsonConvert.SerializeObject(slots);
                File.WriteAllText(jsonFilePath, json);
            }
        }
    }
}
