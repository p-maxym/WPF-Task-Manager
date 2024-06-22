using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace WPF_Task_Manager
{
    class DBConnection
    {
        static string DBConnect = "server = localhost; user=root; password=Parolotmysql1@;" +
            "database=taskmanagerdb";
        public static MySqlDataAdapter? msDataAdapter;
        static MySqlConnection? myConnect;
        static public MySqlCommand? msCommand;

        public static bool ConnectionDB()
        {
            try
            {
                myConnect = new MySqlConnection(DBConnect);
                myConnect.Open();
                msCommand = new MySqlCommand();
                msCommand.Connection = myConnect;
                msDataAdapter = new MySqlDataAdapter(msCommand);
                return true;
            }
            catch
            {
                MessageBox.Show("DataBase connection error!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static void CloseDB()
        {
            if (myConnect != null) myConnect.Close();
            else throw new InvalidOperationException("Database connection is not initialized.");
        }

        public MySqlConnection getConnection()
        {
            if (myConnect == null) throw new InvalidOperationException("Database connection is not initialized.");
            return myConnect;
        }
    }
}
