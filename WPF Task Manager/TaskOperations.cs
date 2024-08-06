using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Task_Manager
{
    class TaskOperations
    {
        public string? TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string? TaskStatus { get; set; }
        public string? TaskType { get; set; }

        public static async Task AddTaskToDBAsync(string taskDescription, string taskStatus, string taskType, DateTime dueDate)
        {
            MySqlConnection connectStatus = DBConnection.getConnection();
            try
            {
                if (connectStatus != null && connectStatus.State == ConnectionState.Open)
                {
                    string insertQuery = "INSERT INTO tasks (id, TaskDescription, DueDate, TaskStatus, TaskType) VALUES(@id, @TaskDescription, @DueDate, @TaskStatus, @TaskType)";

                    await using (MySqlCommand cmd = new(insertQuery, connectStatus))
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

        public static async Task<List<TaskOperations>> GetTasksByIdAsync(string taskType)
        {
            List<TaskOperations> tasks = [];
            MySqlConnection connectStatus = DBConnection.getConnection();
            try
            {
                if (connectStatus != null && connectStatus.State == System.Data.ConnectionState.Open)
                {
                    string selectQuery = "SELECT * FROM tasks WHERE id = @id AND TaskType = @TaskType";

                    await using (MySqlCommand cmd = new MySqlCommand(selectQuery, connectStatus))
                    {
                        cmd.Parameters.AddWithValue("@id", "12345678");
                        cmd.Parameters.AddWithValue("@TaskType", taskType);

                        await using (DbDataReader dbDataReader = await cmd.ExecuteReaderAsync())
                        {
                            while (await dbDataReader.ReadAsync())
                            {
                                TaskOperations task = new TaskOperations
                                {
                                    TaskDescription = dbDataReader.GetString("TaskDescription"),
                                    DueDate = dbDataReader.GetDateTime("DueDate"),
                                    TaskStatus = dbDataReader.GetString("TaskStatus"),
                                    TaskType = dbDataReader.GetString("TaskType")
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

    }
}
