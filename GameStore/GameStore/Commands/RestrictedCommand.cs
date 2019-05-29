using System.Linq;
using GameStore.Exceptions;

namespace GameStore.Commands
{
    public abstract class RestrictedCommand : AuthenticatedCommand
    {
        private readonly string[] roles;

        protected RestrictedCommand(Engine engine, params string[] roles) : base(engine)
        {
            this.roles = roles;
        }

        protected override void Validate()
        {
            base.Validate();
            if (this.roles.All(r => !this.Engine.CurrentRoles.Contains(r.ToUpper())))
            {
                throw new GameStoreException("You do not have permission for this command.");
            }
        }

        protected abstract override void ExecuteCore(string[] args);
    }
}
