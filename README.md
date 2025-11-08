# PPacker

A powerful command-line tool for packing PNG files into texture atlases for MonoGame projects, with comprehensive sprite animation support.

## Features

- **Texture Atlas Packing**: Efficiently pack multiple PNG files into a single texture atlas
- **Sprite Data Merging**: Merge JSON sprite definitions and automatically update coordinates
- **Animation Support**: Create animation definitions from sprite sequences
- **Flexible Input**: Support for individual sprites or existing sprite sheets with metadata
- **Smart Packing**: Bin packing algorithm with optional rotation for optimal space usage
- **Sprite Trimming**: Automatically remove transparent borders to save space
- **Power-of-2 Support**: Generate power-of-2 atlas dimensions for optimal GPU performance
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

### Verbose Output
```bash
ppacker --config config.json --verbose
```

### Generate Examples
```bash
ppacker example --output ./examples
```

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
    "version": "1.0.0",
    "generated": "2025-11-04T10:00:00Z",
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
        { "sprite": "player_walk_01", "duration": null },
        { "sprite": "player_walk_02", "duration": null }
      ],
      "defaultDuration": 100,
      "loop": true,
      "totalDuration": 800
    }
  ]
}
```

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

## Acknowledgments

- Uses [ImageSharp](https://github.com/SixLabors/ImageSharp) for image processing
- Uses [System.CommandLine](https://github.com/dotnet/command-line-api) for CLI interface
- Inspired by various texture packing tools in the game development community