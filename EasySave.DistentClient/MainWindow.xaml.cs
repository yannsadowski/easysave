using System.Windows;
using EasySave.DistentClient.ViewModels;

namespace EasySave.DistentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            dgSaveWorks.DataContext = ViewModel;
            try
            {
                ViewModel.ConnectSocket();
            }
            catch
            {
                MessageBox.Show("Impossible de se connecter au serveur distant");
            }
        }

        public void Visual_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Lancer_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Pause_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Stop_Click(object sender, RoutedEventArgs e)
        {
            //avec multi-thread
        }

        public void LaunchSelectedCommand(object sender, RoutedEventArgs e)
        {
        }

        public void selectAllCommand(object sender, RoutedEventArgs e)
        {
            dgSaveWorks.SelectAll();
        }

        public void pauseSelectedCommand(object sender, RoutedEventArgs e)
        {   // !!!!!!! avec multi-thread !!!!!!!!
        }

        public void stopSelectedCommand(object sender, RoutedEventArgs e)
        {   // !!!!!!! avec multi-thread !!!!!!!!!
        }
    }
}