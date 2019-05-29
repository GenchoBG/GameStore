using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.RestrictedCommands
{
    class UpdatePriceCommand : RestrictedCommand
    {
        public UpdatePriceCommand(Engine engine) : base(engine, "Manager")
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 2)
            {
                throw new GameStoreException("Command format: UpdatePrice {name} {price}.");
            }

            var name = args[0];
            var newPrice = double.Parse(args[1]);

            var existsQuery = "SELECT g.Name FROM Games g " +
                              "WHERE g.Name = @GameName; ";

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("GameName", name);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (!exists)
                {
                    throw new GameStoreException("Game not found.");
                }
            }

            var updateQuery = "UPDATE Games " +
                              "SET Price = @NewPrice " +
                              "WHERE Name = @Name;";

            using (var cmd = new MySqlCommand(updateQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("NewPrice", newPrice);
                cmd.Parameters.AddWithValue("Name", name);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage($"Price successfully updated to {newPrice}$.");
        }
    }
}
