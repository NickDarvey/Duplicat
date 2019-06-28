using System;

namespace Duplicat.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var isSuccess = LocalDuplicateFinder.TryFind(args[0], true, out var results, out var errors);

            if (isSuccess)
            {
                foreach (var group in results)
                {
                    foreach (var file in group) Console.WriteLine(file);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var error in errors) Console.Error.WriteLine(error);
                Console.ResetColor();
            }
        }
    }
}
