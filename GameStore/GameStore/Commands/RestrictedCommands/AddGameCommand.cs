using System;

namespace GameStore.Commands.RestrictedCommands
{
    public class AddGameCommand : RestrictedCommand
    {
        public AddGameCommand(Engine engine) : base(engine, "Manager")
        {
        }

        protected override void ExecuteCore(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
