using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class DepositCommand : AuthenticatedCommand
    {
        public DepositCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)
            {
                throw new GameStoreException("Command format: Deposit {amount}.");
            }

            var balanceQuery = "SELECT u.Balance FROM Users u WHERE u.Username = @Username;";

            var username = this.Engine.CurrentUsername;
            var deposit = double.Parse(args[0]);

            using (var cmd = new MySqlCommand(balanceQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                var currentBalance = (double) cmd.ExecuteScalar();

                var updateQuery = "UPDATE Users SET Balance = @NewBalance WHERE Username = @Username;";

                using (var msc = new MySqlCommand(updateQuery, this.Engine.Connection))
                {
                    msc.Parameters.AddWithValue("Username", username);
                    msc.Parameters.AddWithValue("NewBalance", currentBalance + deposit);

                    msc.ExecuteNonQuery();

                    this.Engine.Connection.Close();
                }
            }

            this.WriteSuccessMessage("Balance successfully updated.");
        }
    }
}
