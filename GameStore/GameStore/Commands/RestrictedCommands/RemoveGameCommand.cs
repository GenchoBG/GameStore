using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.RestrictedCommands
{
    class RemoveGameCommand : RestrictedCommand
    {
        public RemoveGameCommand(Engine engine) : base(engine, "Manager")
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)
            {
                throw new GameStoreException("Command format: RemoveGame {name}.");
            }

            var game = args[0];

            var existsQuery = "SELECT g.Name FROM Games g " +
                              "WHERE g.Name = @GameName; ";

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("GameName", game);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (!exists)
                {
                    throw new GameStoreException("Game not found.");
                }
            }

            Console.WriteLine($"Are you sure you want to delete '{game}'? (Y/n)");
            var confirmation = Console.ReadLine();
            if (confirmation.ToUpper() != "Y")
            {
                return;
            }

            var updateQuery = "DELETE FROM Games WHERE Name = @GameName;";

            using (var cmd = new MySqlCommand(updateQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("GameName", game);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage($"{game} successfully deleted.");
        }
    }
}
