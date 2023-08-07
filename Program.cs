using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

#region User Manual
/*
  1. Выход exit
 */
#endregion

namespace ProductDB
{
    class Program
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ProductsDB"].ConnectionString.Replace("%FileToDb%", Path.GetFullPath("ProductsDB.mdf"));
        //Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Светлана\\source\\repos\\ProductDB\\ProductDB\\ProductsDB.mdf;Integrated Security=True
        private static SqlConnection sqlConnection = null;
        static void Main(string[] args)
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            Console.WriteLine("Products");

            SqlDataReader sqlDataReader = null;

            string command = string.Empty;

            string result = string.Empty;

            while (true)
            {
                try
                {
                    Console.Write("> ");
                    command = Console.ReadLine();

                    #region Exit
                    if (command.ToLower().Equals("exit"))
                    {
                        if (sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }

                        if (sqlDataReader != null)
                        {
                            sqlDataReader.Close();
                        }

                        break;
                    }
                    #endregion

                    SqlCommand sqlCommand = null;

                    string[] commandArray = command.ToLower().Split(' ');

                    switch (commandArray[0])
                    {
                        //fselectall - выводит все данные из бд в текстовый файл и на консоль
                        //selectall - выводит все данные из бд на консоль
                        case "fselectall":
                        case "selectall":

                            sqlCommand = new SqlCommand("SELECT * FROM [Products]", sqlConnection);

                            sqlDataReader = sqlCommand.ExecuteReader();

                            while (sqlDataReader.Read())
                            {
                               result += $"{sqlDataReader["Id"]} {sqlDataReader["Product_Name"]} {sqlDataReader["Kolvo"]}\n";

                                result += new string('-', 30) + "\n";
                            }

                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Close();
                            }

                            Console.WriteLine(result);

                            if(commandArray[0][0] == 'f')
                            {
                                using(StreamWriter sw = new StreamWriter($"{AppDomain.CurrentDomain.BaseDirectory}/{commandArray[0]}_{DateTime.Now.ToString().Replace(':', '-')}.txt",
                                    true, Encoding.UTF8))
                                {
                                    sw.WriteLine(DateTime.Now.ToString());

                                    sw.WriteLine(command);

                                    sw.WriteLine(result);
                                }
                            }

                            break;
                        
                        // SELECT * FROM Products WHERE Id=
                        // Команда выборки
                        case "select":

                            sqlCommand = new SqlCommand(command, sqlConnection);

                            sqlDataReader = sqlCommand.ExecuteReader();

                            while (sqlDataReader.Read())
                            {
                                Console.WriteLine($"{sqlDataReader["Id"]} {sqlDataReader["Product_Name"]} {sqlDataReader["Kolvo"]}");

                                Console.WriteLine(new string('-', 30));
                            }

                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Close();
                            }

                            break;

                        // INSERT INTO Products (Poduct_Name, Kolvo) VALUES (N'Банан', '40')
                        // Команда добавления
                        case "insert":

                            sqlCommand = new SqlCommand(command, sqlConnection);

                            Console.WriteLine($"Добавлено: { sqlCommand.ExecuteNonQuery()} строк(а)");

                            break;

                        // UPDATE Products set Kolvo=34/Product_Name=N'Клубника' where id=
                        // Команда обновления данных
                        case "update":

                            sqlCommand = new SqlCommand(command, sqlConnection);

                            Console.WriteLine($"Изменено: { sqlCommand.ExecuteNonQuery()} строк(а)");

                            break;

                        // DELETE FROM Products WHERE Id=
                        // Команда удаления данных
                        case "delete":

                            sqlCommand = new SqlCommand(command, sqlConnection);

                            Console.WriteLine($"Удалено: { sqlCommand.ExecuteNonQuery()} строк(а)");

                            break;

                        // SORTBY Product_Name asc/desc
                        // asc - прямая сортировка (А-Я), desc - обратная сортировка (Я-А)
                        case "sortby":

                            sqlCommand = new SqlCommand($"SELECT * FROM [Products] ORDER BY {commandArray[1]} {commandArray[2]}", sqlConnection);

                            sqlDataReader = sqlCommand.ExecuteReader();

                            while (sqlDataReader.Read())
                            {
                                Console.WriteLine($"{sqlDataReader["Id"]} {sqlDataReader["Product_Name"]} {sqlDataReader["Kolvo"]}");

                                Console.WriteLine(new string('-', 30));
                            }

                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Close();
                            }

                            break;

                        // search product_name/kolvo
                        // Команда поиска
                        case "search":

                            if (commandArray[1].Equals("product_name"))
                            {
                                sqlCommand = new SqlCommand($"SELECT * FROM [Products] WHERE Product_Name LIKE N'%{commandArray[2]}%'", sqlConnection);
                            }
                            else if(commandArray[1].Equals("kolvo"))
                            {
                                sqlCommand = new SqlCommand($"SELECT * FROM [Products] WHERE Kolvo='{commandArray[2]}'", sqlConnection);
                            }
                            else
                            {
                                Console.WriteLine($"Аргумент {commandArray[1]} некорректен!");
                            }

                            
                            sqlDataReader = sqlCommand.ExecuteReader();

                            try
                            {
                                while (sqlDataReader.Read())
                                {
                                    Console.WriteLine($"{sqlDataReader["Id"]} {sqlDataReader["Product_Name"]} {sqlDataReader["Kolvo"]}");

                                    Console.WriteLine(new string('-', 30));
                                }
                            }
                            finally
                            {
                                if (sqlDataReader != null)
                                {
                                    sqlDataReader.Close();
                                }
                            }
                            break;

                        default:

                            Console.WriteLine($"Команда {command} некорректна!");

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
