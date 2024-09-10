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
    public class DBOperations
    {
        public string? TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string? TaskStatus { get; set; }
        public string? TaskType { get; set; }
        public int Important { get; set; }
        public int Numeration { get; set; }

        static readonly string DBConnect = "server=localhost;user=root;password=Parolotmysql1@;" + "database=taskmanagerdb";
        static MySqlDataAdapter? msDataAdapter;
        public static MySqlDataAdapter? MsDataAdapterInstance
        {
            get => msDataAdapter;
            private set => msDataAdapter = value;
        }
        static MySqlConnection? myConnect;
        static MySqlCommand? msCommand;
        public static MySqlCommand? MsCommandInstance
        {
            get => msCommand;
            private set => msCommand = value;
        }


        public static bool ConnectionDB()
        {
            try
            {
                myConnect = new MySqlConnection(DBConnect);
                myConnect.Open();
                MsCommandInstance = new MySqlCommand() { Connection = myConnect };
                MsDataAdapterInstance = new MySqlDataAdapter(msCommand);
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

        public static async Task AddTaskToDBAsync(string taskDescription, string taskStatus, string taskType, DateTime dueDate, int importantValue)
        {
            MySqlConnection connectStatus = GetConnection();
            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string insertQuery = "INSERT INTO tasks (id, TaskDescription, DueDate, TaskStatus, TaskType, Important) VALUES(@id, @TaskDescription, @DueDate, @TaskStatus, @TaskType, @Important)";

                    await using MySqlCommand cmd = new(insertQuery, connectStatus);
                    {
                        cmd.Parameters.AddWithValue("@id", "12345678");
                        cmd.Parameters.AddWithValue("@TaskDescription", taskDescription);
                        cmd.Parameters.AddWithValue("@DueDate", dueDate);
                        cmd.Parameters.AddWithValue("@TaskStatus", taskStatus);
                        cmd.Parameters.AddWithValue("@TaskType", taskType);
                        cmd.Parameters.AddWithValue("@Important", importantValue);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        Debug.WriteLine($"TASK ADDED SUCCESSFULLY. ROWS AFFECTED: {rowsAffected}");
                    }
                }
                else
                {
                    Debug.WriteLine("FAILED CONNECT TO DATABASE.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        public static async Task<List<DBOperations>> GetTasksByIdAsync(string taskType, int importantValue)
        {
            List<DBOperations> tasks = [];
            MySqlConnection connectStatus = GetConnection();
            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string selectQuery = "SELECT * FROM tasks WHERE id = @id AND TaskType = @TaskType";

                    if (importantValue == 1) selectQuery = "SELECT * FROM tasks WHERE id = @id AND TaskType = @TaskType OR id = @id AND Important = @Important";

                    await using MySqlCommand cmd = new(selectQuery, connectStatus);
                    {
                        cmd.Parameters.AddWithValue("@id", Autorization._Id);
                        cmd.Parameters.AddWithValue("@TaskType", taskType);
                        cmd.Parameters.AddWithValue("@Important", importantValue);

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
                                    Important = dbDataReader.GetInt16("Important"),
                                    Numeration = dbDataReader.GetInt32("Numeration")
                                };
                                tasks.Add(task);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("FAILED CONNECT TO DATABASE.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
            return tasks;
        }

        public static async Task MarkTaskAsPending(int numeration)
        {
            MySqlConnection connectStatus = GetConnection();

            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string updateQuery = "UPDATE tasks SET TaskStatus = @TaskStatus WHERE numeration = @numeration AND id = @id";

                    await using MySqlCommand cmd = new(updateQuery, connectStatus);
                    {
                        cmd.Parameters.AddWithValue("@TaskStatus", "Pending");
                        cmd.Parameters.AddWithValue("@numeration", numeration);
                        cmd.Parameters.AddWithValue("@id", Autorization._Id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    Debug.WriteLine("FAILED CONNECT TO DATABASE.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        public static async Task MarkTaskAsCompleted(int numeration)
        {
            MySqlConnection connectStatus = GetConnection();

            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string updateQuery = "UPDATE tasks SET TaskStatus = @TaskStatus WHERE numeration = @numeration AND id = @id";

                    await using MySqlCommand cmd = new(updateQuery, connectStatus);
                    {
                        cmd.Parameters.AddWithValue("@TaskStatus", "Completed");
                        cmd.Parameters.AddWithValue("@numeration", numeration);
                        cmd.Parameters.AddWithValue("@id", Autorization._Id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    Debug.WriteLine("FAILED CONNECT TO DATABASE.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
            }
        }

        public static async Task DeleteTask(int numeration)
        {
            MySqlConnection connectStatus = GetConnection();

            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string updateQuery = "DELETE FROM tasks WHERE numeration = @numeration";

                    await using MySqlCommand cmd = new(updateQuery, myConnect);
                    {
                        cmd.Parameters.AddWithValue("@numeration", numeration);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    Debug.WriteLine("TASK DELETED SUCCESSFULLY.");
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
        }
    }
}
