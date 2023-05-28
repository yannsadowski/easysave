using EasySave.lib.Models;
using EasySave.lib.Services;
using EasySave.lib.Services.Server;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace EasySave.DesktopApp.ViewModels
{
    public class MainWindowViewModel
    {
        //private Model _Model = Model.GetInstance();
        public ObservableCollection<SaveWorkModel> SaveWork { get; } = new ObservableCollection<SaveWorkModel>();
        public SaveWorkManager _SaveWorkManager = SaveWorkManager.GetInstance();

        public GenerateKey _GenerateKey = new GenerateKey();
        public SaveWorkService SaveWorkService = new SaveWorkService();
        public Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        // peut être à utiliser pour les socket
        
        public Server Server = new Server();
        public SaveWorkModel _SaveWorkModel;

        //public MainWindowViewModel(Window currentWindow)
        //{
        //    _currentWindow = currentWindow;
        //}
        //public ICommand ShowDialogCommand => new RelayCommand(ShowDialog);

        public void GetData()
        {
            Application.Current.Dispatcher.Invoke( () =>
            {
                SaveWork.Clear();
                foreach (SaveWorkModel model in GetSaveWorks())
                {
                    // peut être à utiliser pour les socket
                    //model.PercentageChange += PercentageChangeEvent;
                    SaveWork.Add(model);
                }

            });
            
        }
  

        private void PercentageChangeEvent(object sender, float percentage)
        {
            Debug.Write(percentage);
        }

        public void OpenSocket()
        {
            Server.Start();
        }

        public int GenerateNewKey()
        {
            return _GenerateKey.Generate();
        }

        //public SaveWorkService SaveWorkCreator(SaveWorkModel AttributsForSaveWork)
        //{
        //    return _SaveWorkManager.SaveWorkCreator(AttributsForSaveWork);
        //}
        public List<SaveWorkModel> GetSaveWorks()
        {
            Debug.WriteLine(_SaveWorkManager.ArrayOfSaveWork);
            return _SaveWorkManager.ArrayOfSaveWork;
        }

        /// <summary>
        /// Adds a SaveWork as an instance and saves it in a JSON file
        /// </summary>
        /// <param name="AttributsForSaveWork"></param>
        /// <returns></returns>
        public void AddNewSaveWork(SaveWorkModel AttributsForSaveWork)
        {
            _SaveWorkManager.AddNewSaveWork(AttributsForSaveWork);
            GetData();
        }

        /// <summary>
        /// Adds a SaveWork as an instance and saves it in a JSON file
        /// </summary>
        /// <param name="AttributsForSaveWork"></param>
        /// <returns></returns>
        public void UpdateSaveWork(SaveWorkModel AttributsForSaveWork, SaveWorkModel OldSaveWork)
        {
             _SaveWorkManager.UpdateSaveWork(AttributsForSaveWork, OldSaveWork);
            GetData();
        }

        /// <summary>
        /// Instance all save works from the json files
        /// </summary>
        public void SaveWorkInitializing()
        {
            _SaveWorkManager.SaveWorkInitializing();
        }

        public void RemoveSaveWorkWPF(SaveWorkModel _SaveWork)
        {
             _SaveWorkManager.RemoveSaveWorkWPF(_SaveWork);
            GetData();
        }

        //public int ExecuteSaveWork(string SaveWorkID)
        //{
        //    return _SaveWorkManager.ExecuteSaveWork(SaveWorkID);
        //}

        public void ExecuteSaveWorkWPF(SaveWorkModel _SaveWork)
        {

            _SaveWorkManager.ExecuteSaveWorkWPF(_SaveWork);
            
        }

        //public int SequentialSaveWorksExecution()
        //{
        //    return _SaveWorkManager.SequentialSaveWorksExecution(_Model.ArrayOfSaveWork);
        //}

        public bool CheckRunningProcess(string ProcessName)
        {
            while (_SaveWorkManager.CheckRunningProcess(ProcessName))
            {
                
                return true;
            }
            return false;
        }

        public void LaunchAllCommand()
        {
            
            foreach (SaveWorkModel _saveWork in _SaveWorkManager.ArrayOfSaveWork)
            {
                while (CheckRunningProcess(ConfigurationManager.AppSettings["RunningProcess"]) == true)
                {
                    MessageBox.Show(currentWindow,"Un logiciel métier est actif");
                }
                ExecuteSaveWorkWPF(_saveWork);
            }
        }

        public void LaunchCommand(SaveWorkModel model)
        {
            
            while (CheckRunningProcess(ConfigurationManager.AppSettings["RunningProcess"]) == true)
            {
                MessageBox.Show(currentWindow,"Un logiciel métier est actif");
            }

            ExecuteSaveWorkWPF(model);
        }

        public void pauseSaveWork(SaveWorkModel model)
        {
            SaveWorkService.PauseSaveWork(model);
        }
        public void resumeSaveWork(SaveWorkModel model)
        {
            SaveWorkService.ResumeSaveWork(model);
        }
        public void stopSaveWork(SaveWorkModel model)
        {
            SaveWorkService.StopSaveWork(model);
        }
    }
}