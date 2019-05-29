using System;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class BuyGameCommand : AuthenticatedCommand
    {
        public BuyGameCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length != 1)
            {
                throw new GameStoreException("Command format: BuyGame {name}.");
            }

            var userQuery = "SELECT u.Balance, u.Id FROM Users u WHERE u.Username = @Username;";

            var username = this.Engine.CurrentUsername;
            var game = args[0];

            using (var cmd = new MySqlCommand(userQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", username);

                this.Engine.Connection.Open();

                var userReader = cmd.ExecuteReader();
                userReader.Read();

                var balance = userReader.GetDouble("Balance");
                var userId = userReader.GetInt32("Id");

                userReader.Close();

                var gameQuery = "SELECT g.Price, g.Id FROM Games g WHERE g.Name = @Name;";

                using (var gameCommand = new MySqlCommand(gameQuery, this.Engine.Connection))
                {
                    gameCommand.Parameters.AddWithValue("Name", game);

                    var gameReader = gameCommand.ExecuteReader();
                    if (!gameReader.HasRows)
                    {
                        throw new GameStoreException("Game not found.");
                    }

                    gameReader.Read();

                    var price = gameReader.GetDouble("Price");
                    var gameId = gameReader.GetInt32("Id");

                    gameReader.Close();

                    var existsQuery = "SELECT * FROM UserGames ug WHERE ug.UserId = @UserId AND ug.GameId = @GameId;";

                    using (var existsCmd = new MySqlCommand(existsQuery, this.Engine.Connection))
                    {
                        existsCmd.Parameters.AddWithValue("UserId", userId);
                        existsCmd.Parameters.AddWithValue("GameId", gameId);

                        var exists = existsCmd.ExecuteScalar() != null;

                        if (exists)
                        {
                            throw new GameStoreException("You can't buy a game multiple times!");
                        }
                    }

                    if (balance < price)
                    {
                        throw new GameStoreException("Not enough money.");
                    }

                    var buyQuery = "INSERT INTO UserGames(UserId, GameId) VALUES(@UserId, @GameId);";
                    
                    using (var buyCmd = new MySqlCommand(buyQuery, this.Engine.Connection))
                    {
                        buyCmd.Parameters.AddWithValue("UserId", userId);
                        buyCmd.Parameters.AddWithValue("GameId", gameId);

                        buyCmd.ExecuteNonQuery();
                    }

                    var deductBalanceQuery = "UPDATE Users SET Balance = @Value WHERE Id = @UserId;";

                    using (var deductBalanceCmd = new MySqlCommand(deductBalanceQuery, this.Engine.Connection))
                    {
                        deductBalanceCmd.Parameters.AddWithValue("UserId", userId);
                        deductBalanceCmd.Parameters.AddWithValue("Value", balance - price);

                        deductBalanceCmd.ExecuteNonQuery();
                    }

                    this.Engine.Connection.Close();
                }
            }

            if (game == "Fortnite")
            {
                throw new GameStoreException("Fucking moron...");
            }

            this.WriteSuccessMessage("Game successfully bought.");
        }
    }
}
