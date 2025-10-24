using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

using Spectre.Console;

namespace Sharpfetch.Core.Helpers
{
    public class ASCIIArtGenerator
    { // Long, nice-looking ramp (light -> dark). Swap for a stylized one below.
        private readonly string _ramp;

        /// <param name="ramp">
        /// Optional custom ramp from light→dark, e.g.:
        ///   " .:/+osydmNM"  // Ubuntu/neofetch-ish look
        ///   " .'`^\",:;Il!i><~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"
        ///   " ░▒▓█"         // block characters
        /// </param>
        public ASCIIArtGenerator(string? ramp = null)
        {
            _ramp = ramp ?? " .'`^\",:;Il!i><~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";
        }

        /// <summary>
        /// Generate ASCII art as Spectre.Console markup.
        /// </summary>
        /// <param name="imageBytes">Source image bytes.</param>
        /// <param name="maxWidth">Target character width (<= console width recommended).</param>
        /// <param name="invert">Invert light/dark mapping.</param>
        /// <param name="colorize">Truecolor output if true; grayscale glyphs if false.</param>
        /// <param name="gamma">Gamma correction (1.6–2.2 gives punchy results).</param>
        public string Generate(
            byte[] imageBytes,
            int maxWidth = 80,
            bool invert = false,
            bool colorize = true,
            double gamma = 1.8)
        {
            using Image<Rgba32> img = Image.Load<Rgba32>(imageBytes);

            // Fit to console by default.
            if (maxWidth <= 0)
                maxWidth = Math.Max(AnsiConsole.Profile.Width - 2, 40);

            // Console characters are ~2x taller than they are wide; correct so circles look round.
            const double charAspect = 2.0;
            int targetHeight = (int)Math.Max(1,
                Math.Round(img.Height * (maxWidth / (double)img.Width) / charAspect));

            img.Mutate(ctx => ctx.Resize(maxWidth, targetHeight, KnownResamplers.NearestNeighbor, true));

            var sb = new StringBuilder(targetHeight * (maxWidth + Environment.NewLine.Length));
            int last = _ramp.Length - 1;

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        Rgba32 p = row[x];

                        // Treat transparent pixels as empty.
                        if (p.A < 10)
                        {
                            sb.Append(' ');
                            continue;
                        }

                        // Perceived luminance (0..1)
                        double r = p.R / 255.0;
                        double g = p.G / 255.0;
                        double b = p.B / 255.0;
                        double lum = 0.2126 * r + 0.7152 * g + 0.0722 * b;

                        // Gamma gives nicer tonal separation.
                        lum = Math.Pow(lum, 1.0 / gamma);
                        if (invert) lum = 1.0 - lum;

                        int idx = (int)Math.Round(lum * last);
                        idx = Math.Clamp(idx, 0, last);
                        char ch = _ramp[idx];

                        if (colorize)
                            sb.Append($"[rgb({p.R},{p.G},{p.B})]{ch}[/]");
                        else
                            sb.Append(ch);
                    }
                    sb.AppendLine();
                }
            });

            return sb.ToString();
        }
    }
}