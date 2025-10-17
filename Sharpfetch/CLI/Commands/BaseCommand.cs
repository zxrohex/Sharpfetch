using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;
using Sharpfetch.Core;
using Sharpfetch.Core.Helpers;
using Spectre.Console;
using Sharpfetch.Core.Sys;

namespace Sharpfetch.CLI.Commands
{
    public class BaseCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var si = SystemInformation.Create();
            si.Print();
            await Task.CompletedTask;
            return 0;
        }
    }
}
