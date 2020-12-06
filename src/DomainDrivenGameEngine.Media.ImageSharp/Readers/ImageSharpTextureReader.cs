using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DomainDrivenGameEngine.Media.Models;
using DomainDrivenGameEngine.Media.Readers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DomainDrivenGameEngine.Media.ImageSharp.Readers
{
    /// <summary>
    /// A SixLabors.ImageSharp implementation of a <see cref="IMediaReader{Texture}"/> for use with projects utilizing DomainDrivenGameEngine.Media.
    /// </summary>
    public class ImageSharpTextureReader : BaseMediaReader<Texture>
    {
        /// <summary>
        /// The extensions this source service supports.
        /// </summary>
        private static readonly IReadOnlyCollection<string> SupportedExtensions = new string[]
        {
            ".bmp",
            ".jpg",
            ".jpeg",
            ".png",
            ".tga",
        };

        /// <summary>
        /// A lookup of extensions which support an alpha channel.
        /// </summary>
        private static readonly HashSet<string> SupportedExtensionsWithAlpha = new HashSet<string>
        {
            ".png",
            ".tga",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSharpTextureReader"/> class.
        /// </summary>
        public ImageSharpTextureReader()
            : base(SupportedExtensions)
        {
        }

        /// <inheritdoc/>
        public override Texture Read(Stream stream, string path, string extension)
        {
            // To avoid running branching logic on a per pixel basis, branch here depending on if the
            // image format supports an alpha channel or not.
            return SupportedExtensionsWithAlpha.Contains(extension)
                ? LoadRgba32Texture(stream)
                : LoadRgb8Texture(stream);
        }

        /// <summary>
        /// Loads a texture with the Rgba32 <see cref="TextureFormat"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to the image to load.</param>
        /// <returns>The loaded <see cref="Texture"/>.</returns>
        private Texture LoadRgba32Texture(Stream stream)
        {
            using (var image = Image.Load<Rgba32>(stream))
            {
                var bytes = new byte[image.Width * image.Height * 4];
                var offset = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        bytes[offset] = pixel.R;
                        bytes[offset + 1] = pixel.G;
                        bytes[offset + 2] = pixel.B;
                        bytes[offset + 3] = pixel.A;
                        offset += 4;
                    }
                }

                return new Texture(image.Width, image.Height, TextureFormat.Rgba32, new ReadOnlyCollection<byte>(bytes), stream);
            }
        }

        /// <summary>
        /// Loads a texture with the Rgb24 <see cref="TextureFormat"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to the image to load.</param>
        /// <returns>The loaded <see cref="Texture"/>.</returns>
        private Texture LoadRgb8Texture(Stream stream)
        {
            using (var image = Image.Load<Rgb24>(stream))
            {
                var bytes = new byte[image.Width * image.Height * 3];
                var offset = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        bytes[offset] = pixel.R;
                        bytes[offset + 1] = pixel.G;
                        bytes[offset + 2] = pixel.B;
                        offset += 3;
                    }
                }

                return new Texture(image.Width, image.Height, TextureFormat.Rgb24, new ReadOnlyCollection<byte>(bytes), stream);
            }
        }
    }
}
