using Microsoft.Extensions.CommandLineUtils;
using System;

namespace Duplicat.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "Duplicat";
            app.Description = "Finds duplicate files in a directory and outputs their path.";

            app.HelpOption("-?|-h|--help");

            var pathOption = app.Option(
                template: "-p|--path",
                description: "The path to the directory in which to search for duplicates.",
                CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (pathOption.HasValue())
                {
                    var isSuccess = LocalDuplicateFinder.TryFind(pathOption.Value(), true, out var results, out var errors);

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
                        return 1;
                    }
                }
                else
                {
                    app.ShowHint();
                }
                return 0;
            });

            app.Execute(args);
        }
    }
}
