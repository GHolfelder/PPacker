using System.Text.Json.Serialization;

namespace PPacker.Models;

/// <summary>
/// Main configuration for the texture packer
/// </summary>
public class PackerConfig
{
    /// <summary>
    /// List of input sprite sheets to pack
    /// </summary>
    [JsonPropertyName("inputs")]
    public List<InputConfig> Inputs { get; set; } = new();

    /// <summary>
    /// Output configuration
    /// </summary>
    [JsonPropertyName("output")]
    public OutputConfig Output { get; set; } = new();

    /// <summary>
    /// Atlas generation settings
    /// </summary>
    [JsonPropertyName("atlas")]
    public AtlasConfig Atlas { get; set; } = new();

    /// <summary>
    /// Animation definitions (optional)
    /// </summary>
    [JsonPropertyName("animations")]
    public List<AnimationConfig>? Animations { get; set; }
}

/// <summary>
/// Configuration for input files
/// </summary>
public class InputConfig
{
    /// <summary>
    /// Path to the PNG file
    /// </summary>
    [JsonPropertyName("imagePath")]
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Path to the JSON sprite data file (optional)
    /// </summary>
    [JsonPropertyName("dataPath")]
    public string? DataPath { get; set; }

    /// <summary>
    /// Prefix to add to sprite names from this input
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    /// <summary>
    /// Path to Tiled map file (TMX) for tile map processing
    /// </summary>
    [JsonPropertyName("tmxPath")]
    public string? TmxPath { get; set; }
}

/// <summary>
/// Output configuration
/// </summary>
public class OutputConfig
{
    /// <summary>
    /// Output PNG file path
    /// </summary>
    [JsonPropertyName("imagePath")]
    public string ImagePath { get; set; } = "atlas.png";

    /// <summary>
    /// Output JSON data file path
    /// </summary>
    [JsonPropertyName("dataPath")]
    public string DataPath { get; set; } = "atlas.json";

    /// <summary>
    /// Animation data file path (if animations are defined)
    /// </summary>
    [JsonPropertyName("animationPath")]
    public string? AnimationPath { get; set; }

    /// <summary>
    /// Path to save map JSON file for Tiled maps (optional)
    /// </summary>
    [JsonPropertyName("mapPath")]
    public string? MapPath { get; set; }
}

/// <summary>
/// Atlas generation configuration
/// </summary>
public class AtlasConfig
{
    /// <summary>
    /// Maximum width of the atlas
    /// </summary>
    [JsonPropertyName("maxWidth")]
    public int MaxWidth { get; set; } = 2048;

    /// <summary>
    /// Maximum height of the atlas
    /// </summary>
    [JsonPropertyName("maxHeight")]
    public int MaxHeight { get; set; } = 2048;

    /// <summary>
    /// Padding between sprites in pixels
    /// </summary>
    [JsonPropertyName("padding")]
    public int Padding { get; set; } = 1;

    /// <summary>
    /// Whether to allow rotation of sprites for better packing
    /// </summary>
    [JsonPropertyName("allowRotation")]
    public bool AllowRotation { get; set; } = false;

    /// <summary>
    /// Whether to trim transparent pixels from sprites
    /// </summary>
    [JsonPropertyName("trimSprites")]
    public bool TrimSprites { get; set; } = true;

    /// <summary>
    /// Power of 2 sizing for the atlas
    /// </summary>
    [JsonPropertyName("powerOfTwo")]
    public bool PowerOfTwo { get; set; } = true;
}

/// <summary>
/// Animation configuration
/// </summary>
public class AnimationConfig
{
    /// <summary>
    /// Name of the animation
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of sprite names that make up this animation
    /// </summary>
    [JsonPropertyName("frames")]
    public List<string> Frames { get; set; } = new();

    /// <summary>
    /// Duration of each frame in milliseconds
    /// </summary>
    [JsonPropertyName("frameDuration")]
    public int FrameDuration { get; set; } = 100;

    /// <summary>
    /// Whether the animation should loop
    /// </summary>
    [JsonPropertyName("loop")]
    public bool Loop { get; set; } = true;

    /// <summary>
    /// Pattern-based frame generation (alternative to manual frames list)
    /// </summary>
    [JsonPropertyName("pattern")]
    public AnimationPattern? Pattern { get; set; }
}

/// <summary>
/// Pattern for generating animation frames automatically
/// </summary>
public class AnimationPattern
{
    /// <summary>
    /// Base name pattern (e.g., "walk_{0:D2}" for walk_01, walk_02, etc.)
    /// </summary>
    [JsonPropertyName("namePattern")]
    public string NamePattern { get; set; } = string.Empty;

    /// <summary>
    /// Starting frame number
    /// </summary>
    [JsonPropertyName("startFrame")]
    public int StartFrame { get; set; } = 1;

    /// <summary>
    /// Ending frame number
    /// </summary>
    [JsonPropertyName("endFrame")]
    public int EndFrame { get; set; } = 1;
}