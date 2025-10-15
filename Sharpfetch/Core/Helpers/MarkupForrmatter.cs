using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Sharpfetch.Core.Helpers
{
    public class MarkupFormatter
    {
        public static string FormatAndMarkup(string heading, Dictionary<string, string> keyValuePairs, System.Drawing.Color? fgColor = null)
        {
            var sb = new StringBuilder();

            string emphasisMarkup = "bold";

            if (fgColor != null)
            {
                // Use provided foreground color for emphasis
                var c = fgColor.Value;
                emphasisMarkup = $"bold #{c.R:X2}{c.G:X2}{c.B:X2}";
            }

            heading ??= string.Empty;

            sb.AppendLine($"[{emphasisMarkup}]{heading}[/]");
            sb.Append('-', heading.Length);
            sb.Append('\n');

            foreach (var kvp in keyValuePairs)
            {
                sb.AppendLine($"[{emphasisMarkup}]{kvp.Key}:[/] {kvp.Value}");
            }

            return sb.ToString();
        }

        public static IRenderable GenerateTestBars()
        {
            var darkColors = new[]
            {
                Spectre.Console.Color.DarkBlue,
                Spectre.Console.Color.DarkGreen,
                Spectre.Console.Color.DarkCyan,
                Spectre.Console.Color.DarkRed,
                Spectre.Console.Color.DarkMagenta,
                Spectre.Console.Color.Yellow4,
                Spectre.Console.Color.Grey,
                Spectre.Console.Color.Grey23
            };

            var brightColors = new[]
            {
                Spectre.Console.Color.Blue,
                Spectre.Console.Color.Green,
                Spectre.Console.Color.Cyan1,
                Spectre.Console.Color.Red,
                Spectre.Console.Color.Magenta1,
                Spectre.Console.Color.Yellow,
                Spectre.Console.Color.White,
                Spectre.Console.Color.Grey
            };

            const int blockWidth = 2;
            const int blockHeight = 1;
            var canvasWidth = darkColors.Length * blockWidth;
            var canvasHeight = blockHeight * 2;

            var canvas = new Canvas(canvasWidth, canvasHeight);

            for (var i = 0; i < darkColors.Length; i++)
            {
                var currentDarkColor = darkColors[i];
                var currentBrightColor = brightColors[i];
                var xOffset = i * blockWidth;

                for (var y = 0; y < blockHeight; y++)
                {
                    for (var x = 0; x < blockWidth; x++)
                    {
                        canvas.SetPixel(xOffset + x, y, currentDarkColor);
                    }
                }

                for (var y = 0; y < blockHeight; y++)
                {
                    for (var x = 0; x < blockWidth; x++)
                    {
                        canvas.SetPixel(xOffset + x, y + blockHeight, currentBrightColor);
                    }
                }
            }

            return canvas;
        }
    }
}
