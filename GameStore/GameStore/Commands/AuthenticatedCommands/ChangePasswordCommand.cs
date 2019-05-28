using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    class ChangePasswordCommand : AuthenticatedCommand
    {
        public ChangePasswordCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 2)
            {
                throw new GameStoreException("Command format: ChangePassword {old} {new}.");
            }

            var passwordQuery = "SELECT u.Password FROM Users u WHERE u.Username = @Username;";

            var username = this.Engine.CurrentUsername;
            var oldPassword = args[0];
            var newPassword = args[1];

            using (var cmd = new MySqlCommand(passwordQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                var password = cmd.ExecuteScalar() as string;

                this.Engine.Connection.Close();

                if (password != oldPassword)
                {
                    throw new GameStoreException("Passwords don't match.");
                }
            }

            var updateQuery = "UPDATE Users SET Password = @NewPassword WHERE Username = @Username;";

            using (var cmd = new MySqlCommand(updateQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("NewPassword", newPassword);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage("New password successfully updated.");
        }
    }
}
