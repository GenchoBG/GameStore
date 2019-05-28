using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.GuestCommands
{
    public class LoginCommand : GuestCommand
    {
        public LoginCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 2)
            {
                throw new GameStoreException("Command format: Login {username} {password}.");
            }

            var existsQuery = "SELECT * FROM Users u WHERE u.Username = @Username AND u.Password = @Password;";

            var username = args[0];
            var password = args[1];

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("Password", password);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (!exists)
                {
                    throw new GameStoreException("Invalid username or password.");
                }

                this.Engine.IsLoggedIn = true;
                this.Engine.CurrentUsername = username;

                this.WriteSuccessMessage($"Welcome back, {username}");
            }
        }
    }
}
