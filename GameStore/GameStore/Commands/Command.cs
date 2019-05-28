using FootballBetting.Commands;

namespace GameStore.Commands
{
    public abstract class Command : ICommand
    {
        protected Command(Engine engine)
        {
            this.Engine = engine;
        }

        protected Engine Engine { get; }

        public void Execute()
        {
            this.Validate();
            this.ExecuteCore();
        }

        protected virtual void Validate() { }

        protected abstract void ExecuteCore();
    }
}
