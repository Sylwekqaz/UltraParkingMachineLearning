using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Inż.Model;
using Inż.utils;
using Inż.Views;
using LiteDB;
using Newtonsoft.Json;
using Ninject;
using OpenCvSharp;

namespace Inż
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeDi();

            var mainWindow = IoC.Resolve<ParkingPreviewWindow>();
            mainWindow.Show(); // hold app live

            SetContourOnImages(@"..\..\Images\DataSet\", "*.png");

           

            base.OnStartup(e);
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
                .ToMethod(context => new CameraSource(2))
                .InTransientScope();

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
    }
}
