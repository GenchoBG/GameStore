using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class ViewGameCommand : AuthenticatedCommand
    {
        public ViewGameCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)   
            {
                throw new GameStoreException("Command format: ViewGame {name}.");
            }

            var gameQuery = "SELECT * FROM Games g WHERE g.Name = @Name;";

            var game = args[0];

            using (var cmd = new MySqlCommand(gameQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Name", game);

                this.Engine.Connection.Open();

                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new GameStoreException("Game not found.");
                }
                reader.Read();

                var price = reader.GetDouble("Price");
                var description = reader.GetString("Description");

                Console.WriteLine($"{game} ({price}$)");
                Console.WriteLine(new string('-', Console.WindowWidth - 1));
                Console.WriteLine(description);

                this.Engine.Connection.Close();
            }
        }
    }
}
