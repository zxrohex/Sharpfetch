using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;

using Sharpfetch.Core;
using Sharpfetch.Core.Helpers;
using Sharpfetch.Properties;

using SixLabors.ImageSharp.Processing;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Sharpfetch.CLI.Commands
{
    public class BaseCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsSystemInformation windowsSystemInformation = new WindowsSystemInformation();

                windowsSystemInformation.Print();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LinuxSystemInformation linuxSystemInformation = new LinuxSystemInformation();

                linuxSystemInformation.Print();
            }
           



            return 0;
        }
    }
}
