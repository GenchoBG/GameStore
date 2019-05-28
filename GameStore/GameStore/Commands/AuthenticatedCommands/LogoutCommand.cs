using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Commands.AuthenticatedCommands
{
    class LogoutCommand : AuthenticatedCommand
    {
        public LogoutCommand(Engine engine) : base(engine)
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            this.Engine.IsLoggedIn = false;
            this.Engine.CurrentUsername = null;

            this.WriteSuccessMessage("Successfully logged out.");
        }
    }
}
