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
            var query = @"DROP DATABASE IF EXISTS GameStore;
                            CREATE DATABASE GameStore CHARSET 'utf8';
                            USE GameStore;

                            CREATE TABLE Users(
	                            Id INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY,
	                            Usermame VARCHAR(150) NOT NULL,
	                            Password VARCHAR(150) NOT NULL,
                                Balance DOUBLE NOT NULL
                            );

                            CREATE INDEX Index_User_Username ON Users (Usermame);

                            CREATE TABLE Roles(
	                            Id INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY,
	                            Name VARCHAR(150) NOT NULL
                            );

                            CREATE INDEX Index_Roles_Name ON Roles (Name);

                            CREATE TABLE UserRoles(
	                            UserId INTEGER NOT NULL,
	                            RoleId INTEGER NOT NULL, 
	                            FOREIGN KEY (UserId) REFERENCES Users(Id),
	                            FOREIGN KEY (RoleId) REFERENCES Roles(Id),
	                            PRIMARY KEY (UserId, RoleId)
                            );";

            using (var cmd = new MySqlCommand(query, this.Connection))
            {
                this.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Connection.Close();
            }
        }
    }
}
