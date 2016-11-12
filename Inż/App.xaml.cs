using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Inż.Model;
using Inż.utils;
using Inż.Views;
using LiteDB;
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
            mainWindow.Show();

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
                .ToMethod(context => new ImageSrc(@"C:\Users\Sylwekqaz\Pictures\Camera Roll\WIN_20160902_18_56_36_Pro.jpg"))
                .InTransientScope();

            kernel.Bind<CounturEditorWindow>().ToSelf();
            kernel.Bind<ParkingPreviewWindow>().ToSelf();

            IoC.Initialize(kernel);
        }
    }
}
