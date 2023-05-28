using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using EasySave.DesktopApp.Resources;

namespace EasySave.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string LockFileName = "EasySave.DesktopApp";
        private FileStream lockFile;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LockFileName));
            string lockFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LockFileName);
            try
            {
                lockFile = new FileStream(lockFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            }
            catch (IOException)
            {
                MessageBox.Show($"{langage.MonoInstance}", $"{langage.Error}", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            // Ajoutez du code ici.
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            lockFile.Close();
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LockFileName));
        }


    }
}