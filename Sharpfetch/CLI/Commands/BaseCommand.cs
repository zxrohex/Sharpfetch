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
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            SystemInfo info = new SystemInfo();

   
            Panel logoPanel = new Panel(new CanvasImage(info.OSPlatform == OSPlatform.Windows ? Resources.WindowsLogo64px : Resources.LinuxLogo64px)
                .Mutate(m => m.Resize(20, 20, KnownResamplers.NearestNeighbor)))
                .NoBorder();

            Panel contentPanel = new Panel(info.ToConsoleText()).NoBorder().Expand();

            var columns = new Columns(logoPanel, contentPanel);

            columns.Collapse();

            AnsiConsole.Write(columns);

            

            return 0;
        }
    }
}
