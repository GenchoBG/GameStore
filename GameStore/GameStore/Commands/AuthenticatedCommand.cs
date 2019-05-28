using GameStore.Exceptions;

namespace GameStore.Commands
{
    public abstract class AuthenticatedCommand : Command
    {
        protected AuthenticatedCommand(Engine engine) : base(engine)
        {
        }

        protected override void Validate()
        {
            if (!this.Engine.IsLoggedIn)
            {
                throw new GameStoreException("You must be logged in to use this command.");
            }
        }

        protected abstract override void ExecuteCore();
    }
}
