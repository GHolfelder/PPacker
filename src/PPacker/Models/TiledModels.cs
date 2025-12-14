using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PPacker.Models;

/// <summary>
/// Represents a Tiled map (TMX format) with layers and tilesets
/// </summary>
[XmlRoot("map")]
public class TiledMap
{
    /// <summary>
    /// Map format version
    /// </summary>
    [XmlAttribute("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Tiled version used to save the file
    /// </summary>
    [XmlAttribute("tiledversion")]
    public string TiledVersion { get; set; } = string.Empty;

    /// <summary>
    /// Map orientation (orthogonal, isometric, staggered, hexagonal)
    /// </summary>
    [XmlAttribute("orientation")]
    public string Orientation { get; set; } = "orthogonal";

    /// <summary>
    /// Render order (right-down, right-up, left-down, left-up)
    /// </summary>
    [XmlAttribute("renderorder")]
    public string RenderOrder { get; set; } = "right-down";

    /// <summary>
    /// Map width in tiles
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Map height in tiles
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Tile width in pixels
    /// </summary>
    [XmlAttribute("tilewidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Tile height in pixels
    /// </summary>
    [XmlAttribute("tileheight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Whether the map is infinite
    /// </summary>
    [XmlAttribute("infinite")]
    public int Infinite { get; set; }

    /// <summary>
    /// Map background color
    /// </summary>
    [XmlAttribute("backgroundcolor")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Tilesets used by this map
    /// </summary>
    [XmlElement("tileset")]
    public List<TiledTilesetRef> Tilesets { get; set; } = new();

    /// <summary>
    /// Layers in this map
    /// </summary>
    [XmlElement("layer")]
    public List<TiledLayer> Layers { get; set; } = new();

    /// <summary>
    /// Object groups in this map
    /// </summary>
    [XmlElement("objectgroup")]
    public List<TiledObjectGroup> ObjectGroups { get; set; } = new();

    /// <summary>
    /// Image layers in this map
    /// </summary>
    [XmlElement("imagelayer")]
    public List<TiledImageLayer> ImageLayers { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Reference to a tileset in a map
/// </summary>
public class TiledTilesetRef
{
    /// <summary>
    /// First global tile ID
    /// </summary>
    [XmlAttribute("firstgid")]
    public int FirstGid { get; set; }

    /// <summary>
    /// External tileset file (.tsx)
    /// </summary>
    [XmlAttribute("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Inline tileset data (when not using external file)
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    [XmlAttribute("tilewidth")]
    public int TileWidth { get; set; }

    [XmlAttribute("tileheight")]
    public int TileHeight { get; set; }

    [XmlAttribute("tilecount")]
    public int TileCount { get; set; }

    [XmlAttribute("columns")]
    public int Columns { get; set; }

    /// <summary>
    /// Margin around the tileset image
    /// </summary>
    [XmlAttribute("margin")]
    public int Margin { get; set; }

    /// <summary>
    /// Spacing between tiles
    /// </summary>
    [XmlAttribute("spacing")]
    public int Spacing { get; set; }

    /// <summary>
    /// Image for inline tileset
    /// </summary>
    [XmlElement("image")]
    public TiledImage? Image { get; set; }

    /// <summary>
    /// Individual tile definitions
    /// </summary>
    [XmlElement("tile")]
    public List<TiledTile> Tiles { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// External tileset definition (TSX format)
/// </summary>
[XmlRoot("tileset")]
public class TiledTileset
{
    /// <summary>
    /// Tileset name
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tile width in pixels
    /// </summary>
    [XmlAttribute("tilewidth")]
    public int TileWidth { get; set; }

    /// <summary>
    /// Tile height in pixels
    /// </summary>
    [XmlAttribute("tileheight")]
    public int TileHeight { get; set; }

    /// <summary>
    /// Number of tiles in the tileset
    /// </summary>
    [XmlAttribute("tilecount")]
    public int TileCount { get; set; }

    /// <summary>
    /// Number of tile columns
    /// </summary>
    [XmlAttribute("columns")]
    public int Columns { get; set; }

    /// <summary>
    /// Margin around the tileset image
    /// </summary>
    [XmlAttribute("margin")]
    public int Margin { get; set; }

    /// <summary>
    /// Spacing between tiles
    /// </summary>
    [XmlAttribute("spacing")]
    public int Spacing { get; set; }

    /// <summary>
    /// Tileset image
    /// </summary>
    [XmlElement("image")]
    public TiledImage? Image { get; set; }

    /// <summary>
    /// Individual tile definitions
    /// </summary>
    [XmlElement("tile")]
    public List<TiledTile> Tiles { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Tile layer containing tile data
/// </summary>
public class TiledLayer
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer width in tiles
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Layer height in tiles
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [XmlAttribute("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [XmlAttribute("offsetx")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [XmlAttribute("offsety")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Tile data
    /// </summary>
    [XmlElement("data")]
    public TiledLayerData? Data { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Object group layer containing objects
/// </summary>
public class TiledObjectGroup
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [XmlAttribute("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [XmlAttribute("offsetx")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [XmlAttribute("offsety")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Objects in this layer
    /// </summary>
    [XmlElement("object")]
    public List<TiledObject> Objects { get; set; } = new();

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Image layer
/// </summary>
public class TiledImageLayer
{
    /// <summary>
    /// Layer ID
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Layer name
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Layer opacity (0-1)
    /// </summary>
    [XmlAttribute("opacity")]
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether layer is visible
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Horizontal offset in pixels
    /// </summary>
    [XmlAttribute("offsetx")]
    public float OffsetX { get; set; }

    /// <summary>
    /// Vertical offset in pixels
    /// </summary>
    [XmlAttribute("offsety")]
    public float OffsetY { get; set; }

    /// <summary>
    /// Layer image
    /// </summary>
    [XmlElement("image")]
    public TiledImage? Image { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Layer data containing tile information
/// </summary>
public class TiledLayerData
{
    /// <summary>
    /// Data encoding (csv, base64)
    /// </summary>
    [XmlAttribute("encoding")]
    public string? Encoding { get; set; }

    /// <summary>
    /// Data compression (gzip, zlib, zstd)
    /// </summary>
    [XmlAttribute("compression")]
    public string? Compression { get; set; }

    /// <summary>
    /// Raw tile data as text
    /// </summary>
    [XmlText]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Individual tile elements (when not using encoding)
    /// </summary>
    [XmlElement("tile")]
    public List<TiledDataTile> Tiles { get; set; } = new();
}

/// <summary>
/// Individual tile in layer data
/// </summary>
public class TiledDataTile
{
    /// <summary>
    /// Global tile ID
    /// </summary>
    [XmlAttribute("gid")]
    public uint Gid { get; set; }
}

/// <summary>
/// Object in an object layer
/// </summary>
public class TiledObject
{
    /// <summary>
    /// Object ID
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Object name
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Object type
    /// </summary>
    [XmlAttribute("type")]
    public string? Type { get; set; }

    /// <summary>
    /// X position in pixels
    /// </summary>
    [XmlAttribute("x")]
    public float X { get; set; }

    /// <summary>
    /// Y position in pixels
    /// </summary>
    [XmlAttribute("y")]
    public float Y { get; set; }

    /// <summary>
    /// Width in pixels
    /// </summary>
    [XmlAttribute("width")]
    public float Width { get; set; }

    /// <summary>
    /// Height in pixels
    /// </summary>
    [XmlAttribute("height")]
    public float Height { get; set; }

    /// <summary>
    /// Rotation in degrees
    /// </summary>
    [XmlAttribute("rotation")]
    public float Rotation { get; set; }

    /// <summary>
    /// Global tile ID (for tile objects)
    /// </summary>
    [XmlAttribute("gid")]
    public uint Gid { get; set; }

    /// <summary>
    /// Whether object is visible
    /// </summary>
    [XmlAttribute("visible")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// Polygon points
    /// </summary>
    [XmlElement("polygon")]
    public TiledPolygon? Polygon { get; set; }

    /// <summary>
    /// Polyline points
    /// </summary>
    [XmlElement("polyline")]
    public TiledPolyline? Polyline { get; set; }

    /// <summary>
    /// Ellipse element (indicates object is an ellipse)
    /// </summary>
    [XmlElement("ellipse")]
    public TiledEllipse? Ellipse { get; set; }

    /// <summary>
    /// Point element (indicates object is a point)
    /// </summary>
    [XmlElement("point")]
    public TiledPoint? Point { get; set; }

    /// <summary>
    /// Text content
    /// </summary>
    [XmlElement("text")]
    public TiledText? Text { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Individual tile definition in a tileset
/// </summary>
public class TiledTile
{
    /// <summary>
    /// Tile ID (local to tileset)
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Tile type
    /// </summary>
    [XmlAttribute("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Individual tile image (for image collection tilesets)
    /// </summary>
    [XmlElement("image")]
    public TiledImage? Image { get; set; }

    /// <summary>
    /// Animation frames
    /// </summary>
    [XmlArray("animation")]
    [XmlArrayItem("frame")]
    public List<TiledAnimationFrame>? Animation { get; set; }

    /// <summary>
    /// Collision data
    /// </summary>
    [XmlElement("objectgroup")]
    public TiledObjectGroup? ObjectGroup { get; set; }

    /// <summary>
    /// Custom properties
    /// </summary>
    [XmlElement("properties")]
    public TiledProperties? Properties { get; set; }
}

/// <summary>
/// Image reference
/// </summary>
public class TiledImage
{
    /// <summary>
    /// Image file path
    /// </summary>
    [XmlAttribute("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Image width
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// Image height
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// Transparent color
    /// </summary>
    [XmlAttribute("trans")]
    public string? Trans { get; set; }
}

/// <summary>
/// Animation frame
/// </summary>
public class TiledAnimationFrame
{
    /// <summary>
    /// Tile ID for this frame
    /// </summary>
    [XmlAttribute("tileid")]
    public int TileId { get; set; }

    /// <summary>
    /// Frame duration in milliseconds
    /// </summary>
    [XmlAttribute("duration")]
    public int Duration { get; set; }
}

/// <summary>
/// Custom properties container
/// </summary>
public class TiledProperties
{
    /// <summary>
    /// Property list
    /// </summary>
    [XmlElement("property")]
    public List<TiledProperty> Properties { get; set; } = new();
}

/// <summary>
/// Individual custom property
/// </summary>
public class TiledProperty
{
    /// <summary>
    /// Property name
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Property type (string, int, float, bool, color, file, object)
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; } = "string";

    /// <summary>
    /// Property value
    /// </summary>
    [XmlAttribute("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Property value as text content
    /// </summary>
    [XmlText]
    public string? Content { get; set; }
}

/// <summary>
/// Polygon shape
/// </summary>
public class TiledPolygon
{
    /// <summary>
    /// Point coordinates as comma-separated pairs
    /// </summary>
    [XmlAttribute("points")]
    public string Points { get; set; } = string.Empty;
}

/// <summary>
/// Polyline shape
/// </summary>
public class TiledPolyline
{
    /// <summary>
    /// Point coordinates as comma-separated pairs
    /// </summary>
    [XmlAttribute("points")]
    public string Points { get; set; } = string.Empty;
}

/// <summary>
/// Text object
/// </summary>
public class TiledText
{
    /// <summary>
    /// Font family
    /// </summary>
    [XmlAttribute("fontfamily")]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Font size in pixels
    /// </summary>
    [XmlAttribute("pixelsize")]
    public int PixelSize { get; set; } = 16;

    /// <summary>
    /// Text color
    /// </summary>
    [XmlAttribute("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether text is bold
    /// </summary>
    [XmlAttribute("bold")]
    public int Bold { get; set; }

    /// <summary>
    /// Whether text is italic
    /// </summary>
    [XmlAttribute("italic")]
    public int Italic { get; set; }

    /// <summary>
    /// Text content
    /// </summary>
    [XmlText]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Ellipse element (empty tag indicating object is an ellipse)
/// </summary>
public class TiledEllipse
{
    // Empty class - presence of this element indicates ellipse type
}

/// <summary>
/// Point element (empty tag indicating object is a point)
/// </summary>
public class TiledPoint
{
    // Empty class - presence of this element indicates point type
}