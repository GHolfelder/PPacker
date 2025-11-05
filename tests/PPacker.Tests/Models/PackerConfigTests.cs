using FluentAssertions;
using PPacker.Models;
using System.Text.Json;
using Xunit;

namespace PPacker.Tests.Models;

public class PackerConfigTests
{
    [Fact]
    public void PackerConfig_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var config = new PackerConfig
        {
            Inputs = new List<InputConfig>
            {
                new()
                {
                    ImagePath = "test.png",
                    DataPath = "test.json",
                    Prefix = "test_"
                }
            },
            Output = new OutputConfig
            {
                ImagePath = "atlas.png",
                DataPath = "atlas.json",
                AnimationPath = "animations.json"
            },
            Atlas = new AtlasConfig
            {
                MaxWidth = 1024,
                MaxHeight = 1024,
                Padding = 2,
                AllowRotation = true,
                TrimSprites = true,
                PowerOfTwo = false
            },
            Animations = new List<AnimationConfig>
            {
                new()
                {
                    Name = "walk",
                    Frames = new List<string> { "frame1", "frame2" },
                    FrameDuration = 100,
                    Loop = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        var deserializedConfig = JsonSerializer.Deserialize<PackerConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        deserializedConfig.Should().NotBeNull();
        deserializedConfig!.Inputs.Should().HaveCount(1);
        deserializedConfig.Inputs[0].ImagePath.Should().Be("test.png");
        deserializedConfig.Atlas.MaxWidth.Should().Be(1024);
        deserializedConfig.Animations.Should().HaveCount(1);
        deserializedConfig.Animations![0].Name.Should().Be("walk");
    }

    [Fact]
    public void AnimationConfig_WithPattern_ShouldHaveValidProperties()
    {
        // Arrange
        var animation = new AnimationConfig
        {
            Name = "test_anim",
            Pattern = new AnimationPattern
            {
                NamePattern = "sprite_{0:D2}",
                StartFrame = 1,
                EndFrame = 5
            },
            FrameDuration = 150,
            Loop = false
        };

        // Act & Assert
        animation.Name.Should().Be("test_anim");
        animation.Pattern.Should().NotBeNull();
        animation.Pattern!.NamePattern.Should().Be("sprite_{0:D2}");
        animation.Pattern.StartFrame.Should().Be(1);
        animation.Pattern.EndFrame.Should().Be(5);
        animation.FrameDuration.Should().Be(150);
        animation.Loop.Should().BeFalse();
    }
}