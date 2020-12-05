using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DomainDrivenGameEngine.Media.ImageSharp.IO
{
    /// <summary>
    /// A wrapper <see cref="Stream"/> for reading pixel data from an ImageSharp <see cref="Image{TPixel}"/> object.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type of the image to stream.</typeparam>
    internal abstract class BaseImageStream<TPixel> : Stream
        where TPixel : unmanaged, IPixel<TPixel>
    {
        /// <summary>
        /// The number of bytes per pixel that will be streamed from the image.
        /// </summary>
        private int _bytesPerPixel;

        /// <summary>
        /// The current pixel bytes being read from.
        /// </summary>
        private byte[] _currentPixelBytes;

        /// <summary>
        /// The current index to read from the current pixel bytes.
        /// </summary>
        private int _currentPixelBytesIndex;

        /// <summary>
        /// The current x cursor position on the image.
        /// </summary>
        private int _x;

        /// <summary>
        /// The current y cursor position on the image.
        /// </summary>
        private int _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseImageStream{TPixel}"/> class.
        /// </summary>
        /// <param name="image">The image to stream bytes from.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel that will be streamed from the image.</param>
        protected BaseImageStream(Image<TPixel> image, int bytesPerPixel)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            if (bytesPerPixel < 0)
            {
                throw new ArgumentException($"A valid {nameof(bytesPerPixel)} is required.");
            }

            _bytesPerPixel = bytesPerPixel;
            _x = 0;
            _y = 0;
            _currentPixelBytes = null;
            _currentPixelBytesIndex = 0;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => Image.Height * Image.Width * _bytesPerPixel;

        /// <inheritdoc/>
        public override long Position
        {
            get => (_y * Image.Height * _bytesPerPixel) + (_x * _bytesPerPixel) + _currentPixelBytesIndex;
            set
            {
                var newY = (int)(value / (Image.Width * _bytesPerPixel));
                var newX = (int)(value % (Image.Width * _bytesPerPixel));

                if (_x != newX || _y != newY)
                {
                    _x = newX;
                    _y = newY;
                    _currentPixelBytes = ReadPixelBytes(newX, newY);
                    _currentPixelBytesIndex = (int)(value % _bytesPerPixel);
                }
                else
                {
                    _currentPixelBytesIndex = (int)(value % _bytesPerPixel);
                }
            }
        }

        /// <summary>
        /// Gets the image to stream bytes from.
        /// </summary>
        protected Image<TPixel> Image { get; private set; }

        /// <summary>
        /// Disposes of this stream.
        /// </summary>
        public new void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }

            base.Dispose();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentException($"A valid {nameof(offset)} is required.");
            }

            if (count < 0)
            {
                throw new ArgumentException($"A valid{nameof(count)} is required.");
            }

            if (_y >= Image.Height)
            {
                return 0;
            }

            var originalCount = count;
            while (count > 0)
            {
                if (_currentPixelBytes == null)
                {
                    _currentPixelBytes = ReadPixelBytes(_x, _y);
                }
                else if (_currentPixelBytesIndex >= _currentPixelBytes.Length)
                {
                    _x++;
                    if (_x >= Image.Width)
                    {
                        _x = 0;
                        _y++;

                        if (_y >= Image.Height)
                        {
                            _currentPixelBytes = null;
                            _currentPixelBytesIndex = 0;
                            break;
                        }
                    }

                    _currentPixelBytes = ReadPixelBytes(_x, _y);
                    _currentPixelBytesIndex = 0;
                }

                buffer[offset++] = _currentPixelBytes[_currentPixelBytesIndex++];
                count--;
            }

            return originalCount - count;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return Position;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the bytes for a pixel at a given location.
        /// </summary>
        /// <param name="x">The x position of the pixel to read.</param>
        /// <param name="y">The y position of the pixel to read.</param>
        /// <returns>The read bytes.</returns>
        protected abstract byte[] ReadPixelBytes(int x, int y);
    }
}
