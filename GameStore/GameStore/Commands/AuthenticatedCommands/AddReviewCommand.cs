using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    class AddReviewCommand : AuthenticatedCommand
    {
        public AddReviewCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)
            {
                throw new GameStoreException("Command format: AddReview {name}.");
            }

            var game = args[0];

            var idQuery = @"SELECT g.Id AS GameId, u.Id AS UserId FROM Users u
                            INNER JOIN UserGames ug ON ug.UserId = u.Id
                            INNER JOIN Games g ON g.Id = ug.GameId
                            WHERE u.Username = @Username AND g.Name = @GameName;";

            using (var cmd = new MySqlCommand(idQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", this.Engine.CurrentUsername);
                cmd.Parameters.AddWithValue("GameName", game);

                this.Engine.Connection.Open();

                var executeReader = cmd.ExecuteReader();
                executeReader.Read();

                if (!executeReader.HasRows)
                {
                    throw new GameStoreException("Game not found in your library.");
                }

                var gameId = executeReader.GetDouble("GameId");
                var userId = executeReader.GetInt32("UserId");

                executeReader.Close();

                var existsQuery = @"SELECT r.UserId, r.GameId FROM Users u
                                    INNER JOIN UserGames ug ON ug.UserId = u.Id
                                    INNER JOIN Games g ON g.Id = ug.GameId
                                    INNER JOIN Reviews r ON r.UserId = u.Id AND r.GameId = g.Id
                                    WHERE u.Id = @UserId AND g.Id = @GameId;";

                using (var existsCmd = new MySqlCommand(existsQuery, this.Engine.Connection))
                {
                    existsCmd.Parameters.AddWithValue("UserId", userId);
                    existsCmd.Parameters.AddWithValue("GameId", gameId);
                    
                    var exists = existsCmd.ExecuteScalar() != null;
                    
                    if (exists)
                    {
                        throw new GameStoreException("You have already uploaded a review on this game.");
                    }
                }
                Console.WriteLine("Heading:");
                var heading = Console.ReadLine();

                Console.WriteLine("Content:");
                var content = Console.ReadLine();

                var reviewQuery = @"INSERT INTO Reviews(UserId, GameId, Heading, Content) 
                                    VALUES(@UserId, @GameId, @Heading, @Content)";

                using (var reviewCmd = new MySqlCommand(reviewQuery, this.Engine.Connection))
                {
                    reviewCmd.Parameters.AddWithValue("UserId", userId);
                    reviewCmd.Parameters.AddWithValue("GameId", gameId);
                    reviewCmd.Parameters.AddWithValue("Heading", heading);
                    reviewCmd.Parameters.AddWithValue("Content", content);
                    
                    reviewCmd.ExecuteNonQuery();
                }

                this.Engine.Connection.Close();
            }

            this.WriteSuccessMessage("Review successfully uploaded.");
        }
    }
}
