using System;

namespace GameStore.Exceptions
{
    public class GameStoreException : Exception
    {
        public GameStoreException(string message) : base(message)
        {
        }
    }
}
