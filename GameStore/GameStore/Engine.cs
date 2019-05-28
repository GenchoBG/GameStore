using System;
using System.Collections.Generic;
using FootballBetting.Commands;
using MySql.Data.MySqlClient;

namespace GameStore
{
    public class Engine : IDisposable
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Uid=root;Pwd=root;";

        private IDictionary<string, ICommand> commands;
        
        public Engine()
        {
            this.Connection = new MySqlConnection(ConnectionString);

            this.RegisterCommands();
            this.InitialSetup();
        }

        public MySqlConnection Connection { get; }

        public bool IsLoggedIn { get; set; }

        public string CurrentUsername { get; set; }

        public void Run()
        {
            while (true)
            {
                var command = Console.ReadLine();
                if (command == "EXIT")
                {
                    break;
                }


            }
        }

        public void Dispose()
        {
            this.Connection.Dispose();
        }

        private void RegisterCommands()
        {
            this.commands = new Dictionary<string, ICommand>();
        }

        private void InitialSetup()
        {
            var query = @"";

            using (var cmd = new MySqlCommand(query, this.Connection))
            {
                this.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Connection.Close();
            }
        }
    }
}
