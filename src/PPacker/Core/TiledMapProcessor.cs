using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using PPacker.Models;

namespace PPacker.Core;

/// <summary>
/// Processor for Tiled map files (TMX/TSX)
/// </summary>
public static class TiledMapProcessor
{
    /// <summary>
    /// Load a TMX map file and its associated TSX tilesets
    /// </summary>
    public static async Task<TiledMap> LoadMapAsync(string tmxPath, bool verbose = false)
    {
        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Loading Tiled map: {tmxPath}");
        }

        if (!File.Exists(tmxPath))
        {
            throw new FileNotFoundException($"TMX file not found: {tmxPath}");
        }

        // Parse TMX file
        var serializer = new XmlSerializer(typeof(TiledMap));
        TiledMap map;
        
        try
        {
            using var stream = File.OpenRead(tmxPath);
            map = (TiledMap)(serializer.Deserialize(stream) ?? throw new InvalidOperationException("Failed to deserialize TMX file"));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse TMX file {tmxPath}: {ex.Message}", ex);
        }

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Parsed map: {map.Width}x{map.Height} tiles, {map.Tilesets.Count} tileset(s), {map.Layers.Count} layer(s)");
        }

        // Load external tilesets
        var mapDirectory = Path.GetDirectoryName(tmxPath) ?? string.Empty;
        for (int i = 0; i < map.Tilesets.Count; i++)
        {
            var tilesetRef = map.Tilesets[i];
            if (!string.IsNullOrEmpty(tilesetRef.Source))
            {
                var tsxPath = Path.Combine(mapDirectory, tilesetRef.Source);
                if (verbose)
                {
                    Console.WriteLine($"[VERBOSE] Loading external tileset: {tsxPath}");
                }

                var tileset = await LoadTilesetAsync(tsxPath, verbose);
                
                // Copy tileset data to the reference
                tilesetRef.Name = tileset.Name;
                tilesetRef.TileWidth = tileset.TileWidth;
                tilesetRef.TileHeight = tileset.TileHeight;
                tilesetRef.TileCount = tileset.TileCount;
                tilesetRef.Columns = tileset.Columns;
                tilesetRef.Image = tileset.Image;
                tilesetRef.Tiles = tileset.Tiles;
                tilesetRef.Properties = tileset.Properties;
            }
        }

        return map;
    }

    /// <summary>
    /// Load a TSX tileset file
    /// </summary>
    public static Task<TiledTileset> LoadTilesetAsync(string tsxPath, bool verbose = false)
    {
        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Loading tileset: {tsxPath}");
        }

        if (!File.Exists(tsxPath))
        {
            throw new FileNotFoundException($"TSX file not found: {tsxPath}");
        }

        var serializer = new XmlSerializer(typeof(TiledTileset));
        
        try
        {
            using var stream = File.OpenRead(tsxPath);
            var result = (TiledTileset)(serializer.Deserialize(stream) ?? throw new InvalidOperationException("Failed to deserialize TSX file"));
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse TSX file {tsxPath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extract all image paths from a map and its tilesets
    /// </summary>
    public static List<string> ExtractImagePaths(TiledMap map, string mapDirectory, bool verbose = false)
    {
        return ExtractImagePathsWithTilesetInfo(map, mapDirectory, verbose).imagePaths;
    }

    /// <summary>
    /// Extract all image paths from a map and its tilesets with tileset source information
    /// </summary>
    public static (List<string> imagePaths, Dictionary<string, string> tilesetSources) ExtractImagePathsWithTilesetInfo(TiledMap map, string mapDirectory, bool verbose = false)
    {
        var imagePaths = new List<string>();
        var tilesetSources = new Dictionary<string, string>(); // tileset name -> TSX path

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Extracting image paths from map");
        }

        // Extract tileset images
        foreach (var tileset in map.Tilesets)
        {
            // Determine the base directory for this tileset's images
            string baseDirectory;
            if (!string.IsNullOrEmpty(tileset.Source))
            {
                // External tileset - images are relative to the TSX file location
                var tsxPath = Path.Combine(mapDirectory, tileset.Source);
                baseDirectory = Path.GetDirectoryName(tsxPath) ?? mapDirectory;
                if (!string.IsNullOrEmpty(tileset.Name))
                {
                    tilesetSources[tileset.Name] = tsxPath;
                }
            }
            else
            {
                // Inline tileset - images are relative to the TMX file location
                baseDirectory = mapDirectory;
            }

            if (tileset.Image != null && !string.IsNullOrEmpty(tileset.Image.Source))
            {
                var imagePath = Path.Combine(baseDirectory, tileset.Image.Source);
                imagePaths.Add(imagePath);
                
                if (verbose)
                {
                    Console.WriteLine($"[VERBOSE] Found tileset image: {imagePath}");
                }
            }

            // Individual tile images (for image collection tilesets)
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Image != null && !string.IsNullOrEmpty(tile.Image.Source))
                {
                    var imagePath = Path.Combine(baseDirectory, tile.Image.Source);
                    imagePaths.Add(imagePath);
                    
                    if (verbose)
                    {
                        Console.WriteLine($"[VERBOSE] Found tile image: {imagePath}");
                    }
                }
            }
        }

        // Extract image layer images (always relative to map directory)
        foreach (var imageLayer in map.ImageLayers)
        {
            if (imageLayer.Image != null && !string.IsNullOrEmpty(imageLayer.Image.Source))
            {
                var imagePath = Path.Combine(mapDirectory, imageLayer.Image.Source);
                imagePaths.Add(imagePath);
                
                if (verbose)
                {
                    Console.WriteLine($"[VERBOSE] Found image layer: {imagePath}");
                }
            }
        }

        return (imagePaths.Distinct().ToList(), tilesetSources);
    }

    /// <summary>
    /// Parse layer data based on encoding and compression
    /// </summary>
    public static uint[] ParseLayerData(TiledLayerData data, int width, int height, bool verbose = false)
    {
        if (data.Tiles.Count > 0)
        {
            // Individual tile elements
            if (verbose)
            {
                Console.WriteLine($"[VERBOSE] Parsing layer data from individual tile elements");
            }
            return data.Tiles.Select(t => t.Gid).ToArray();
        }

        if (string.IsNullOrEmpty(data.Content))
        {
            return new uint[width * height];
        }

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Parsing layer data, encoding: {data.Encoding ?? "none"}, compression: {data.Compression ?? "none"}");
        }

        return data.Encoding?.ToLower() switch
        {
            "csv" => ParseCsvData(data.Content, verbose),
            "base64" => ParseBase64Data(data.Content, data.Compression, verbose),
            _ => ParseCsvData(data.Content, verbose) // Default to CSV
        };
    }

    /// <summary>
    /// Parse CSV encoded layer data
    /// </summary>
    private static uint[] ParseCsvData(string content, bool verbose = false)
    {
        var values = content.Trim()
            .Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => uint.Parse(s.Trim()))
            .ToArray();

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Parsed {values.Length} CSV tile values");
        }

        return values;
    }

    /// <summary>
    /// Parse Base64 encoded layer data
    /// </summary>
    private static uint[] ParseBase64Data(string content, string? compression, bool verbose = false)
    {
        var bytes = Convert.FromBase64String(content.Trim());

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Decoded {bytes.Length} bytes from Base64");
        }

        // Decompress if needed
        if (!string.IsNullOrEmpty(compression))
        {
            bytes = compression.ToLower() switch
            {
                "gzip" => DecompressGzip(bytes, verbose),
                "zlib" => DecompressZlib(bytes, verbose),
                "zstd" => throw new NotSupportedException("ZSTD compression is not supported"),
                _ => bytes
            };
        }

        // Convert to uint array (little-endian)
        var result = new uint[bytes.Length / 4];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = BitConverter.ToUInt32(bytes, i * 4);
        }

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Converted to {result.Length} tile values");
        }

        return result;
    }

    /// <summary>
    /// Decompress GZIP data
    /// </summary>
    private static byte[] DecompressGzip(byte[] data, bool verbose = false)
    {
        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Decompressing GZIP data ({data.Length} bytes)");
        }

        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        
        gzip.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    /// Decompress ZLIB data (Deflate with zlib header)
    /// </summary>
    private static byte[] DecompressZlib(byte[] data, bool verbose = false)
    {
        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Decompressing ZLIB data ({data.Length} bytes)");
        }

        // Skip zlib header (2 bytes) and use Deflate
        using var input = new MemoryStream(data, 2, data.Length - 2);
        using var deflate = new DeflateStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        
        deflate.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    /// Convert Tiled map to MonoGame map data
    /// </summary>
    public static MapData ConvertToMapData(TiledMap map, Dictionary<string, string> imageToSpriteMap, string atlasFile, bool verbose = false)
    {
        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Converting Tiled map to MonoGame format");
        }

        var mapData = new MapData
        {
            Name = Path.GetFileNameWithoutExtension(atlasFile), // Use atlas name as map name
            Width = map.Width,
            Height = map.Height,
            TileWidth = map.TileWidth,
            TileHeight = map.TileHeight,
            Orientation = map.Orientation,
            BackgroundColor = map.BackgroundColor,
            AtlasFile = atlasFile,
            Properties = ParseProperties(map.Properties)
        };

        // Convert tilesets
        foreach (var tileset in map.Tilesets)
        {
            var mapTileset = ConvertTileset(tileset, imageToSpriteMap, verbose);
            mapData.Tilesets.Add(mapTileset);
        }

        // Convert tile layers
        foreach (var layer in map.Layers)
        {
            var mapLayer = ConvertTileLayer(layer, verbose);
            mapData.TileLayers.Add(mapLayer);
        }

        // Convert object layers
        foreach (var objectGroup in map.ObjectGroups)
        {
            var mapObjectLayer = ConvertObjectLayer(objectGroup, verbose);
            mapData.ObjectLayers.Add(mapObjectLayer);
        }

        // Convert image layers
        foreach (var imageLayer in map.ImageLayers)
        {
            var mapImageLayer = ConvertImageLayer(imageLayer, imageToSpriteMap, verbose);
            mapData.ImageLayers.Add(mapImageLayer);
        }

        if (verbose)
        {
            Console.WriteLine($"[VERBOSE] Converted map with {mapData.Tilesets.Count} tilesets, {mapData.TileLayers.Count} tile layers, {mapData.ObjectLayers.Count} object layers, {mapData.ImageLayers.Count} image layers");
        }

        return mapData;
    }

    /// <summary>
    /// Convert tileset to map tileset
    /// </summary>
    private static MapTileset ConvertTileset(TiledTilesetRef tileset, Dictionary<string, string> imageToSpriteMap, bool verbose = false)
    {
        var mapTileset = new MapTileset
        {
            Name = tileset.Name ?? "unnamed",
            FirstGid = tileset.FirstGid,
            TileWidth = tileset.TileWidth,
            TileHeight = tileset.TileHeight,
            TileCount = tileset.TileCount,
            Columns = tileset.Columns,
            Properties = ParseProperties(tileset.Properties)
        };

        // Find atlas sprite for tileset image
        if (tileset.Image != null && !string.IsNullOrEmpty(tileset.Image.Source))
        {
            var imageKey = Path.GetFileNameWithoutExtension(tileset.Image.Source);
            if (imageToSpriteMap.TryGetValue(imageKey, out var spriteName))
            {
                mapTileset.AtlasSprite = spriteName;
            }
        }

        // Convert individual tiles
        foreach (var tile in tileset.Tiles)
        {
            var mapTile = ConvertTile(tile, imageToSpriteMap, verbose);
            mapTileset.Tiles.Add(mapTile);
        }

        return mapTileset;
    }

    /// <summary>
    /// Convert tile to map tile
    /// </summary>
    private static MapTile ConvertTile(TiledTile tile, Dictionary<string, string> imageToSpriteMap, bool verbose = false)
    {
        var mapTile = new MapTile
        {
            Id = tile.Id,
            Type = tile.Type,
            Properties = ParseProperties(tile.Properties)
        };

        // Find atlas sprite for individual tile image
        if (tile.Image != null && !string.IsNullOrEmpty(tile.Image.Source))
        {
            var imageKey = Path.GetFileName(tile.Image.Source);
            if (imageToSpriteMap.TryGetValue(imageKey, out var spriteName))
            {
                mapTile.AtlasSprite = spriteName;
            }
        }

        // Convert animation
        if (tile.Animation != null && tile.Animation.Count > 0)
        {
            mapTile.Animation = tile.Animation.Select(frame => new MapAnimationFrame
            {
                TileId = frame.TileId,
                Duration = frame.Duration
            }).ToList();
        }

        // Convert collision objects
        if (tile.ObjectGroup != null && tile.ObjectGroup.Objects.Count > 0)
        {
            mapTile.CollisionObjects = tile.ObjectGroup.Objects.Select(obj => ConvertObject(obj, verbose)).ToList();
        }

        return mapTile;
    }

    /// <summary>
    /// Convert tile layer
    /// </summary>
    private static MapTileLayer ConvertTileLayer(TiledLayer layer, bool verbose = false)
    {
        var tiles = Array.Empty<uint>();
        if (layer.Data != null)
        {
            tiles = ParseLayerData(layer.Data, layer.Width, layer.Height, verbose);
        }

        return new MapTileLayer
        {
            Id = layer.Id,
            Name = layer.Name,
            Width = layer.Width,
            Height = layer.Height,
            Opacity = layer.Opacity,
            Visible = layer.Visible == 1,
            OffsetX = layer.OffsetX,
            OffsetY = layer.OffsetY,
            Tiles = tiles,
            Properties = ParseProperties(layer.Properties)
        };
    }

    /// <summary>
    /// Convert object layer
    /// </summary>
    private static MapObjectLayer ConvertObjectLayer(TiledObjectGroup objectGroup, bool verbose = false)
    {
        return new MapObjectLayer
        {
            Id = objectGroup.Id,
            Name = objectGroup.Name,
            Opacity = objectGroup.Opacity,
            Visible = objectGroup.Visible == 1,
            OffsetX = objectGroup.OffsetX,
            OffsetY = objectGroup.OffsetY,
            Objects = objectGroup.Objects.Select(obj => ConvertObject(obj, verbose)).ToList(),
            Properties = ParseProperties(objectGroup.Properties)
        };
    }

    /// <summary>
    /// Convert image layer
    /// </summary>
    private static MapImageLayer ConvertImageLayer(TiledImageLayer imageLayer, Dictionary<string, string> imageToSpriteMap, bool verbose = false)
    {
        string? atlasSprite = null;
        
        if (imageLayer.Image != null && !string.IsNullOrEmpty(imageLayer.Image.Source))
        {
            var imageKey = Path.GetFileName(imageLayer.Image.Source);
            imageToSpriteMap.TryGetValue(imageKey, out atlasSprite);
        }

        return new MapImageLayer
        {
            Id = imageLayer.Id,
            Name = imageLayer.Name,
            Opacity = imageLayer.Opacity,
            Visible = imageLayer.Visible == 1,
            OffsetX = imageLayer.OffsetX,
            OffsetY = imageLayer.OffsetY,
            AtlasSprite = atlasSprite,
            Properties = ParseProperties(imageLayer.Properties)
        };
    }

    /// <summary>
    /// Convert object
    /// </summary>
    private static MapObject ConvertObject(TiledObject obj, bool verbose = false)
    {
        var mapObject = new MapObject
        {
            Id = obj.Id,
            Name = obj.Name,
            Type = obj.Type,
            X = obj.X,
            Y = obj.Y,
            Width = obj.Width,
            Height = obj.Height,
            Rotation = obj.Rotation,
            Gid = obj.Gid,
            Visible = obj.Visible == 1,
            Properties = ParseProperties(obj.Properties)
        };

        // Determine object geometry type based on XML elements present
        if (obj.Point != null)
        {
            mapObject.ObjectType = "point";
        }
        else if (obj.Ellipse != null)
        {
            // Check if it's a circle (width == height) or ellipse
            mapObject.ObjectType = Math.Abs(obj.Width - obj.Height) < 0.001f ? "circle" : "ellipse";
        }
        else if (obj.Polygon != null && !string.IsNullOrEmpty(obj.Polygon.Points))
        {
            mapObject.ObjectType = "polygon";
            mapObject.Polygon = ParsePoints(obj.Polygon.Points);
        }
        else if (obj.Polyline != null && !string.IsNullOrEmpty(obj.Polyline.Points))
        {
            mapObject.ObjectType = "polyline";
            mapObject.Polyline = ParsePoints(obj.Polyline.Points);
        }
        else if (obj.Text != null)
        {
            mapObject.ObjectType = "text";
            mapObject.Text = new MapText
            {
                FontFamily = obj.Text.FontFamily,
                PixelSize = obj.Text.PixelSize,
                Color = obj.Text.Color,
                Bold = obj.Text.Bold == 1,
                Italic = obj.Text.Italic == 1,
                Content = obj.Text.Content
            };
        }
        else if (obj.Gid > 0)
        {
            mapObject.ObjectType = "tile";
        }
        else
        {
            // Default to rectangle for regular objects with width/height
            mapObject.ObjectType = "rectangle";
        }

        return mapObject;
    }

    /// <summary>
    /// Parse properties from Tiled format
    /// </summary>
    private static Dictionary<string, object> ParseProperties(TiledProperties? properties)
    {
        var result = new Dictionary<string, object>();

        if (properties?.Properties == null) return result;

        foreach (var prop in properties.Properties)
        {
            var value = prop.Value ?? prop.Content ?? string.Empty;
            
            object parsedValue = prop.Type?.ToLower() switch
            {
                "int" => int.TryParse(value, out var i) ? i : 0,
                "float" => float.TryParse(value, out var f) ? f : 0f,
                "bool" => bool.TryParse(value, out var b) ? b : false,
                "color" => value,
                "file" => value,
                "object" => int.TryParse(value, out var oid) ? oid : 0,
                _ => value
            };

            result[prop.Name] = parsedValue;
        }

        return result;
    }

    /// <summary>
    /// Parse point coordinates from string
    /// </summary>
    private static List<MapPoint> ParsePoints(string pointsString)
    {
        var points = new List<MapPoint>();
        var pairs = pointsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var coords = pair.Split(',');
            if (coords.Length == 2 && 
                float.TryParse(coords[0], out var x) && 
                float.TryParse(coords[1], out var y))
            {
                points.Add(new MapPoint { X = x, Y = y });
            }
        }

        return points;
    }
}