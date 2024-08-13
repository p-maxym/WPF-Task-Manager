using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Task_Manager
{
    partial class AutorizationPanel : UserControl
    {
        public AutorizationPanel()
        {
            InitializeComponent();
            DBOperations.ConnectionDB();
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
    }
}
