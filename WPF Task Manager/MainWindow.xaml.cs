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

namespace WPF_Task_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PasswordTextBox.PasswordChanged += new RoutedEventHandler(PasswordTextBox_PasswordChanged);
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
    }
}