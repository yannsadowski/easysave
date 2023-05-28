using EasySave.DesktopApp.ViewModels;
using EasySave.lib.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Data;
using System.Windows.Data;
using System.Threading;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySave.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Model _Model = Model.GetInstance();
        private MainWindowViewModel _ViewModel = new MainWindowViewModel();
        

        public MainWindow()
        {
            InitializeComponent();
            _ViewModel.GenerateNewKey();
            _ViewModel.SaveWorkInitializing();
            _ViewModel.GetData();
            DataContext = _ViewModel;
            

            _ViewModel.OpenSocket();

            //set ArrayOfSaveWork in datagrid
            dgSaveWorks.ItemsSource = _ViewModel.GetSaveWorks();
            
        }

        //                                                                           method for savework in datagrid
        public void RefreshSaveWorks()
        {
            dgSaveWorks.Items.Refresh();
        }

        public void deleteCommand(object sender, RoutedEventArgs e)
        {
            SaveWorkModel selectedSaveWork = dgSaveWorks.SelectedItem as SaveWorkModel;

            _ViewModel.RemoveSaveWorkWPF(selectedSaveWork);
            RefreshSaveWorks();
        }

        public void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkModel selectedSaveWork = dgSaveWorks.SelectedItem as SaveWorkModel;

            _ViewModel.RemoveSaveWorkWPF(selectedSaveWork);
            RefreshSaveWorks();
        }

        public void Pause_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.pauseSaveWork(dgSaveWorks.SelectedItem as SaveWorkModel);
        }

        public void Lancer_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkModel selectedSaveWork = dgSaveWorks.SelectedItem as SaveWorkModel;

            //_ViewModel.LaunchCommand(selectedSaveWork);
            if (selectedSaveWork.ProgressState == "Active")
            {
                _ViewModel.resumeSaveWork(selectedSaveWork);
            }
            else
            {

                _ViewModel.LaunchCommand(selectedSaveWork);
                
            }
            
        }

        public void Stop_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkModel selectedSaveWork = dgSaveWorks.SelectedItem as SaveWorkModel;
            _ViewModel.stopSaveWork(selectedSaveWork);
        }

        public void Visual_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkControlPanel NewsPage = new SaveWorkControlPanel();
            SaveWorkModel selectedSaveWork = dgSaveWorks.SelectedItem as SaveWorkModel;
            // Wait for the user to close the windows
            NewsPage.NameTextBox.Text = selectedSaveWork.NameSaveWork;
            NewsPage.TypeComboBox.SelectedIndex = selectedSaveWork.TypeSaveWork - 1;
            NewsPage.SourcePathTextBox.Text = selectedSaveWork.SourcePathSaveWork;
            NewsPage.DestinationPathTextBox.Text = selectedSaveWork.DestinationPathSaveWork;
            NewsPage.ShowDialog();
            //get the data the user set in the windows pop-up
            SaveWorkModel savedVariable = NewsPage.SaveWork;
            if (savedVariable != null)
            {
                _ViewModel.UpdateSaveWork(savedVariable, selectedSaveWork);
                dgSaveWorks.Items.Refresh();
            }
        }

        //                                                                           method for bouton select
        public void AddSaveWorkCommand(object sender, RoutedEventArgs e)
        {
            SaveWorkControlPanel NewsPage = new SaveWorkControlPanel();
            // Wait for the user to close the windows
            NewsPage.ShowDialog();

            //get the data the user set in the windows pop-up
            SaveWorkModel savedVariable = NewsPage.SaveWork;
            if (savedVariable != null)
            {
                _ViewModel.AddNewSaveWork(savedVariable);
                dgSaveWorks.Items.Refresh();
            }
        }

        public void generateCommand(object sender, RoutedEventArgs e)
        {
            _ViewModel.GenerateNewKey();
        }

        public void LaunchSelectedCommand(object sender, RoutedEventArgs e)
        {

            foreach (SaveWorkModel item in dgSaveWorks.SelectedItems)
            {
                _ViewModel.ExecuteSaveWorkWPF(item);
            }
            
        }

        public void selectAllCommand(object sender, RoutedEventArgs e)
        {
            dgSaveWorks.SelectAll();
        }

        public void pauseSelectedCommand(object sender, RoutedEventArgs e)
        {   // !!!!!!! avec multi-thread !!!!!!!!
            foreach (SaveWorkModel item in dgSaveWorks.SelectedItems)
            {
                _ViewModel.pauseSaveWork(item);
            }
        }

        public void stopSelectedCommand(object sender, RoutedEventArgs e)
        {   // !!!!!!! avec multi-thread !!!!!!!!!
            foreach (SaveWorkModel item in dgSaveWorks.SelectedItems)
            {
                _ViewModel.stopSaveWork(item);
            }
        }

        public void deleteSelectedCommand(object sender, RoutedEventArgs e)
        {
            foreach (SaveWorkModel item in dgSaveWorks.SelectedItems)
            {
                _ViewModel.RemoveSaveWorkWPF(item);
            }
            RefreshSaveWorks();
        }
    }
}