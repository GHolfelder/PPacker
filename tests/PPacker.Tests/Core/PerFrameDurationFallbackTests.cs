using System.Text.Json;
using FluentAssertions;
using PPacker.Core;
using PPacker.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace PPacker.Tests.Core;

public class PerFrameDurationFallbackTests
{
    [Fact]
    public async Task AsepriteDurations_MissingOnSomeFrames_ShouldFallbackToDefault()
    {
        // Arrange
        var tempRoot = Path.Combine(Path.GetTempPath(), "ppacker_test_fallback_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        try
        {
            var inputDir = Path.Combine(tempRoot, "input");
            var outputDir = Path.Combine(tempRoot, "output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            var sheetPath = Path.Combine(inputDir, "walk_E.png");
            var dataPath = Path.Combine(inputDir, "walk_E.json");
            var animPath = Path.Combine(outputDir, "animations.json");
            var atlasPath = Path.Combine(outputDir, "atlas.png");
            var atlasDataPath = Path.Combine(outputDir, "atlas.json");

            using (var img = new Image<Rgba32>(128, 64))
            {
                await img.SaveAsPngAsync(sheetPath);
            }

            // Frame 0 has duration, frame 1 lacks it (removed field entirely)
            var asepriteJson = """
            { "frames": [
               {
                "filename": "walk_E_0.aseprite",
                "frame": { "x": 0, "y": 0, "w": 64, "h": 64 },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
                "sourceSize": { "w": 64, "h": 64 },
                "duration": 150
               },
               {
                "filename": "walk_E_1.aseprite",
                "frame": { "x": 64, "y": 0, "w": 64, "h": 64 },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
                "sourceSize": { "w": 64, "h": 64 }
               }
             ]}
            """;
            await File.WriteAllTextAsync(dataPath, asepriteJson);

            var config = new PackerConfig
            {
                Inputs = new List<InputConfig>
                {
                    new() { ImagePath = sheetPath, DataPath = dataPath, Prefix = "player_" }
                },
                Output = new OutputConfig
                {
                    ImagePath = atlasPath,
                    DataPath = atlasDataPath,
                    AnimationPath = animPath
                },
                Atlas = new AtlasConfig
                {
                    MaxWidth = 512,
                    MaxHeight = 512,
                    Padding = 1,
                    AllowRotation = false,
                    TrimSprites = false,
                    PowerOfTwo = false
                },
                Animations = new List<AnimationConfig>
                {
                    new()
                    {
                        Name = "walk_east",
                        Pattern = new AnimationPattern
                        {
                            NamePattern = "player_walk_E_{0}",
                            StartFrame = 0,
                            EndFrame = 1
                        },
                        FrameDuration = 100,
                        Loop = true
                    }
                }
            };

            var packer = new AtlasPacker(config, verbose: false);

            // Act
            var ok = await packer.PackAsync();

            // Assert
            ok.Should().BeTrue();
            var json = await File.ReadAllTextAsync(animPath);
            var anims = JsonSerializer.Deserialize<AnimationCollection>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            anims.Should().NotBeNull();
            var anim = anims!.Animations.Single(a => a.Name == "walk_east");
            anim.DefaultDuration.Should().Be(100);
            anim.Frames.Should().HaveCount(2);
            var f0 = anim.Frames[0];
            var f1 = anim.Frames[1];
            f0.Sprite.Should().Be("player_walk_E_0");
            f1.Sprite.Should().Be("player_walk_E_1");
            f0.Duration.Should().Be(150); // explicit
            f1.Duration.Should().BeNull(); // fallback expected
            anim.TotalDuration.Should().Be(250); // 150 + 100(default)
        }
        finally
        {
            try { Directory.Delete(tempRoot, true); } catch { }
        }
    }
}
