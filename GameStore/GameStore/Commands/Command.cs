using System;
using GameStore.Commands.Interfaces;

namespace GameStore.Commands
{
    public abstract class Command : ICommand
    {
        protected Command(Engine engine)
        {
            this.Engine = engine;
        }

        protected Engine Engine { get; }

        public void Execute(string[] args)
        {
            this.Validate();
            this.ExecuteCore(args);
        }

        protected virtual void Validate() { }

        protected abstract void ExecuteCore(string[] args);

        protected void WriteSuccessMessage(string message)
        {
            var @default = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = @default;
        }
    }
}
