using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using PPacker.Models;
using PPacker.Core;

namespace PPacker.Core;

/// <summary>
/// Main atlas packer that coordinates the packing process
/// </summary>
public class AtlasPacker
{
    private readonly PackerConfig _config;

    public AtlasPacker(PackerConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Pack all input files into a texture atlas
    /// </summary>
    public async Task<bool> PackAsync()
    {
        try
        {
            Console.WriteLine("Starting atlas packing...");

            // Load all sprites from inputs
            var allSprites = await LoadAllSpritesAsync();
            if (!allSprites.Any())
            {
                Console.WriteLine("No sprites found to pack.");
                return false;
            }

            Console.WriteLine($"Loaded {allSprites.Count} sprites");

            // Create packing rectangles
            var packingRects = allSprites.Select(s => new PackingRectangle(
                s.Image.Width, s.Image.Height, s.Name)).ToList();

            // Pack the rectangles
            var packer = new BinPacker(
                _config.Atlas.MaxWidth,
                _config.Atlas.MaxHeight,
                _config.Atlas.Padding,
                _config.Atlas.AllowRotation);

            var packingResult = packer.Pack(packingRects);
            if (packingResult == null)
            {
                Console.WriteLine("Failed to pack sprites into atlas. Consider increasing max dimensions or reducing sprite count.");
                return false;
            }

            Console.WriteLine($"Packing efficiency: {packingResult.Efficiency:P2}");
            Console.WriteLine($"Atlas size: {packingResult.ActualWidth}x{packingResult.ActualHeight}");

            // Calculate final dimensions
            var (finalWidth, finalHeight) = SpriteProcessor.CalculateFinalDimensions(
                packingResult.ActualWidth, packingResult.ActualHeight, _config.Atlas.PowerOfTwo);

            Console.WriteLine($"Final atlas size: {finalWidth}x{finalHeight}");

            // Create the atlas image
            using var atlasImage = SpriteProcessor.CreateAtlas(allSprites, packingResult, finalWidth, finalHeight);

            // Save the atlas image
            await SaveAtlasImageAsync(atlasImage, _config.Output.ImagePath);

            // Create and save atlas data
            var atlasData = CreateAtlasData(packingResult, finalWidth, finalHeight, allSprites);
            await SaveAtlasDataAsync(atlasData, _config.Output.DataPath);

            // Create and save animations if configured
            if (_config.Animations?.Any() == true && !string.IsNullOrEmpty(_config.Output.AnimationPath))
            {
                var animationCollection = CreateAnimationCollection(atlasData);
                await SaveAnimationDataAsync(animationCollection, _config.Output.AnimationPath);
            }

            // Cleanup
            foreach (var sprite in allSprites)
            {
                sprite.Dispose();
            }

            Console.WriteLine("Atlas packing completed successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during packing: {ex.Message}");
            return false;
        }
    }

    private async Task<List<SpriteInfo>> LoadAllSpritesAsync()
    {
        var allSprites = new List<SpriteInfo>();

        foreach (var input in _config.Inputs)
        {
            if (!File.Exists(input.ImagePath))
            {
                Console.WriteLine($"Warning: Image file not found: {input.ImagePath}");
                continue;
            }

            if (!string.IsNullOrEmpty(input.DataPath))
            {
                // Load sprites from sprite sheet with data
                if (!File.Exists(input.DataPath))
                {
                    Console.WriteLine($"Warning: Data file not found: {input.DataPath}");
                    continue;
                }

                var spriteData = await LoadSpriteDataAsync(input.DataPath);
                var sprites = SpriteProcessor.ExtractSprites(input.ImagePath, spriteData, input.Prefix);
                allSprites.AddRange(sprites);
            }
            else
            {
                // Load single sprite
                var spriteName = input.Prefix != null 
                    ? $"{input.Prefix}{Path.GetFileNameWithoutExtension(input.ImagePath)}"
                    : null;
                
                var sprite = SpriteProcessor.LoadSprite(input.ImagePath, spriteName, _config.Atlas.TrimSprites);
                allSprites.Add(sprite);
            }
        }

        return allSprites;
    }

    private async Task<List<SpriteData>> LoadSpriteDataAsync(string dataPath)
    {
        var jsonContent = await File.ReadAllTextAsync(dataPath);
        
        // Try to parse as AtlasData first, then as simple SpriteData array
        try
        {
            var atlasData = JsonSerializer.Deserialize<AtlasData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (atlasData?.Sprites != null)
            {
                return atlasData.Sprites;
            }
        }
        catch
        {
            // Try as direct sprite array
        }

        try
        {
            var spriteArray = JsonSerializer.Deserialize<List<SpriteData>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return spriteArray ?? new List<SpriteData>();
        }
        catch
        {
            Console.WriteLine($"Warning: Could not parse sprite data from {dataPath}");
            return new List<SpriteData>();
        }
    }

    private AtlasData CreateAtlasData(PackingResult packingResult, int atlasWidth, int atlasHeight, List<SpriteInfo> sprites)
    {
        var atlasData = new AtlasData
        {
            Width = atlasWidth,
            Height = atlasHeight,
            Metadata = new AtlasMetadata
            {
                Version = "1.0.0",
                Generated = DateTime.UtcNow,
                Sources = _config.Inputs.Select(i => i.ImagePath).ToList(),
                Settings = _config.Atlas
            }
        };

        foreach (var packedRect in packingResult.PackedRectangles)
        {
            var sprite = sprites.First(s => s.Name == packedRect.Name);
            
            var spriteData = new SpriteData
            {
                Name = packedRect.Name,
                X = packedRect.X,
                Y = packedRect.Y,
                Width = packedRect.Width,
                Height = packedRect.Height,
                Rotated = packedRect.Rotated
            };

            // Add trim information if the sprite was trimmed
            if (sprite.WasTrimmed)
            {
                spriteData.SourceWidth = sprite.OriginalWidth;
                spriteData.SourceHeight = sprite.OriginalHeight;
                spriteData.TrimX = sprite.TrimX;
                spriteData.TrimY = sprite.TrimY;
            }

            atlasData.Sprites.Add(spriteData);
        }

        return atlasData;
    }

    private AnimationCollection CreateAnimationCollection(AtlasData atlasData)
    {
        var collection = new AnimationCollection
        {
            AtlasFile = _config.Output.ImagePath
        };

        if (_config.Animations == null) return collection;

        foreach (var animConfig in _config.Animations)
        {
            var animation = new AnimationData
            {
                Name = animConfig.Name,
                DefaultDuration = animConfig.FrameDuration,
                Loop = animConfig.Loop
            };

            // Generate frames from pattern or use explicit frame list
            var frameNames = GenerateFrameNames(animConfig, atlasData);
            
            foreach (var frameName in frameNames)
            {
                // Verify the sprite exists in the atlas
                if (atlasData.Sprites.Any(s => s.Name == frameName))
                {
                    animation.Frames.Add(new AnimationFrame
                    {
                        Sprite = frameName,
                        Duration = null // Use default duration
                    });
                }
                else
                {
                    Console.WriteLine($"Warning: Animation frame '{frameName}' not found in atlas");
                }
            }

            if (animation.Frames.Any())
            {
                collection.Animations.Add(animation);
            }
        }

        return collection;
    }

    private List<string> GenerateFrameNames(AnimationConfig animConfig, AtlasData atlasData)
    {
        if (animConfig.Pattern != null)
        {
            // Generate frame names from pattern
            var frameNames = new List<string>();
            for (int i = animConfig.Pattern.StartFrame; i <= animConfig.Pattern.EndFrame; i++)
            {
                var frameName = string.Format(animConfig.Pattern.NamePattern, i);
                frameNames.Add(frameName);
            }
            return frameNames;
        }
        else
        {
            // Use explicit frame list
            return animConfig.Frames.ToList();
        }
    }

    private async Task SaveAtlasImageAsync(Image<Rgba32> atlasImage, string outputPath)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await atlasImage.SaveAsPngAsync(outputPath, new PngEncoder
        {
            CompressionLevel = PngCompressionLevel.BestCompression
        });

        Console.WriteLine($"Atlas image saved: {outputPath}");
    }

    private async Task SaveAtlasDataAsync(AtlasData atlasData, string outputPath)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(atlasData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(outputPath, json);
        Console.WriteLine($"Atlas data saved: {outputPath}");
    }

    private async Task SaveAnimationDataAsync(AnimationCollection animationCollection, string outputPath)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(animationCollection, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(outputPath, json);
        Console.WriteLine($"Animation data saved: {outputPath}");
    }
}