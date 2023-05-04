using System;
using System.Text.RegularExpressions;
using System.Security.Policy;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ConsoleApp7;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Data.SqlTypes;


namespace ConsoleApp7
{

    // Создаем делегат
    delegate void myDel(string Name, string Email, string Phone);

    internal class Program
    {
        
        // создаем users -  статический список  пользователей типа User для хранения информации о пользователях 
        public static List<User> users = new List<User>();

        //Метод вывода данных
         static void Print(string Name, string Email, string Phone)
        {
            // выводим результаты в виде таблицы
            Console.WriteLine("\nИмя\tEmail\t\t\tТелефон");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name}\t{user.Email}\t{user.Phone}");

            }
        }

        //Метод записи в файл
        static void WriteToFile(string Name, string Email, string Phone)
        {
            string _name = "output.txt";
            var sw = new StreamWriter(_name, true);
            sw.WriteLine(Name, Email, Phone);
            sw.Close();
            //using (var sw01 = new StreamWriter(_name))
            //{
            //    sw01.WriteLine(_text, true);
            //}

        }

        //Метод записи в базу  данных
        static void WriteToDB(string Name, string Email, string Phone)
        {
            // Создание соединения с базой данных
            string connectionString002 = @"Data Source=(localdb)\MSSQLLocalDB;
                                        Initial Catalog=master;Integrated Security=True;
                                        Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;
                                        ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(connectionString002);
            try
            {
                // Открытие соединения с базой данных
                connection.Open();
                Console.WriteLine("Connection opened successfully.");

                // Создание SQL-запроса на добавление записи в таблицу
                string query = "INSERT INTO MyTable (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";

                // Создание объекта Command
                SqlCommand command = new SqlCommand(query, connection);

                // List<User> users = new List<User>();

                foreach (User user in users)
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Phone", user.Phone);

                }
                {
                    // Выполнение SQL-запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    // Закрытие соединения с базой данных
                    connection.Close();
                    Console.WriteLine("Connection closed.");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while opening connection: " + ex.Message);
            }


        }
        static void Main(string[] args)
        {
            // запрашиваем данные у пользователя
            Console.WriteLine("Введите данные пользователя (имя, email, телефон), разделенные пробелами:");
            Console.WriteLine("Формат номера телефона: +7XXXXXXXXXX или 8XXXXXXXXXX");
            Console.WriteLine("Для завершения ввода введите END_INPUT");

            string input;
            while ((input = Console.ReadLine()) != "END_INPUT")
            {
                // проверяем, что ввод соответствует формату: имя email телефон
                var match = Regex.Match(input, @"^(\S+)\s+(\S+)\s+([+]?[78]\d{10})$");
                if (match.Success)
                {
                    // если формат верный, добавляем пользователя в список
                    var user = new User()
                    {
                        Name = match.Groups[1].Value,
                        Email = match.Groups[2].Value,
                        Phone = match.Groups[3].Value,
                    };
                    users.Add(user);
                }
                else
                {
                    // если формат неверный, выводим сообщение об ошибке
                    Console.WriteLine("Неверный формат ввода. Введите данные в формате: имя email телефон");
                }
            }
           

            // Создаем экземпляр делегата
            myDel putText;
            putText = Print;
            putText = WriteToDB;
            putText += WriteToFile;
           // putText.Invoke();
            Console.ReadKey();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();


        }

    }
}


// класс, представляющий пользователя
class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}


