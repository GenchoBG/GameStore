using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.GuestCommands
{
    public class RegisterCommand : GuestCommand
    {
        public RegisterCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 2)
            {
                throw new GameStoreException("Command format: Register {username} {password}.");
            }

            var existsQuery = "SELECT * FROM Users u WHERE u.Username = @Username;";

            var username = args[0];
            var password = args[1];

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (exists)
                {
                    throw new GameStoreException("User with such username already exists.");
                }
            }

            var insertQuery = "INSERT INTO Users (Username, Password, Balance) VALUES (@Username, @Password, 0);";

            using (var cmd = new MySqlCommand(insertQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("Password", password);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.Engine.IsLoggedIn = true;
            this.Engine.CurrentUsername = username;

            this.SetRoles();

            this.WriteSuccessMessage($"Welcome, {username}");
        }
    }
}
