using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DomainDrivenGameEngine.Media.ImageSharp.IO
{
    /// <summary>
    /// A <see cref="Stream"/> for streaming bytes from a <see cref="Image{Rgb24}"/>.
    /// </summary>
    internal class Rgb24ImageStream : BaseImageStream<Rgb24>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rgb24ImageStream"/> class.
        /// </summary>
        /// <param name="image">The image to stream bytes from.</param>
        public Rgb24ImageStream(Image<Rgb24> image)
            : base(image, 3)
        {
        }

        /// <inheritdoc/>
        protected override byte[] ReadPixelBytes(int x, int y)
        {
            var pixel = Image[x, y];
            return new[] { pixel.R, pixel.G, pixel.B };
        }
    }
}
