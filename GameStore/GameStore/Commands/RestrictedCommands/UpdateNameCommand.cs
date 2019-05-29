using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.RestrictedCommands
{
    class UpdateNameCommand : RestrictedCommand
    {
        public UpdateNameCommand(Engine engine) : base(engine, "Manager")
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 2)
            {
                throw new GameStoreException("Command format: UpdateName {old} {new}.");
            }

            var oldName = args[0];
            var newName = args[1];

            var existsQuery = "SELECT g.Name FROM Games g " +
                              "WHERE g.Name = @GameName; ";

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("GameName", oldName);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (!exists)
                {
                    throw new GameStoreException("Game not found.");
                }
            }

            var updateQuery = "UPDATE Games " +
                              "SET Name = @NewName " +
                              "WHERE Name = @OldName;";

            using (var cmd = new MySqlCommand(updateQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("NewName", newName);
                cmd.Parameters.AddWithValue("OldName", oldName);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage($"Name successfully changed to {newName}.");
        }
    }
}
