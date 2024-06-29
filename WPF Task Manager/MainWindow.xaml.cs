using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace WPF_Task_Manager
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource? _ctoken;
        private bool isAvailable = false;
        public MainWindow()
        {
            InitializeComponent();
            DBConnection.ConnectionDB();
            LoginInTaskManager();
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text))
                placeholderTextLogin.Visibility = Visibility.Visible;
            else placeholderTextLogin.Visibility = Visibility.Collapsed;
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
                placeholderTextPassword.Visibility = Visibility.Visible;
            else placeholderTextPassword.Visibility = Visibility.Collapsed;
        }

        private async void LoginInTaskManager()
        {
            if (_ctoken != null)
            {
                _ctoken.Cancel();
                _ctoken.Dispose();
            }

            if (DBConnection.msCommand == null)
            {
                Debug.WriteLine("Database command cannot be null!");
                return;
            }

            _ctoken = new CancellationTokenSource();
            var token = _ctoken.Token;

            while (!isAvailable)
            {
                string login = LoginTextBox.Text;
                string password = PasswordTextBox.Password;
                try
                {
                    isAvailable = await Autorization.AutorizationUserAsync(login, password, DBConnection.msCommand);

                    if (isAvailable)
                    {
                        AutorizationTickImage.Visibility = Visibility.Visible;
                        /// MessageBox.Show($"Welcome, {Autorization._Name}");
                    }
                    else await Task.Delay(600);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"Failed to login: {login}");
                    return;
                }
            }            
        }
    }
}