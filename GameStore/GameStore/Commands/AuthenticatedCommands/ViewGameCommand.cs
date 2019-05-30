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

                var id = reader.GetInt32("Id");
                var price = reader.GetDouble("Price");
                var description = reader.GetString("Description");

                reader.Close();

                Console.WriteLine($"{game} ({price}$)");
                Console.WriteLine(new string('-', Console.WindowWidth - 1));
                Console.WriteLine(description);

                var reviewsQuery = $"SELECT * FROM Reviews r INNER JOIN Users u ON r.UserId = u.Id WHERE r.GameId = @GameId;";

                using (var reviewsCmd = new MySqlCommand(reviewsQuery, this.Engine.Connection))
                {
                    reviewsCmd.Parameters.AddWithValue("GameId", id);

                    var reviewsReader = reviewsCmd.ExecuteReader();

                    if (!reviewsReader.HasRows)
                    {
                        Console.WriteLine("No reviews :/");
                    }

                    while (reviewsReader.Read())
                    {
                        var username = reviewsReader.GetString("Username");
                        var heading = reviewsReader.GetString("Heading");
                        var content = reviewsReader.GetString("Content");

                        Console.WriteLine();
                        Console.WriteLine($"{heading} (By {username})");
                        Console.WriteLine(content);
                    }
                }

                this.Engine.Connection.Close();
            }
        }
    }
}
