﻿using MySql.Data.MySqlClient;
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
        static public string? _Name, _Password, _Id;
        static private CancellationTokenSource? _ctoken;

        static private async Task<bool> AutorizationUserAsync(string login, string password, MySqlCommand msCommand)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                Debug.WriteLine("Login and password cannot be empty!");
                return false;
            }
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

        static public async void AutorizationCheck(MainWindow mainWindow)
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
            bool isAvailable = false;

            while (!isAvailable)
            {
                MainWindow.FieldsUpdate(mainWindow.LoginTextBox, mainWindow.PasswordTextBox);

                string? login = MainWindow._Login;
                string? password = MainWindow._Password;

                try
                {
                    if (login != null && password != null)
                    {
                        isAvailable = await AutorizationUserAsync(login, password, DBConnection.msCommand);
                    }
                    if (isAvailable)
                    {
                        MainWindow.TickImageUpdate(mainWindow.AutorizationTickImage);
                        await Task.Delay(1200);
                        MainWindow.TickImageUpdate(mainWindow.AutorizationTickImage);
                    }
                    else await Task.Delay(600);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"Failed to login: {MainWindow._Login}");
                    return;
                }
            }
        }
    }
}
