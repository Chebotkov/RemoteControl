using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace ServerPart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainWindowModel();
            MainWindow1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            textField.Focus();
        }
    }
}
