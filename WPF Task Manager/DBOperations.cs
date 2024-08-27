using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

namespace WPF_Task_Manager
{
    class DBOperations
    {
        public string? TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string? TaskStatus { get; set; }
        public string? TaskType { get; set; }
        public int Numeration { get; set; }

        static readonly string DBConnect = "server=localhost;user=root;password=Parolotmysql1@;" + "database=taskmanagerdb";
        public static MySqlDataAdapter? msDataAdapter;
        static MySqlConnection? myConnect;
        static public MySqlCommand? msCommand;


        public static bool ConnectionDB()
        {
            try
            {
                myConnect = new MySqlConnection(DBConnect);
                myConnect.Open();
                msCommand = new MySqlCommand() {Connection = myConnect};
                msDataAdapter = new MySqlDataAdapter(msCommand);
                Debug.WriteLine("CONNECTED TO DATABASE SUCCESSFULLY.");
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

        public static MySqlConnection GetConnection()
        {
            if (myConnect == null) throw new InvalidOperationException("Database connection is not initialized.");
            return myConnect;
        }

        public static async Task AddTaskToDBAsync(string taskDescription, string taskStatus, string taskType, DateTime dueDate)
        {
            if (ConnectionDB())
            {
                try
                {
                    string insertQuery = "INSERT INTO tasks (id, TaskDescription, DueDate, TaskStatus, TaskType) VALUES(@id, @TaskDescription, @DueDate, @TaskStatus, @TaskType)";

                    await using MySqlCommand cmd = new(insertQuery, GetConnection());
                    {
                        cmd.Parameters.AddWithValue("@id", "12345678");
                        cmd.Parameters.AddWithValue("@TaskDescription", taskDescription);
                        cmd.Parameters.AddWithValue("@DueDate", dueDate);
                        cmd.Parameters.AddWithValue("@TaskStatus", taskStatus);
                        cmd.Parameters.AddWithValue("@TaskType", taskType);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        Debug.WriteLine($"TASK ADDED SUCCESSFULLY. ROWS AFFECTED: {rowsAffected}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: " + ex.Message);
                }
                finally
                {
                    CloseDB();
                }
            }
            else
            {
                Debug.WriteLine("FAILED CONNECT TO DATABASE.");
            }
        }

        public static async Task<List<DBOperations>> GetTasksByIdAsync(string taskType)
        {
            List<DBOperations> tasks = new();
            if (ConnectionDB())
            {
                try
                {
                    string selectQuery = "SELECT * FROM tasks WHERE id = @id AND TaskType = @TaskType";

                    await using MySqlCommand cmd = new(selectQuery, GetConnection());
                    {
                        cmd.Parameters.AddWithValue("@id", Autorization._Id);
                        cmd.Parameters.AddWithValue("@TaskType", taskType);

                        await using DbDataReader dbDataReader = await cmd.ExecuteReaderAsync();
                        {
                            while (await dbDataReader.ReadAsync())
                            {
                                DBOperations task = new()
                                {
                                    TaskDescription = dbDataReader.GetString("TaskDescription"),
                                    DueDate = dbDataReader.GetDateTime("DueDate"),
                                    TaskStatus = dbDataReader.GetString("TaskStatus"),
                                    TaskType = dbDataReader.GetString("TaskType"),
                                    Numeration = dbDataReader.GetInt32("Numeration")
                                };
                                tasks.Add(task);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: " + ex.Message);
                }
                finally
                {
                    CloseDB();
                }
            }
            else
            {
                Debug.WriteLine("FAILED CONNECT TO DATABASE.");
            }
            return tasks;
        }

        public static async Task MarkTaskAsCompleted(int numeration)
        {
            if (ConnectionDB())
            {
                try
                {
                    string updateQuery = "UPDATE tasks SET TaskStatus = @TaskStatus WHERE numeration = @numeration AND id = @id";

                    await using MySqlCommand cmd = new(updateQuery, GetConnection());
                    {
                        cmd.Parameters.AddWithValue("@TaskStatus", "Completed");
                        cmd.Parameters.AddWithValue("@numeration", numeration);
                        cmd.Parameters.AddWithValue("@id", Autorization._Id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: " + ex.Message);
                }
                finally
                {
                    CloseDB();
                }
            }
            else
            {
                Debug.WriteLine("FAILED CONNECT TO DATABASE.");
            }
        }

        public static async Task DeleteTask()
        {
            try
            {
                if (ConnectionDB())
                {
                    string updateQuery = "DELETE FROM tasks WHERE numeration = @numeration";

                    await using MySqlCommand cmd = new(updateQuery, myConnect);
                    {
                        cmd.Parameters.AddWithValue("@numeration", TaskSettings._Numeration);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    Debug.WriteLine("FAILED TO CONNECT TO DATABASE.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                if (myConnect != null && myConnect.State == ConnectionState.Open)
                {
                    myConnect.Close();
                    Debug.WriteLine("DISCONNECTED FROM DATABASE.");
                }
            }
        }
    }
}
