
using FishyFlip.Lexicon.App.Bsky.Embed;
using Microsoft.Extensions.Logging;

namespace MackerelSocial.Core.Utilities;

public class FileContentTypeDetector
{
    private (byte[], string)[] fileSignatures;
    private ILogger? logger;

    public FileContentTypeDetector(ILogger? logger)
    {
        this.logger = logger;
        this.fileSignatures = new[]
        {
                (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg"), // JPEG
                (new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png"), // PNG
                ("GIF8"u8.ToArray(), "image/gif"), // GIF
                (new byte[] { 0x52, 0x49, 0x46, 0x46 }, "image/webp"), // WebP
        };
    }

    public string GetContentType(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return "unsupported";
        }

        return this.GetContentType(new MemoryStream(bytes));
    }

    public string GetContentType(Stream? fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return "unsupported";
        }

        long originalPosition = fileStream.Position;
        fileStream.Position = 0;

        foreach (var (signature, mimeType) in this.fileSignatures)
        {
            var buffer = new byte[signature.Length];
            if (fileStream.Read(buffer, 0, signature.Length) == signature.Length)
            {
                if (signature.SequenceEqual(buffer))
                {
                    fileStream.Position = originalPosition;
                    this.logger?.LogDebug($"Detected file type: {mimeType}");
                    return mimeType;
                }
            }

            fileStream.Position = 0;
        }

        fileStream.Position = originalPosition;
        this.logger?.LogDebug("Unsupported file type.");
        return "unsupported";
    }

    public AspectRatio GetAspectRatio(Stream? fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            this.logger?.LogDebug("Invalid stream for aspect ratio detection.");
            return new AspectRatio(0, 0);
        }

        long originalPosition = fileStream.Position;
        fileStream.Position = 0;

        try
        {
            string contentType = GetContentType(fileStream);
            fileStream.Position = 0;

            var dimensions = contentType switch
            {
                "image/jpeg" => GetJpegDimensions(fileStream),
                "image/png" => GetPngDimensions(fileStream),
                "image/gif" => GetGifDimensions(fileStream),
                "image/webp" => GetWebpDimensions(fileStream),
                _ => (0, 0)
            };
            
            return new AspectRatio(dimensions.width, dimensions.height);
        }
        finally
        {
            fileStream.Position = originalPosition;
        }
    }

    private (long width, long height) GetJpegDimensions(Stream stream)
    {
        stream.Position = 0;
        var buffer = new byte[2];

        // Skip JPEG signature
        stream.Position = 2;

        while (stream.Position < stream.Length)
        {
            // Read marker
            if (stream.Read(buffer, 0, 2) != 2 || buffer[0] != 0xFF)
                break;

            byte marker = buffer[1];

            // Skip padding bytes
            while (marker == 0xFF && stream.Position < stream.Length)
            {
                marker = (byte)stream.ReadByte();
            }

            // SOF markers (Start of Frame) contain dimension info
            if ((marker >= 0xC0 && marker <= 0xC3) ||
                (marker >= 0xC5 && marker <= 0xC7) ||
                (marker >= 0xC9 && marker <= 0xCB) ||
                (marker >= 0xCD && marker <= 0xCF))
            {
                stream.Position += 3; // Skip length and precision

                var heightBytes = new byte[2];
                var widthBytes = new byte[2];

                if (stream.Read(heightBytes, 0, 2) == 2 && stream.Read(widthBytes, 0, 2) == 2)
                {
                    int height = (heightBytes[0] << 8) | heightBytes[1];
                    int width = (widthBytes[0] << 8) | widthBytes[1];
                    this.logger?.LogDebug($"JPEG dimensions: {width}x{height}");
                    return (width, height);
                }
            }
            else
            {
                // Read segment length and skip
                if (stream.Read(buffer, 0, 2) == 2)
                {
                    int segmentLength = (buffer[0] << 8) | buffer[1];
                    stream.Position += segmentLength - 2;
                }
            }
        }

        this.logger?.LogDebug("Could not extract JPEG dimensions.");
        return (0, 0);
    }

    private (long width, long height) GetPngDimensions(Stream stream)
    {
        stream.Position = 8; // Skip PNG signature

        var buffer = new byte[8];

        // Read IHDR chunk length (should be at position 8)
        if (stream.Read(buffer, 0, 8) == 8)
        {
            // Verify this is IHDR chunk
            if (buffer[4] == 'I' && buffer[5] == 'H' && buffer[6] == 'D' && buffer[7] == 'R')
            {
                var dimensionBytes = new byte[8];
                if (stream.Read(dimensionBytes, 0, 8) == 8)
                {
                    int width = (dimensionBytes[0] << 24) | (dimensionBytes[1] << 16) |
                               (dimensionBytes[2] << 8) | dimensionBytes[3];
                    int height = (dimensionBytes[4] << 24) | (dimensionBytes[5] << 16) |
                                (dimensionBytes[6] << 8) | dimensionBytes[7];

                    this.logger?.LogDebug($"PNG dimensions: {width}x{height}");
                    return (width, height);
                }
            }
        }

        this.logger?.LogDebug("Could not extract PNG dimensions.");
        return (0, 0);
    }

    private (long width, long height) GetGifDimensions(Stream stream)
    {
        stream.Position = 6; // Skip GIF signature

        var buffer = new byte[4];

        if (stream.Read(buffer, 0, 4) == 4)
        {
            // GIF uses little-endian byte order
            int width = buffer[0] | (buffer[1] << 8);
            int height = buffer[2] | (buffer[3] << 8);

            this.logger?.LogDebug($"GIF dimensions: {width}x{height}");
            return (width, height);
        }

        this.logger?.LogDebug("Could not extract GIF dimensions.");
        return (0, 0);
    }

    private (long width, long height) GetWebpDimensions(Stream stream)
    {
        stream.Position = 0;
        var buffer = new byte[30];

        if (stream.Read(buffer, 0, 30) < 30)
            return (0, 0);

        // Verify RIFF and WEBP signatures
        if (buffer[0] != 'R' || buffer[1] != 'I' || buffer[2] != 'F' || buffer[3] != 'F' ||
            buffer[8] != 'W' || buffer[9] != 'E' || buffer[10] != 'B' || buffer[11] != 'P')
            return (0, 0);

        // Check VP8 variant
        if (buffer[12] == 'V' && buffer[13] == 'P' && buffer[14] == '8')
        {
            if (buffer[15] == ' ') // VP8 (lossy)
            {
                // Skip to frame tag
                if (buffer[23] == 0x9D && buffer[24] == 0x01 && buffer[25] == 0x2A)
                {
                    int width = (buffer[26] | (buffer[27] << 8)) & 0x3FFF;
                    int height = (buffer[28] | (buffer[29] << 8)) & 0x3FFF;

                    this.logger?.LogDebug($"WebP (VP8) dimensions: {width}x{height}");
                    return (width, height);
                }
            }
            else if (buffer[15] == 'L') // VP8L (lossless)
            {
                // Read 4 bytes starting at position 21
                uint bits = (uint)(buffer[21] | (buffer[22] << 8) | (buffer[23] << 16) | (buffer[24] << 24));
                int width = (int)((bits & 0x3FFF) + 1);
                int height = (int)(((bits >> 14) & 0x3FFF) + 1);

                this.logger?.LogDebug($"WebP (VP8L) dimensions: {width}x{height}");
                return (width, height);
            }
            else if (buffer[15] == 'X') // VP8X (extended)
            {
                // Width and height are 24-bit values at specific positions
                int width = (buffer[24] | (buffer[25] << 8) | (buffer[26] << 16)) + 1;
                int height = (buffer[27] | (buffer[28] << 8) | (buffer[29] << 16)) + 1;

                this.logger?.LogDebug($"WebP (VP8X) dimensions: {width}x{height}");
                return (width, height);
            }
        }

        this.logger?.LogDebug("Could not extract WebP dimensions.");
        return (0, 0);
    }
}