using System.Text.Json.Serialization;

namespace PPacker.Models;

/// <summary>
/// Represents sprite data in the atlas
/// </summary>
public class SpriteData
{
    /// <summary>
    /// Name/identifier of the sprite
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// X coordinate in the atlas
    /// </summary>
    [JsonPropertyName("x")]
    public int X { get; set; }

    /// <summary>
    /// Y coordinate in the atlas
    /// </summary>
    [JsonPropertyName("y")]
    public int Y { get; set; }

    /// <summary>
    /// Width of the sprite
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Height of the sprite
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// Whether the sprite is rotated in the atlas
    /// </summary>
    [JsonPropertyName("rotated")]
    public bool Rotated { get; set; }

    /// <summary>
    /// Original width before trimming (if trimming was applied)
    /// </summary>
    [JsonPropertyName("sourceWidth")]
    public int? SourceWidth { get; set; }

    /// <summary>
    /// Original height before trimming (if trimming was applied)
    /// </summary>
    [JsonPropertyName("sourceHeight")]
    public int? SourceHeight { get; set; }

    /// <summary>
    /// X offset of the trimmed area
    /// </summary>
    [JsonPropertyName("trimX")]
    public int? TrimX { get; set; }

    /// <summary>
    /// Y offset of the trimmed area
    /// </summary>
    [JsonPropertyName("trimY")]
    public int? TrimY { get; set; }
}

/// <summary>
/// Atlas metadata and sprite definitions
/// </summary>
public class AtlasData
{
    /// <summary>
    /// Width of the atlas texture
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Height of the atlas texture
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// List of sprites in the atlas
    /// </summary>
    [JsonPropertyName("sprites")]
    public List<SpriteData> Sprites { get; set; } = new();

    /// <summary>
    /// Metadata about the atlas generation
    /// </summary>
    [JsonPropertyName("metadata")]
    public AtlasMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Metadata about atlas generation
/// </summary>
public class AtlasMetadata
{
    /// <summary>
    /// Version of PPacker that generated this atlas
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Timestamp when the atlas was generated
    /// </summary>
    [JsonPropertyName("generated")]
    public DateTime Generated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// List of source files used to generate the atlas
    /// </summary>
    [JsonPropertyName("sources")]
    public List<string> Sources { get; set; } = new();

    /// <summary>
    /// Settings used for atlas generation
    /// </summary>
    [JsonPropertyName("settings")]
    public AtlasConfig Settings { get; set; } = new();
}

/// <summary>
/// Animation definition for sprite sequences
/// </summary>
public class AnimationData
{
    /// <summary>
    /// Name of the animation
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of animation frames
    /// </summary>
    [JsonPropertyName("frames")]
    public List<AnimationFrame> Frames { get; set; } = new();

    /// <summary>
    /// Default duration for frames (can be overridden per frame)
    /// </summary>
    [JsonPropertyName("defaultDuration")]
    public int DefaultDuration { get; set; } = 100;

    /// <summary>
    /// Whether the animation loops
    /// </summary>
    [JsonPropertyName("loop")]
    public bool Loop { get; set; } = true;

    /// <summary>
    /// Total duration of the animation in milliseconds
    /// </summary>
    [JsonPropertyName("totalDuration")]
    public int TotalDuration => Frames.Sum(f => f.Duration ?? DefaultDuration);
}

/// <summary>
/// Individual frame in an animation
/// </summary>
public class AnimationFrame
{
    /// <summary>
    /// Name of the sprite for this frame
    /// </summary>
    [JsonPropertyName("sprite")]
    public string Sprite { get; set; } = string.Empty;

    /// <summary>
    /// Duration of this frame in milliseconds (optional, uses default if not set)
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}

/// <summary>
/// Collection of all animations
/// </summary>
public class AnimationCollection
{
    /// <summary>
    /// List of animations
    /// </summary>
    [JsonPropertyName("animations")]
    public List<AnimationData> Animations { get; set; } = new();

    /// <summary>
    /// Reference to the atlas file these animations are for
    /// </summary>
    [JsonPropertyName("atlasFile")]
    public string AtlasFile { get; set; } = string.Empty;
}