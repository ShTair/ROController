using ShComp;
using System;
using System.Threading.Tasks;

namespace ROController
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunAsync).Wait();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            Mouse.SetHook();
        }
    }
}
