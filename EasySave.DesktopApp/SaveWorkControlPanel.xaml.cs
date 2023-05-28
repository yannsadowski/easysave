using EasySave.DesktopApp.Resources;
using EasySave.DesktopApp.ViewModels;
using EasySave.lib.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EasySave.DesktopApp
{
    /// <summary>
    /// Logique d'interaction pour SaveWorkControlPanel.xaml
    /// </summary>
    public partial class SaveWorkControlPanel : Window
    {
        private ControlPanelViewModel ViewModel = new ControlPanelViewModel();
        public SaveWorkModel SaveWork;

        public SaveWorkControlPanel()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveWork = null;
            //for the windows
            Close();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveWork = new SaveWorkModel();
            // get the data of the new saveWork
            string name = NameTextBox.Text;
            string type = (TypeComboBox.SelectedIndex == 0) ? "1" : "2";
            string sourcePath = SourcePathTextBox.Text;
            string destinationPath = DestinationPathTextBox.Text;

            int verifName = ViewModel.TestNameSaveWork(name);
            int verifPathSource = ViewModel.TestSourcePathSaveWork(sourcePath);
            int verifPathDest = ViewModel.TestDestinationPathSaveWork(destinationPath);

            if (verifName == 1 || verifPathSource == 1 || verifPathDest == 1)
            {
                if (verifName == 1)
                {
                    MessageBox.Show($"{langage.ErrorName}");
                }
                if (verifPathSource == 1)
                {
                    MessageBox.Show($"{langage.ErrorPathSource}");
                }
                if (verifPathDest == 1)
                {
                    MessageBox.Show($"{langage.ErrorPathDest}");
                }
            }
            else
            {
                //this.AttributsForSaveWork = new string[4] { name, type, sourcePath, destinationPath };

                SaveWork.NameSaveWork = name;
                SaveWork.TypeSaveWork = Int32.Parse(type);
                SaveWork.SourcePathSaveWork = sourcePath;
                SaveWork.DestinationPathSaveWork = destinationPath;

                Close();
            }
        }

        private void SourcePathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedDirectory = dialog.SelectedPath;
                SourcePathTextBox.Text = selectedDirectory;
            }
        }

        private void DestinationPathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedDirectory = dialog.SelectedPath;
                DestinationPathTextBox.Text = selectedDirectory;
            }
        }
    }
}