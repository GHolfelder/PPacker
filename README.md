# PPacker

A command-line tool for packing PNG files into texture atlases for MonoGame projects.

## Features

- **Texture Atlas Packing**: Pack multiple PNG files into optimized texture atlases
- **Fixed Sprite Positioning**: Place specific sprites at exact coordinates with priority-based packing
- **Aseprite Support**: Native support for Aseprite JSON exports with frame data
- **Tiled Map Support**: Process TMX map files and TSX tilesets with tile animations
- **Animation Support**: Generate animation definitions from sprite sequences
- **Smart Packing**: Bin packing algorithm with rotation and trimming options
- **MonoGame Ready**: Output format compatible with MonoGame content pipeline

## Installation

### Standalone Executable (Recommended)

Download the latest standalone executable for your platform from the [Releases page](https://github.com/GHolfelder/PPacker/releases):

- **Windows**: `PPacker.exe` 
- **Linux**: `PPacker`
- **macOS**: `PPacker`

### From Source

```bash
git clone https://github.com/MonoGame/PPacker.git
cd PPacker
dotnet build -c Release
```

### Global Tool

```bash
dotnet tool install -g PPacker
```

## Quick Start

1. **Download PPacker**: Get the standalone executable for your platform
2. **Generate example configuration**:
   ```bash
   PPacker example --output ./my-project
   ```
3. **Organize your sprites** according to the configuration paths
4. **Run the packer**:
   ```bash
   PPacker --config ./my-project/ppacker-config.json
   ```

## Configuration

PPacker uses a JSON configuration file to define inputs, outputs, and packing settings.

```json
{
  "inputs": [
    {
      "imagePath": "sprites/player.png",
      "prefix": "player_"
    },
    {
      "imagePath": "sprites/enemies.png",
      "dataPath": "sprites/enemies.json",
      "prefix": "enemy_"
    }
  ],
  "output": {
    "imagePath": "output/atlas.png",
    "dataPath": "output/atlas.json",
    "animationPath": "output/animations.json"
  },
  "atlas": {
    "maxWidth": 2048,
    "maxHeight": 2048,
    "padding": 2,
    "allowRotation": false,
    "trimSprites": true,
    "powerOfTwo": true
  }
}
```

### Input Types

- **Individual Sprite**: Single PNG file
- **Sprite Sheet with Metadata**: PNG with JSON data file
- **Aseprite Export**: Aseprite sprite sheet with JSON frame data
- **Tiled Map**: TMX map files with TSX tilesets

For detailed configuration options, see the [examples](examples/) directory.

## Command Line Usage

```bash
# Basic packing
ppacker --config config.json

# With verbose output
ppacker --config config.json --verbose

# Override output directory
ppacker --config config.json --output-dir ./build

# Generate examples
ppacker example --output ./examples
```

## Release History

### v1.0.12 (Latest)
- Fixed positioning system for sprites at exact atlas coordinates
- Prefix-based TMX map naming with conflict prevention
- Array output consistency for map data
[**View detailed release notes**](RELEASE-NOTES-v1.0.12.md)

### v1.0.11
- Enhanced tilemap animation support with texture coordinates
- MonoGame-ready animation frames with pixel-perfect positioning
[**View detailed release notes**](RELEASE-NOTES-v1.0.11.md)

### v1.0.10
- Complete animation system for MonoGame integration
- Texture coordinate calculation for tileset animations
[**View detailed release notes**](RELEASE-NOTES-v1.0.10.md)

### v1.0.9
- Fixed bin packer boundary issues causing missing sprites
- Security updates for ImageSharp dependency
[**View detailed release notes**](RELEASE-NOTES-v1.0.9.md)

### v1.0.8
- Object geometry type detection for Tiled maps
- Enhanced MonoGame integration support
[**View detailed release notes**](RELEASE-NOTES-v1.0.8.md)

### v1.0.7
- Comprehensive Tiled map support (TMX/TSX files)
- Multi-layer and object support with property preservation
[**View detailed release notes**](RELEASE-NOTES-v1.0.7.md)

## Output Files

PPacker generates the following output files:

### Atlas Image (PNG)
A texture atlas containing all packed sprites.

### Atlas Data (JSON)
Sprite coordinate and metadata information:
```json
{
  "width": 2048,
  "height": 1024,
  "sprites": [
    {
      "name": "player_idle",
      "x": 0, "y": 0,
      "width": 32, "height": 48,
      "rotated": false
    }
  ]
}
```

### Animation Data (JSON)
Animation definitions with frame sequences:
```json
{
  "atlasFile": "atlas.png",
  "animations": [
    {
      "name": "player_walk",
      "frames": [
        { "sprite": "player_walk_01", "duration": 130 }
      ],
      "loop": true
    }
  ]
}
```

### Map Data (JSON)
Tiled map data for MonoGame integration (when processing TMX files).

## MonoGame Integration

### Loading the Atlas
```csharp
// Load texture and data
var atlasTexture = Content.Load<Texture2D>("atlas");
var atlasData = JsonSerializer.Deserialize<AtlasData>(
    File.ReadAllText("Content/atlas.json"));

// Create sprite regions
var sprites = new Dictionary<string, Rectangle>();
foreach (var sprite in atlasData.Sprites)
{
    sprites[sprite.Name] = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height);
}
```

### Animation System
```csharp
public class AnimationPlayer
{
    private AnimationData currentAnimation;
    private int currentFrame;
    private float frameTimer;
    
    public void PlayAnimation(string name) { /* ... */ }
    public void Update(float deltaTime) { /* ... */ }
    public string CurrentSpriteName => currentAnimation?.Frames[currentFrame].Sprite;
}
```

For complete MonoGame integration examples, see the [examples](examples/) directory.

## Building and Contributing

### Prerequisites
- .NET 8.0 SDK or later

### Build Commands
```bash
# Build and test
dotnet restore
dotnet build
dotnet test

# Create release
dotnet build -c Release
```

### Contributing
1. Fork the repository
2. Create a feature branch  
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## Troubleshooting

For common issues and solutions, see [TROUBLESHOOTING.md](TROUBLESHOOTING.md).

### Quick Fixes
- **Missing sprites**: Use `--verbose` to see processing details
- **File not found**: Check paths are relative to config file
- **Animation issues**: Verify frame names match exactly (case-sensitive)

## Documentation

- [Tilemap Animations](TILEMAP-ANIMATIONS.md) - Complete guide for animated tiles
- [Troubleshooting Guide](TROUBLESHOOTING.md) - Common issues and solutions
- [Examples](examples/) - Sample configurations and usage

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [ImageSharp](https://github.com/SixLabors/ImageSharp) for image processing
- [System.CommandLine](https://github.com/dotnet/command-line-api) for CLI interface