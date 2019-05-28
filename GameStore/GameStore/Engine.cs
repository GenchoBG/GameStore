using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.Commands.AuthenticatedCommands;
using GameStore.Commands.GuestCommands;
using GameStore.Commands.Interfaces;
using GameStore.Exceptions;
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
                try
                {
                    Console.Write($"{this.CurrentUsername ?? "Guest"}:~$ ");
                    var input = Console.ReadLine();
                    if (input.ToUpper() == "EXIT")
                    {
                        break;
                    }

                    var args = input.Split();

                    var cmd = args[0];

                    args = args.Skip(1).ToArray();

                    if (!this.commands.ContainsKey(cmd))
                    {
                        throw new GameStoreException("Command not found.");
                    }

                    var command = this.commands[cmd];
                    command.Execute(args);
                }
                catch (GameStoreException e)
                {
                    var @default = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = @default;
                }
            }
        }

        public void Dispose()
        {
            this.Connection.Dispose();
        }

        private void RegisterCommands()
        {
            this.commands = new Dictionary<string, ICommand>
            {
                ["Register"] = new RegisterCommand(this),
                ["Login"] = new LoginCommand(this),
                ["Logout"] = new LogoutCommand(this),
                ["ChangePassword"] = new ChangePasswordCommand(this),
                ["DeleteAccount"] = new DeleteAccountCommand(this),
                ["Deposit"] = new DepositCommand(this)
            };
        }

        private void InitialSetup() //TODO: don't drop the db on every startup
        {
            var query = @"DROP DATABASE IF EXISTS GameStore;
                            CREATE DATABASE GameStore CHARSET 'utf8';
                            USE GameStore;

                            CREATE TABLE Users(
	                            Id INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY,
	                            Username VARCHAR(150) NOT NULL,
	                            Password VARCHAR(150) NOT NULL,
                                Balance DOUBLE NOT NULL
                            );

                            CREATE INDEX Index_User_Username ON Users (Username);

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
                            );

                            INSERT INTO Users(Id, Username, Password, Balance) VALUES(1, 'Manager', 'manager', 100);
                            INSERT INTO Users(Id, Username, Password, Balance) VALUES(2, 'Pesho123', 'peshopesho', 0);

                            INSERT INTO Roles(Id, Name) VALUES(1, 'manager');

                            INSERT INTO UserRoles(UserId, RoleId) VALUES(1, 1);

                            CREATE TABLE Games(
	                            Id INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY,
	                            Name VARCHAR(150) NOT NULL,
	                            Description VARCHAR(500) NOT NULL,
                                Price DOUBLE NOT NULL
                            );

                            CREATE INDEX Index_Games_Name ON Games (Name);

                            CREATE TABLE UserGames(
	                            UserId INTEGER NOT NULL,
	                            GameId INTEGER NOT NULL, 
	                            FOREIGN KEY (UserId) REFERENCES Users(Id),
	                            FOREIGN KEY (GameId) REFERENCES Games(Id),
	                            PRIMARY KEY (UserId, GameId)
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
