using System.Text.Json.Serialization;

namespace PPacker.Models;

/// <summary>
/// Map data for MonoGame consumption
/// </summary>
public class MapData
{
    /// <summary>
    /// Map name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Map width in tiles
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Map height in tiles
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// Tile width in pixels
    /// </summary>
    [JsonPropertyName("tileWidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Tile height in pixels
    /// </summary>
    [JsonPropertyName("tileHeight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Map orientation
    /// </summary>
    [JsonPropertyName("orientation")]
    public string Orientation { get; set; } = "orthogonal";

    /// <summary>
    /// Map background color
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Tilesets used in this map
    /// </summary>
    [JsonPropertyName("tilesets")]
    public List<MapTileset> Tilesets { get; set; } = new();

    /// <summary>
    /// Tile layers
    /// </summary>
    [JsonPropertyName("tileLayers")]
    public List<MapTileLayer> TileLayers { get; set; } = new();

    /// <summary>
    /// Object layers
    /// </summary>
    [JsonPropertyName("objectLayers")]
    public List<MapObjectLayer> ObjectLayers { get; set; } = new();

    /// <summary>
    /// Image layers
    /// </summary>
    [JsonPropertyName("imageLayers")]
    public List<MapImageLayer> ImageLayers { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Reference to the atlas file
    /// </summary>
    [JsonPropertyName("atlasFile")]
    public string AtlasFile { get; set; } = string.Empty;
}

/// <summary>
/// Tileset data for MonoGame
/// </summary>
public class MapTileset
{
    /// <summary>
    /// Tileset name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// First global tile ID
    /// </summary>
    [JsonPropertyName("firstGid")]
    public int FirstGid { get; set; }

    /// <summary>
    /// Tile width in pixels
    /// </summary>
    [JsonPropertyName("tileWidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Tile height in pixels
    /// </summary>
    [JsonPropertyName("tileHeight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Number of tiles
    /// </summary>
    [JsonPropertyName("tileCount")]
    public int TileCount { get; set; }

    /// <summary>
    /// Number of columns in tileset image
    /// </summary>
    [JsonPropertyName("columns")]
    public int Columns { get; set; }

    /// <summary>
    /// Margin around tileset
    /// </summary>
    [JsonPropertyName("margin")]
    public int Margin { get; set; }

    /// <summary>
    /// Spacing between tiles
    /// </summary>
    [JsonPropertyName("spacing")]
    public int Spacing { get; set; }

    /// <summary>
    /// Atlas sprite name for tileset image
    /// </summary>
    [JsonPropertyName("atlasSprite")]
    public string? AtlasSprite { get; set; }

    /// <summary>
    /// Individual tiles with metadata
    /// </summary>
    [JsonPropertyName("tiles")]
    public List<MapTile> Tiles { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Individual tile data
/// </summary>
public class MapTile
{
    /// <summary>
    /// Local tile ID in tileset
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Tile type
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Atlas sprite name for individual tile image
    /// </summary>
    [JsonPropertyName("atlasSprite")]
    public string? AtlasSprite { get; set; }

    /// <summary>
    /// Animation frames
    /// </summary>
    [JsonPropertyName("animation")]
    public List<MapAnimationFrame>? Animation { get; set; }

    /// <summary>
    /// Collision objects
    /// </summary>
    [JsonPropertyName("collisionObjects")]
    public List<MapObject>? CollisionObjects { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Animation frame data
/// </summary>
public class MapAnimationFrame
{
    /// <summary>
    /// Tile ID for this frame
    /// </summary>
    [JsonPropertyName("tileId")]
    public int TileId { get; set; }

    /// <summary>
    /// Frame duration in milliseconds
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
}

/// <summary>
/// Tile layer data
/// </summary>
public class MapTileLayer
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer width in tiles
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Layer height in tiles
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [JsonPropertyName("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [JsonPropertyName("offsetX")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [JsonPropertyName("offsetY")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Tile data as array of global tile IDs
    /// </summary>
    [JsonPropertyName("tiles")]
    public uint[] Tiles { get; set; } = Array.Empty<uint>();

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Object layer data
/// </summary>
public class MapObjectLayer
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [JsonPropertyName("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [JsonPropertyName("offsetX")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [JsonPropertyName("offsetY")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Objects in this layer
    /// </summary>
    [JsonPropertyName("objects")]
    public List<MapObject> Objects { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Image layer data
/// </summary>
public class MapImageLayer
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [JsonPropertyName("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [JsonPropertyName("offsetX")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [JsonPropertyName("offsetY")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Atlas sprite name for the image
    /// </summary>
    [JsonPropertyName("atlasSprite")]
    public string? AtlasSprite { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Map object data
/// </summary>
public class MapObject
{
    /// <summary>
    /// Object ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Object name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Object type
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Object geometry type (rectangle, ellipse, circle, point, polygon, polyline, text, tile)
    /// </summary>
    [JsonPropertyName("objectType")]
    public string ObjectType { get; set; } = "rectangle";

    /// <summary>
    /// X position in pixels
    /// </summary>
    [JsonPropertyName("x")]
    public float X { get; set; }

    /// <summary>
    /// Y position in pixels
    /// </summary>
    [JsonPropertyName("y")]
    public float Y { get; set; }

    /// <summary>
    /// Width in pixels
    /// </summary>
    [JsonPropertyName("width")]
    public float Width { get; set; }

    /// <summary>
    /// Height in pixels
    /// </summary>
    [JsonPropertyName("height")]
    public float Height { get; set; }

    /// <summary>
    /// Rotation in degrees
    /// </summary>
    [JsonPropertyName("rotation")]
    public float Rotation { get; set; }

    /// <summary>
    /// Global tile ID (for tile objects)
    /// </summary>
    [JsonPropertyName("gid")]
    public uint Gid { get; set; }

    /// <summary>
    /// Whether object is visible
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Polygon points (for polygon objects)
    /// </summary>
    [JsonPropertyName("polygon")]
    public List<MapPoint>? Polygon { get; set; }

    /// <summary>
    /// Polyline points (for polyline objects)
    /// </summary>
    [JsonPropertyName("polyline")]
    public List<MapPoint>? Polyline { get; set; }

    /// <summary>
    /// Text data (for text objects)
    /// </summary>
    [JsonPropertyName("text")]
    public MapText? Text { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Point data
/// </summary>
public class MapPoint
{
    /// <summary>
    /// X coordinate
    /// </summary>
    [JsonPropertyName("x")]
    public float X { get; set; }

    /// <summary>
    /// Y coordinate
    /// </summary>
    [JsonPropertyName("y")]
    public float Y { get; set; }
}

/// <summary>
/// Text object data
/// </summary>
public class MapText
{
    /// <summary>
    /// Font family
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Font size in pixels
    /// </summary>
    [JsonPropertyName("pixelSize")]
    public int PixelSize { get; set; } = 16;

    /// <summary>
    /// Text color
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether text is bold
    /// </summary>
    [JsonPropertyName("bold")]
    public bool Bold { get; set; }

    /// <summary>
    /// Whether text is italic
    /// </summary>
    [JsonPropertyName("italic")]
    public bool Italic { get; set; }

    /// <summary>
    /// Text content
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}