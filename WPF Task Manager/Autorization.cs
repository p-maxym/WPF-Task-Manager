using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Task_Manager
{
    class Autorization
    {
        static public string? _Name, _Password;
        static private CancellationTokenSource? _ctoken;
        static public string _Id = "12345678";

        static private async Task<bool> AutorizationUserAsync(string login, string password, MySqlCommand msCommand)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return false;
            try
            {
                msCommand.CommandText = "SELECT * FROM users WHERE username = @username AND password = @password";
                msCommand.Parameters.Clear();
                msCommand.Parameters.AddWithValue("@username", login);
                msCommand.Parameters.AddWithValue("@password", password);

                var result = await msCommand.ExecuteScalarAsync();
                if (result != null)
                {
                    _Id = result.ToString();
                    _Name = login;
                    _Password = password;
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Autorization Error: {ex.Message}");
                return false;
            }
        }

        static public async void LoginCheck(AutorizationPanel auPanel)
        {
            if (_ctoken != null)
            {
                _ctoken.Cancel();
                _ctoken.Dispose();
            }

            if (DBOperations.MsCommandInstance == null)
            {
                Debug.WriteLine("Database command cannot be null!");
                return;
            }

            _ctoken = new CancellationTokenSource();
            var token = _ctoken.Token;
            bool isAvailable = false;

            while (!isAvailable)
            {
                await Task.Delay(750);

                _Name = auPanel.LoginTextBox.Text;
                _Password = auPanel.PasswordTextBox.Password;

                try
                {
                    isAvailable = await AutorizationUserAsync(_Name, _Password, DBOperations.MsCommandInstance);

                    if (isAvailable)
                    {
                        auPanel.AutorizationTickImage.Visibility = Visibility.Visible;
                        await Task.Delay(1500);
                        auPanel.AutorizationTickImage.Visibility = Visibility.Collapsed;
                        auPanel.AutorizationWindow.Visibility = Visibility.Collapsed;
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"Failed to login: {_Name}");
                    return;
                }
            }
        }
    }
}