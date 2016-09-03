using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Inż.Model;
using Inż.Views;
using LiteDB;
using Ninject;

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

            var mainWindow = IoC.Resolve<CounturEditorWindow>();
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
                .InSingletonScope(); ;

            kernel.Bind<CounturEditorWindow>().ToSelf();
            kernel.Bind<ParkingPreviewWindow>().ToSelf();

            IoC.Initialize(kernel);
        }
    }
}
