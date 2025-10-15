using System.Text;

using NJsonSchema;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Sharpfetch.Core.Helpers
{
    public class ASCIIArtGenerator
    {
        private readonly string _asciiChars;
        private readonly string _asciiCharsReversed;

        public ASCIIArtGenerator()
        {
            _asciiChars = " .:-=+*#%@";
            _asciiCharsReversed = "@%#*+=-:. ";
        }

        /// <summary>
        /// Generates an ASCII representation of the image at imagePath.
        /// </summary>
        /// <param name="imagePath">Path to the source image file.</param>
        /// <param name="width">Target width in characters (default 100).</param>
        /// <param name="reverse">If true, reverses the brightness mapping.</param>
        public string Generate(byte[] image, int scale, bool reverse = false)
        {
            using Image<Rgb24> original = Image.Load<Rgb24>(image);

            // Maintain aspect ratio; 0.55 factor accounts for typical console character height/width ratio

            // Clone & resize (avoid mutating original if you might reuse it elsewhere)
            using Image<Rgb24> resized = original.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(original.Width - (original.Height / scale) * 2, original.Height / scale),
                Mode = ResizeMode.Stretch
            }));

            return ConvertToAscii(resized, reverse);
        }

        private string ConvertToAscii(Image<Rgb24> image, bool reverse)
        {
            var sb = new StringBuilder(image.Height * (image.Width + Environment.NewLine.Length));
            string chars = reverse ? _asciiCharsReversed : _asciiChars;
            int charLenMinus1 = chars.Length - 1;

            // Iterate rows efficiently
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var rowSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < rowSpan.Length; x++)
                    {
                        Rgb24 pixel = rowSpan[x];
                        // Luminance (same coefficients as original)
                        double gray = 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
                        int charIndex = (int)(gray / 255.0 * charLenMinus1);

                        sb.Append($"[rgb({pixel.R},{pixel.G},{pixel.B})]{chars[charIndex]}[/]");
                    }
                    sb.AppendLine();
                }
            });

            return sb.ToString();
        }
    }
}
