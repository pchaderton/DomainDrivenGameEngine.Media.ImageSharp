﻿using System.Collections.Generic;
using System.IO;
using DomainDrivenGameEngine.Media.Models;
using DomainDrivenGameEngine.Media.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DomainDrivenGameEngine.Media.ImageSharp
{
    /// <summary>
    /// A SixLabors.ImageSharp implementation of a <see cref="IMediaSourceService{Texture}"/> for use with projects utilizing DomainDrivenGameEngine.Media.
    /// </summary>
    public class ImageSharpTextureSourceService : BaseMediaSourceService<Texture>
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
        /// Initializes a new instance of the <see cref="ImageSharpTextureSourceService"/> class.
        /// </summary>
        public ImageSharpTextureSourceService()
            : base(SupportedExtensions)
        {
        }

        /// <inheritdoc/>
        public override Texture Load(string path)
        {
            // To avoid running branching logic on a per pixel basis, branch here depending on if the
            // image format supports an alpha channel or not.
            return SupportedExtensionsWithAlpha.Contains(Path.GetExtension(path))
                ? LoadRgba8Texture(path)
                : LoadRgb8Texture(path);
        }

        /// <summary>
        /// Loads a texture with the Rgba8 <see cref="PixelFormat"/>.
        /// </summary>
        /// <param name="path">The path to the image to load.</param>
        /// <returns>The loaded <see cref="Texture"/>.</returns>
        private Texture LoadRgba8Texture(string path)
        {
            var pixelFormat = PixelFormat.Rgba8;
            var pixelFormatDetails = PixelFormatDetailsAttribute.GetPixelFormatDetails(pixelFormat);

            using (var image = Image.Load<Rgba32>(path))
            {
                var bytes = new byte[image.Width * image.Height * pixelFormatDetails.BytesPerPixel];
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        var index = ((y * image.Width) + x) * pixelFormatDetails.BytesPerPixel;
                        bytes[index] = pixel.R;
                        bytes[index + 1] = pixel.G;
                        bytes[index + 2] = pixel.B;
                        bytes[index + 3] = pixel.A;
                    }
                }

                return new Texture(image.Width, image.Height, pixelFormat, bytes);
            }
        }

        /// <summary>
        /// Loads a texture with the Rgb8 <see cref="PixelFormat"/>.
        /// </summary>
        /// <param name="path">The path to the image to load.</param>
        /// <returns>The loaded <see cref="Texture"/>.</returns>
        private Texture LoadRgb8Texture(string path)
        {
            var pixelFormat = PixelFormat.Rgb8;
            var pixelFormatDetails = PixelFormatDetailsAttribute.GetPixelFormatDetails(pixelFormat);

            using (var image = Image.Load<Rgba32>(path))
            {
                var bytes = new byte[image.Width * image.Height * pixelFormatDetails.BytesPerPixel];
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        var index = ((y * image.Width) + x) * pixelFormatDetails.BytesPerPixel;
                        bytes[index] = pixel.R;
                        bytes[index + 1] = pixel.G;
                        bytes[index + 2] = pixel.B;
                    }
                }

                return new Texture(image.Width, image.Height, pixelFormat, bytes);
            }
        }
    }
}
