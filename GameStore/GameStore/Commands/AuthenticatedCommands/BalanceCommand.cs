using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class BalanceCommand : AuthenticatedCommand
    {
        public BalanceCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            var userQuery = "SELECT * FROM Users u WHERE u.Username = @Username;";

            using (var cmd = new MySqlCommand(userQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", this.Engine.CurrentUsername);

                this.Engine.Connection.Open();

                var reader = cmd.ExecuteReader();

                reader.Read();

                var balance = reader.GetDouble("Balance");
                Console.WriteLine($"Balance: {balance}");

                this.Engine.Connection.Close();
            }
        }
    }
}
