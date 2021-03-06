﻿using System;
using System.Collections.Generic;
using System.Text;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands.AuthenticatedCommands
{
    public class ListGamesCommand : AuthenticatedCommand
    {
        public ListGamesCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            if (args.Length > 1)
            {
                throw new GameStoreException("Command format: ListGames {page?}.");
            }

            var page = 0U;
            if (args.Length == 1 && !uint.TryParse(args[0], out page))
            {
                throw new GameStoreException("Command format: ListGames {page?}. Page must be a positive integer.");
            }
            if (page > 0)
            {
                page--;
            }

            var gamesPerPage = Console.WindowHeight - 3;

            var userQuery = @"SELECT * FROM Games g 
                                ORDER BY g.Name 
                                LIMIT @GamesPerPage 
                                OFFSET @Offset";

            using (var cmd = new MySqlCommand(userQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("GamesPerPage", gamesPerPage);
                cmd.Parameters.AddWithValue("Offset", gamesPerPage * page);

                this.Engine.Connection.Open();

                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new GameStoreException("Page doesn't exist.");
                }

                var counter = 0;
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString("Name"));
                    counter++;
                }

                while (counter < gamesPerPage)
                {
                    Console.WriteLine();
                    counter++;
                }

                this.Engine.Connection.Close();
            }

            Console.WriteLine($"Page {page + 1}");
        }
    }
}
