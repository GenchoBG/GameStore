using System;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.RestrictedCommands
{
    public class AddGameCommand : RestrictedCommand
    {
        public AddGameCommand(Engine engine) : base(engine, "Manager")
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 3)
            {
                throw new GameStoreException("Command format: AddGame {name} {description} {price}.");
            }

            var name = args[0];
            var description = args[1];
            var price = double.Parse(args[2]);

            var existsQuery = "SELECT g.Name FROM Games g WHERE g.Name = @Name;";

            using (var cmd = new MySqlCommand(existsQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Name", name);

                this.Engine.Connection.Open();

                var exists = cmd.ExecuteScalar() != null;

                this.Engine.Connection.Close();

                if (exists)
                {
                    throw new GameStoreException("Game already in store.");
                }
            }

            var createQuery = "INSERT INTO Games(Name, Description, Price) " +
                              "VALUES(@Name, @Description, @Price); ";

            using (var cmd = new MySqlCommand(createQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.Parameters.AddWithValue("Price", price);

                this.Engine.Connection.Open();

                cmd.ExecuteNonQuery();

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage("Game successfully added to store.");
        }
    }
}
