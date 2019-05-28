using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class DeleteAccountCommand : AuthenticatedCommand
    {
        public DeleteAccountCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)
            {
                throw new GameStoreException("Command format: DeleteAccount {password}.");
            }

            var passwordQuery = "SELECT u.Password FROM Users u WHERE u.Username = @Username;";

            var username = this.Engine.CurrentUsername;
            var inputPassword = args[0];

            using (var cmd = new MySqlCommand(passwordQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                var password = cmd.ExecuteScalar() as string;

                this.Engine.Connection.Close();

                if (password != inputPassword)
                {
                    throw new GameStoreException("Wrong password.");
                }
            }

            Console.WriteLine("Are you sure you want to delete your user? (Y/n)");
            var confirmation = Console.ReadLine();
            if (confirmation.ToUpper() != "Y")
            {
                return;
            }

            var updateQuery = "DELETE FROM Users WHERE Username = @Username;";

            using (var cmd = new MySqlCommand(updateQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.Engine.IsLoggedIn = false;
            this.Engine.CurrentUsername = null;

            this.WriteSuccessMessage("User Deleted.");
        }
    }
}
