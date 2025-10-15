using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;
using Sharpfetch.Core;
using Sharpfetch.Core.Helpers;
using Spectre.Console;

namespace Sharpfetch.CLI.Commands
{
    public class BaseCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            /*AnsiConsole.Write(new Panel(new ASCIIArtGenerator().Generate(Sharpfetch.Properties.Resources.WindowsLogo64px, 4)).Expand());

            AnsiConsole.Write(new Panel(new ASCIIArtGenerator().Generate(Sharpfetch.Properties.Resources.LinuxLogo64px, 4)).Expand());*/


            var si = SystemInformation.Create();
            si.Print();
            await Task.CompletedTask;
            return 0;
        }
    }
}
