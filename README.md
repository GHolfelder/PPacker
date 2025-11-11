# PPacker

A powerful command-line tool for packing PNG files into texture atlases for MonoGame projects, with comprehensive sprite animation support and Aseprite integration.

## Features

- **Texture Atlas Packing**: Efficiently pack multiple PNG files into a single texture atlas
- **Sprite Data Merging**: Merge JSON sprite definitions and automatically update coordinates
- **Aseprite Support**: Native support for Aseprite JSON exports - automatically detects and parses frame data
- **Animation Support**: Create animation definitions from sprite sequences with pattern matching
- **Flexible Input**: Support for individual sprites, existing sprite sheets, or Aseprite exports
- **Smart Packing**: Bin packing algorithm with optional rotation for optimal space usage
- **Sprite Trimming**: Automatically remove transparent borders to save space
- **Power-of-2 Support**: Generate power-of-2 atlas dimensions for optimal GPU performance
- **Enhanced Debugging**: Comprehensive verbose logging to troubleshoot packing issues
- **Graceful Fallbacks**: Missing data files automatically fall back to single sprite processing
- **MonoGame Ready**: Output format compatible with MonoGame content pipeline

## Installation

### Standalone Executable (Recommended)

Download the latest standalone executable for your platform from the [Releases page](https://github.com/GHolfelder/PPacker/releases):

- **Windows**: `PPacker.exe` (self-contained, no .NET installation required)
- **Linux**: `PPacker` (self-contained, no .NET installation required)
- **macOS**: `PPacker` (self-contained, no .NET installation required)

### Building Standalone Executables

To build your own standalone executables:

```bash
# Windows x64
dotnet publish src\PPacker\PPacker.csproj -c Release -r win-x64 --self-contained true -o dist\win-x64

# Linux x64
dotnet publish src\PPacker\PPacker.csproj -c Release -r linux-x64 --self-contained true -o dist\linux-x64

# macOS x64
dotnet publish src\PPacker\PPacker.csproj -c Release -r osx-x64 --self-contained true -o dist\osx-x64

# macOS ARM64 (Apple Silicon)
dotnet publish src\PPacker\PPacker.csproj -c Release -r osx-arm64 --self-contained true -o dist\osx-arm64

# Or use the build script
.\build-standalone.ps1    # PowerShell
.\build-standalone.bat    # Windows Batch
```

### From Source

```bash
git clone https://github.com/MonoGame/PPacker.git
cd PPacker
dotnet build -c Release
dotnet pack -c Release
```

### Global Tool Installation

```bash
dotnet tool install -g PPacker
```

## Quick Start

1. **Download PPacker**: Get the standalone executable for your platform
2. **Generate example configuration**:
   ```bash
   # Windows
   PPacker.exe example --output ./my-project
   
   # Linux/macOS
   ./PPacker example --output ./my-project
   ```

3. **Organize your sprites** according to the configuration paths

4. **Run the packer**:
   ```bash
   # Windows
   PPacker.exe --config ./my-project/ppacker-config.json
   
   # Linux/macOS
   ./PPacker --config ./my-project/ppacker-config.json
   ```

## Configuration

PPacker uses a JSON configuration file to define inputs, outputs, and packing settings.

### Basic Configuration

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

### Input Configuration

Each input can be:

#### Individual Sprite
```json
{
  "imagePath": "sprites/player.png",
  "prefix": "player_"
}
```

#### Sprite Sheet with Metadata
```json
{
  "imagePath": "sprites/ui-elements.png",
  "dataPath": "sprites/ui-elements.json",
  "prefix": "ui_"
}
```

The data file should contain sprite definitions:
```json
{
  "width": 512,
  "height": 256,
  "sprites": [
    {
      "name": "button",
      "x": 0,
      "y": 0,
      "width": 100,
      "height": 32
    },
    {
      "name": "icon",
      "x": 100,
      "y": 0,
      "width": 24,
      "height": 24
    }
  ]
}
```

### Atlas Settings

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `maxWidth` | int | 2048 | Maximum atlas width |
| `maxHeight` | int | 2048 | Maximum atlas height |
| `padding` | int | 1 | Pixels between sprites |
| `allowRotation` | bool | false | Allow sprite rotation for better packing |
| `trimSprites` | bool | true | Remove transparent borders |
| `powerOfTwo` | bool | true | Force power-of-2 dimensions |

### Aseprite Integration

PPacker automatically detects and supports Aseprite JSON exports. Simply export your Aseprite animation as a sprite sheet with JSON data, and PPacker will handle the rest.

Starting with v1.0.6 per-frame Aseprite `duration` values are preserved in the generated `animations.json`. If an animation references sprites that came from an Aseprite sheet, each frame will use its original exported duration. Frames without a captured duration fall back to the animation's `defaultDuration`.

#### Aseprite Export Format

When you export from Aseprite with JSON data, you get this format:
```json
{
  "frames": [
    {
      "filename": "walk_E_0.aseprite",
      "frame": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 130
    },
    {
      "filename": "walk_E_1.aseprite", 
      "frame": { "x": 64, "y": 0, "w": 64, "h": 64 },
      "rotated": false,
      "trimmed": false,
      "spriteSourceSize": { "x": 0, "y": 0, "w": 64, "h": 64 },
      "sourceSize": { "w": 64, "h": 64 },
      "duration": 130
    }
  ]
}
```

#### Using Aseprite Data

```json
{
  "inputs": [
    {
      "imagePath": "animations/walk_east.png",
      "dataPath": "animations/walk_east.json",
      "prefix": "player_"
    }
  ]
}
```

PPacker will:
1. **Auto-detect** the Aseprite JSON format
2. **Extract frames** from the sprite sheet using the frame coordinates
3. **Generate sprite names** by removing the `.aseprite` extension (e.g., `walk_E_0`)
4. **Apply prefix** if specified (e.g., `player_walk_E_0`)
5. **Handle trimming** information automatically
6. **Store per-frame duration** from the `duration` property for use in animation output

#### Format Priority

PPacker tries to parse JSON data in this order:
1. **Aseprite format** (with `frames` array) - NEW!
2. **AtlasData format** (existing PPacker format)
3. **Direct SpriteData array** (simple array format)

### Animation Configuration

Define animations using frame patterns or explicit frame lists:

#### Pattern-Based Animation
```json
{
  "animations": [
    {
      "name": "walk",
      "pattern": {
        "namePattern": "player_walk_{0:D2}",
        "startFrame": 1,
        "endFrame": 8
      },
      "frameDuration": 100,
      "loop": true
    }
  ]
}
```

#### Aseprite Animation Example
Perfect for Aseprite exports where frames are numbered sequentially:
```json
{
  "animations": [
    {
      "name": "walk_east",
      "pattern": {
        "namePattern": "player_walk_E_{0}",
        "startFrame": 0,
        "endFrame": 2
      },
      "frameDuration": 130,
      "loop": true
    }
  ]
}
```

**Pattern Format Guide:**
- `{0}` → Plain numbers: `1`, `2`, `3`, `10`
- `{0:D2}` → Zero-padded 2 digits: `01`, `02`, `03`, `10`
- `{0:D3}` → Zero-padded 3 digits: `001`, `002`, `003`, `010`

#### Explicit Frame List
```json
{
  "animations": [
    {
      "name": "attack",
      "frames": ["player_attack_01", "player_attack_02", "player_attack_03"],
      "frameDuration": 150,
      "loop": false
    }
  ]
}
```

## Command Line Usage

### Basic Packing
```bash
ppacker --config config.json
```

### With Output Directory Override
```bash
ppacker --config config.json --output-dir ./build
```

### Verbose Output (Enhanced Debugging)
```bash
ppacker --config config.json --verbose
```

The verbose mode provides comprehensive debugging information including:
- **File Processing**: Shows each input file being processed
- **Format Detection**: Displays which JSON format is detected (Aseprite, AtlasData, etc.)
- **Sprite Loading**: Details about each sprite loaded and its dimensions
- **Fallback Logic**: When data files are missing, shows fallback to single sprite processing
- **Animation Generation**: Frame name generation and validation
- **Packing Details**: Bin packing progress and final atlas dimensions

Perfect for troubleshooting when sprites don't appear in the final atlas!

### Generate Examples
```bash
ppacker example --output ./examples
```

## Enhanced Features (v1.0.6)

### Intelligent Data File Handling
PPacker now gracefully handles missing or invalid data files:
- **Automatic Fallback**: If a `dataPath` is specified but the file doesn't exist, PPacker automatically falls back to processing the image as a single sprite
- **Multiple Format Support**: Supports Aseprite JSON, PPacker AtlasData, and direct SpriteData arrays
- **Clear Warnings**: Provides helpful warning messages when files are missing or invalid

### Comprehensive Error Debugging
The new verbose logging system helps you identify exactly where issues occur:
```bash
# Example verbose output
[VERBOSE] Processing Input #1:
[VERBOSE]   ImagePath: './sprites/player.png'
[VERBOSE]   DataPath: './sprites/player.json'
[VERBOSE]   Prefix: 'player_'
[VERBOSE] Image file exists: ./sprites/player.png
[VERBOSE] DataPath exists, loading sprite sheet data...
[VERBOSE] Parsed as Aseprite format with 4 frames
[VERBOSE] Converted Aseprite frame: 'walk_0' at (0,0) 64x64
[VERBOSE] Total sprites so far: 4
```

### Cross-Platform Compatibility
- Standalone executables require no .NET installation
- Consistent behavior across Windows, Linux, and macOS
- Automated builds for all platforms via GitHub Actions

## Output Files

### Atlas Image
A PNG file containing all packed sprites.

### Atlas Data (JSON)
```json
{
  "width": 2048,
  "height": 1024,
  "sprites": [
    {
      "name": "player_idle",
      "x": 0,
      "y": 0,
      "width": 32,
      "height": 48,
      "rotated": false,
      "sourceWidth": 32,
      "sourceHeight": 48,
      "trimX": 0,
      "trimY": 0
    }
  ],
  "metadata": {
  "version": "1.0.6",
    "generated": "2025-11-08T10:00:00Z",
    "sources": ["sprites/player.png"],
    "settings": { /* atlas configuration */ }
  }
}
```

### Animation Data (JSON)
```json
{
  "atlasFile": "atlas.png",
  "animations": [
    {
      "name": "player_walk",
      "frames": [
        { "sprite": "player_walk_01", "duration": 130 },
        { "sprite": "player_walk_02", "duration": 130 }
      ],
      "defaultDuration": 100,
      "loop": true,
      "totalDuration": 800
    }
  ]
}
```

Notes:
- `duration` per frame appears when sourced from Aseprite JSON. If absent it will be `null` and runtime code should use `defaultDuration`.
- `totalDuration` is the sum of explicit frame durations (or default where null).

## Integration with MonoGame

### Loading the Atlas
```csharp
// Load the atlas texture
var atlasTexture = Content.Load<Texture2D>("atlas");

// Parse the atlas data (using Newtonsoft.Json or System.Text.Json)
var atlasData = JsonSerializer.Deserialize<AtlasData>(
    File.ReadAllText("Content/atlas.json"));

// Create sprite regions
var sprites = new Dictionary<string, Rectangle>();
foreach (var sprite in atlasData.Sprites)
{
    sprites[sprite.Name] = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height);
}
```

### Using Animations
```csharp
// Load animation definitions
var animationData = JsonSerializer.Deserialize<AnimationCollection>(
    File.ReadAllText("Content/animations.json"));

// Create animation player
public class AnimationPlayer
{
    private AnimationData currentAnimation;
    private int currentFrame;
    private float frameTimer;
    
    public void PlayAnimation(string name)
    {
        currentAnimation = animationData.Animations.First(a => a.Name == name);
        currentFrame = 0;
        frameTimer = 0;
    }
    
    public void Update(float deltaTime)
    {
        if (currentAnimation == null) return;
        
        frameTimer += deltaTime * 1000; // Convert to milliseconds
        var frameDuration = currentAnimation.Frames[currentFrame].Duration 
            ?? currentAnimation.DefaultDuration;
            
        if (frameTimer >= frameDuration)
        {
            frameTimer = 0;
            currentFrame++;
            
            if (currentFrame >= currentAnimation.Frames.Count)
            {
                if (currentAnimation.Loop)
                    currentFrame = 0;
                else
                    currentFrame = currentAnimation.Frames.Count - 1;
            }
        }
    }
    
    public string CurrentSpriteName => currentAnimation?.Frames[currentFrame].Sprite;
}
```

## Advanced Features

### Trimming Support
When `trimSprites` is enabled, PPacker automatically removes transparent borders and stores the original dimensions and trim offsets. This information can be used to properly position sprites that have been trimmed.

### Rotation Support
Enable `allowRotation` to allow PPacker to rotate sprites 90 degrees for better packing efficiency. The rotation information is stored in the sprite data.

### Multiple Input Sources
Combine individual sprites and existing sprite sheets in a single atlas:

```json
{
  "inputs": [
    {
      "imagePath": "individual-sprites/player.png",
      "prefix": "player_"
    },
    {
      "imagePath": "spritesheets/ui.png",
      "dataPath": "spritesheets/ui.json",
      "prefix": "ui_"
    },
    {
      "imagePath": "spritesheets/effects.png",
      "dataPath": "spritesheets/effects.json"
    }
  ]
}
```

## Building from Source

### Prerequisites
- .NET 8.0 SDK or later

### Build Commands
```bash
# Restore dependencies
dotnet restore

# Build debug version
dotnet build

# Build release version
dotnet build -c Release

# Run tests
dotnet test

# Create NuGet package
dotnet pack -c Release
```

### Development Workflow
1. Make changes to source code
2. Run tests: `dotnet test`
3. Test with example: `dotnet run -- --config examples/ppacker-config.json`
4. Build release: `dotnet build -c Release`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Troubleshooting

### Common Issues

#### "Image file not found"
- Verify the `imagePath` is relative to the config file location
- Check file permissions and ensure the file exists
- Use `--verbose` flag to see the exact path being checked

#### "Animation frame not found in atlas"
- Frame names are **case-sensitive** - ensure exact matches
- Use `--verbose` to see generated frame names vs available sprites
- Check that your `namePattern` matches your actual sprite names

#### "No sprites found to pack"
- Verify all image files exist at the specified paths
- Check that data files (if specified) are valid JSON
- Use `--verbose` to trace which inputs are being processed

#### Missing sprites in final atlas
- Use `--verbose` to see detailed processing logs
- Check if data files are being parsed correctly
- Verify sprite names don't conflict (duplicates are skipped)

### Data File Formats

PPacker supports these JSON formats (auto-detected):

1. **Aseprite Format** (recommended for Aseprite users):
   ```json
   { "frames": [ { "filename": "sprite.aseprite", "frame": {...} } ] }
   ```

2. **PPacker AtlasData Format**:
   ```json
   { "width": 512, "sprites": [ { "name": "sprite", "x": 0, "y": 0, ... } ] }
   ```

3. **Direct SpriteData Array**:
   ```json
   [ { "name": "sprite", "x": 0, "y": 0, "width": 32, "height": 32 } ]
   ```

### Getting Help

- Use `ppacker --help` for command reference
- Use `ppacker --verbose` for detailed processing information
- Check the [Issues page](https://github.com/GHolfelder/PPacker/issues) for known problems
- Create example configurations with `ppacker example`

## Acknowledgments

- Uses [ImageSharp](https://github.com/SixLabors/ImageSharp) for image processing
- Uses [System.CommandLine](https://github.com/dotnet/command-line-api) for CLI interface
- Inspired by various texture packing tools in the game development community