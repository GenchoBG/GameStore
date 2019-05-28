using System;
using System.Text;

namespace GameStore
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            using (var engine = new Engine())
            {
                engine.Run();
            }
        }
    }
}
