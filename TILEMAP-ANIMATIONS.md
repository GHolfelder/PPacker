# Tilemap Animation Support

## Overview

PPacker v1.0.10+ includes full support for animated tiles in Tiled maps (TMX/TSX format). This allows you to create animated tiles in Tiled and have them automatically processed into MonoGame-compatible data with precise texture coordinates.

## Tiled Editor Setup

### Creating Animated Tiles

1. In Tiled, open your tileset (TSX file)
2. Select a tile that you want to make animated
3. Go to the **Tile Animation Editor** (Window → Tile Animation Editor)
4. Add frames by clicking tiles in the tileset
5. Set duration for each frame in milliseconds
6. Save your tileset

### Example TSX Animation Definition

```xml
<tile id="40">
  <animation>
    <frame tileid="21" duration="100"/>
    <frame tileid="21" duration="100"/>
    <frame tileid="22" duration="100"/>
    <frame tileid="22" duration="100"/>
    <frame tileid="23" duration="100"/>
    <frame tileid="23" duration="100"/>
  </animation>
</tile>
```

## PPacker Processing

When PPacker processes a TMX map with animated tiles, it:

1. **Parses animation data** from TSX tilesets
2. **Calculates texture coordinates** for each animation frame within the atlas
3. **Outputs enhanced JSON** with ready-to-use MonoGame data

### Output Format

The generated map JSON includes animation data with precise texture coordinates:

```json
{
  "tilesets": [
    {
      "tiles": [
        {
          "id": 40,
          "animation": [
            {
              "tileId": 21,
              "duration": 100,
              "sourceX": 166,
              "sourceY": 67,
              "sourceWidth": 32,
              "sourceHeight": 32
            },
            {
              "tileId": 22,
              "duration": 100,
              "sourceX": 199,
              "sourceY": 67,
              "sourceWidth": 32,
              "sourceHeight": 32
            }
          ]
        }
      ]
    }
  ]
}
```

## MonoGame Integration

### Data Structures

Create corresponding classes in your MonoGame project:

```csharp
public class MapData
{
    public List<MapTileset> Tilesets { get; set; } = new();
    public List<MapTileLayer> TileLayers { get; set; } = new();
    // ... other properties
}

public class MapTileset
{
    public string AtlasSprite { get; set; } = string.Empty;
    public List<MapTile> Tiles { get; set; } = new();
    // ... other properties
}

public class MapTile
{
    public int Id { get; set; }
    public List<MapAnimationFrame>? Animation { get; set; }
    // ... other properties
}

public class MapAnimationFrame
{
    public int TileId { get; set; }
    public int Duration { get; set; }
    public int SourceX { get; set; }
    public int SourceY { get; set; }
    public int SourceWidth { get; set; }
    public int SourceHeight { get; set; }
}
```

### Loading and Using Animation Data

```csharp
public class TileAnimation
{
    private readonly List<MapAnimationFrame> _frames;
    private int _currentFrame;
    private double _frameTimer;
    
    public TileAnimation(List<MapAnimationFrame> frames)
    {
        _frames = frames;
    }
    
    public void Update(GameTime gameTime)
    {
        if (_frames.Count <= 1) return;
        
        _frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        
        if (_frameTimer >= _frames[_currentFrame].Duration)
        {
            _frameTimer = 0;
            _currentFrame = (_currentFrame + 1) % _frames.Count;
        }
    }
    
    public Rectangle GetCurrentSourceRectangle()
    {
        var frame = _frames[_currentFrame];
        return new Rectangle(frame.SourceX, frame.SourceY, 
                           frame.SourceWidth, frame.SourceHeight);
    }
}

// Usage in your tilemap renderer
public void DrawTile(SpriteBatch spriteBatch, Texture2D atlasTexture, 
                    MapTile tile, Vector2 position, TileAnimation? animation = null)
{
    Rectangle sourceRect;
    
    if (animation != null && tile.Animation?.Count > 0)
    {
        // Use animation frame
        sourceRect = animation.GetCurrentSourceRectangle();
    }
    else
    {
        // Calculate static tile source rectangle
        // (using tileset data for non-animated tiles)
        sourceRect = CalculateStaticTileSourceRect(tile);
    }
    
    spriteBatch.Draw(atlasTexture, position, sourceRect, Color.White);
}
```

### Tile Animation Manager

```csharp
public class TileAnimationManager
{
    private readonly Dictionary<int, TileAnimation> _animations = new();
    
    public void Initialize(MapData mapData)
    {
        foreach (var tileset in mapData.Tilesets)
        {
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Animation?.Count > 0)
                {
                    var globalTileId = tileset.FirstGid + tile.Id;
                    _animations[globalTileId] = new TileAnimation(tile.Animation);
                }
            }
        }
    }
    
    public void Update(GameTime gameTime)
    {
        foreach (var animation in _animations.Values)
        {
            animation.Update(gameTime);
        }
    }
    
    public TileAnimation? GetAnimation(int globalTileId)
    {
        return _animations.GetValueOrDefault(globalTileId);
    }
}
```

## Performance Considerations

### Animation Timing
- Frame durations are in milliseconds
- Consider using frame rates that are multiples of your game's target FPS for smoother animations
- Typical values: 100ms (10 FPS), 167ms (6 FPS), 250ms (4 FPS)

### Memory Usage
- Animation data is shared across all instances of the same tile
- Only store one `TileAnimation` instance per unique animated tile type
- Update animations globally, not per tile instance

### Rendering Optimization
- Batch animated tiles by their current frame to reduce texture state changes
- Consider using a texture atlas that groups animation frames together
- Pre-calculate source rectangles during initialization when possible

## Configuration Example

```json
{
  "inputs": [
    {
      "tmxPath": "sample-map.tmx",
      "prefix": "level1_"
    }
  ],
  "output": {
    "directory": "output",
    "atlasName": "level1-atlas",
    "mapOutput": "level1-map.json"
  },
  "atlasSettings": {
    "maxAtlasSize": { "width": 1024, "height": 1024 },
    "padding": 2,
    "allowRotation": false,
    "trimSprites": true,
    "powerOfTwo": true
  }
}
```

## Troubleshooting

### Common Issues

1. **Missing animation data**: Ensure your TSX file contains `<animation>` elements for the tiles
2. **Incorrect coordinates**: Verify tileset margin and spacing settings in Tiled match the TSX file
3. **Performance issues**: Limit the number of animated tiles and optimize frame durations

### Debugging Tips

- Use `--verbose` flag when running PPacker to see detailed processing information
- Check the generated JSON file to verify animation frames have correct coordinates
- Test with simple 2-3 frame animations first before creating complex sequences

## Migration from Previous Versions

If upgrading from an older version of PPacker:

1. **No breaking changes**: Existing functionality remains unchanged
2. **Enhanced output**: Animation frames now include `sourceX`, `sourceY`, `sourceWidth`, and `sourceHeight`
3. **Backward compatibility**: Old tile ID references are still included for compatibility