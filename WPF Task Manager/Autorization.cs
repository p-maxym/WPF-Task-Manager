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
        static public string? _Name, _Password, _Id;

        static public async Task<bool> AutorizationUserAsync(string login, string password, MySqlCommand msCommand)
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
    }
}
