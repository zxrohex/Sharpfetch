using System.Threading.Tasks;
using Sharpfetch.CLI.Commands;
using Spectre.Console.Cli;

namespace Sharpfetch
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(cfg =>
            {
                cfg.PropagateExceptions();
            });

            app.SetDefaultCommand<BaseCommand>();

            await app.RunAsync(args);
        }
    }
}
