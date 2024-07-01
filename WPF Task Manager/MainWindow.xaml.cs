using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
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
        static public string? _Login, _Password;
        public MainWindow()
        {
            InitializeComponent();
            DBConnection.ConnectionDB();
            LoginTextBox.Text = "xD";
            PasswordTextBox.Focus();
            Autorization.LoginCheck(this);
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

        static public void FieldsUpdate(TextBox loginTextBox, PasswordBox passwordTextBox)
        {
            _Login = loginTextBox.Text;
            _Password = passwordTextBox.Password;
        }

        static public void TickImageUpdate(Image autorizationTickImage)
        {
            autorizationTickImage.Visibility = (autorizationTickImage.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}