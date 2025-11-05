using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using PPacker.Models;

namespace PPacker.Core;

/// <summary>
/// Handles loading and processing of sprite images
/// </summary>
public class SpriteProcessor
{
    /// <summary>
    /// Load a sprite from file and optionally trim transparent pixels
    /// </summary>
    public static SpriteInfo LoadSprite(string imagePath, string? name = null, bool trim = true)
    {
        using var image = Image.Load<Rgba32>(imagePath);
        
        var spriteName = name ?? Path.GetFileNameWithoutExtension(imagePath);
        var originalWidth = image.Width;
        var originalHeight = image.Height;

        var trimBounds = trim ? CalculateTrimBounds(image) : new Rectangle(0, 0, image.Width, image.Height);
        
        // Create trimmed image if needed
        Image<Rgba32> processedImage;
        if (trim && (trimBounds.X > 0 || trimBounds.Y > 0 || 
                    trimBounds.Width < image.Width || trimBounds.Height < image.Height))
        {
            processedImage = image.Clone(ctx => ctx.Crop(trimBounds));
        }
        else
        {
            processedImage = image.Clone();
        }

        return new SpriteInfo
        {
            Name = spriteName,
            Image = processedImage,
            OriginalWidth = originalWidth,
            OriginalHeight = originalHeight,
            TrimX = trimBounds.X,
            TrimY = trimBounds.Y,
            WasTrimmed = trim && (trimBounds.X > 0 || trimBounds.Y > 0 || 
                                 trimBounds.Width < originalWidth || trimBounds.Height < originalHeight)
        };
    }

    /// <summary>
    /// Calculate the bounds of non-transparent pixels
    /// </summary>
    private static Rectangle CalculateTrimBounds(Image<Rgba32> image)
    {
        int minX = image.Width;
        int minY = image.Height;
        int maxX = -1;
        int maxY = -1;

        // Find bounds of non-transparent pixels
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                {
                    if (row[x].A > 0) // Non-transparent pixel
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }
        });

        // If no non-transparent pixels found, return full bounds
        if (maxX < 0)
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    /// <summary>
    /// Extract sprites from a sprite sheet using provided sprite data
    /// </summary>
    public static List<SpriteInfo> ExtractSprites(string imagePath, List<SpriteData> spriteData, string? prefix = null)
    {
        using var sourceImage = Image.Load<Rgba32>(imagePath);
        var sprites = new List<SpriteInfo>();

        foreach (var sprite in spriteData)
        {
            var bounds = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height);
            
            // Ensure bounds are within image
            bounds = Rectangle.Intersect(bounds, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
            
            if (bounds.Width <= 0 || bounds.Height <= 0)
                continue;

            var extractedImage = sourceImage.Clone(ctx => ctx.Crop(bounds));
            
            // Handle rotation if specified
            if (sprite.Rotated)
            {
                extractedImage.Mutate(ctx => ctx.Rotate(RotateMode.Rotate90));
            }

            var spriteName = prefix != null ? $"{prefix}{sprite.Name}" : sprite.Name;

            sprites.Add(new SpriteInfo
            {
                Name = spriteName,
                Image = extractedImage,
                OriginalWidth = sprite.SourceWidth ?? sprite.Width,
                OriginalHeight = sprite.SourceHeight ?? sprite.Height,
                TrimX = sprite.TrimX ?? 0,
                TrimY = sprite.TrimY ?? 0,
                WasTrimmed = sprite.TrimX.HasValue || sprite.TrimY.HasValue
            });
        }

        return sprites;
    }

    /// <summary>
    /// Create an atlas image from packed sprites
    /// </summary>
    public static Image<Rgba32> CreateAtlas(List<SpriteInfo> sprites, PackingResult packingResult, int atlasWidth, int atlasHeight)
    {
        var atlas = new Image<Rgba32>(atlasWidth, atlasHeight);

        // Image is already initialized with transparent pixels by default

        // Draw each sprite at its packed position
        foreach (var packedRect in packingResult.PackedRectangles)
        {
            var sprite = sprites.First(s => s.Name == packedRect.Name);
            var image = sprite.Image;

            // Handle rotation
            if (packedRect.Rotated)
            {
                image = image.Clone(ctx => ctx.Rotate(RotateMode.Rotate90));
            }

            // Draw the sprite at the packed position
            atlas.Mutate(ctx => ctx.DrawImage(image, new Point(packedRect.X, packedRect.Y), 1.0f));

            // Dispose rotated image if it was created
            if (packedRect.Rotated && image != sprite.Image)
            {
                image.Dispose();
            }
        }

        return atlas;
    }

    /// <summary>
    /// Resize atlas to power of 2 dimensions if requested
    /// </summary>
    public static (int width, int height) CalculateFinalDimensions(int width, int height, bool powerOfTwo)
    {
        if (!powerOfTwo)
            return (width, height);

        return (NextPowerOfTwo(width), NextPowerOfTwo(height));
    }

    private static int NextPowerOfTwo(int value)
    {
        if (value <= 0) return 1;
        
        value--;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value++;

        return value;
    }
}

/// <summary>
/// Information about a loaded sprite
/// </summary>
public class SpriteInfo : IDisposable
{
    public string Name { get; set; } = string.Empty;
    public Image<Rgba32> Image { get; set; } = null!;
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }
    public int TrimX { get; set; }
    public int TrimY { get; set; }
    public bool WasTrimmed { get; set; }

    public void Dispose()
    {
        Image?.Dispose();
    }
}