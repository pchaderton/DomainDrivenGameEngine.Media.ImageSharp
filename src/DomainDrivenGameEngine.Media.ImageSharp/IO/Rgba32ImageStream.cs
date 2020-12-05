using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DomainDrivenGameEngine.Media.ImageSharp.IO
{
    /// <summary>
    /// A <see cref="Stream"/> for streaming bytes from a <see cref="Image{Rgba32}"/>.
    /// </summary>
    internal class Rgba32ImageStream : BaseImageStream<Rgba32>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rgba32ImageStream"/> class.
        /// </summary>
        /// <param name="image">The image to stream bytes from.</param>
        public Rgba32ImageStream(Image<Rgba32> image)
            : base(image, 4)
        {
        }

        /// <inheritdoc/>
        protected override byte[] ReadPixelBytes(int x, int y)
        {
            var pixel = Image[x, y];
            return new[] { pixel.R, pixel.G, pixel.B, pixel.A };
        }
    }
}
