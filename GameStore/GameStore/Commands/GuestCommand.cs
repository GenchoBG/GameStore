using System.Collections.Generic;
using GameStore.Exceptions;
using MySql.Data.MySqlClient;

namespace GameStore.Commands
{
    public abstract class GuestCommand : Command
    {
        protected GuestCommand(Engine engine) : base(engine)
        {
        }

        protected override void Validate()
        {
            if (this.Engine.IsLoggedIn)
            {
                throw new GameStoreException("You must not be logged in to use this command.");
            }
        }

        protected abstract override void ExecuteCore(string[] args);

        protected void SetRoles()
        {
            var rolesQuery = @"SELECT r.Name FROM Users u
                                INNER JOIN UserRoles ur ON ur.UserId = u.Id
                                INNER JOIN Roles r ON r.Id = ur.RoleId
                                WHERE u.Username = @Username";

            var result = new List<string>();

            using (var cmd = new MySqlCommand(rolesQuery, this.Engine.Connection))
            {
                cmd.Parameters.AddWithValue("Username", this.Engine.CurrentUsername);

                this.Engine.Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var current = reader.GetString("Name");

                    if (current != null)
                    {
                        result.Add(current.ToUpper());
                    }
                }

                this.Engine.CurrentRoles = result;

                this.Engine.Connection.Close();
            }
        }
    }
}
