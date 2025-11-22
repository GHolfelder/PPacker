# PPacker v1.0.7 Release Notes

## 🚀 Major Features

### Comprehensive Tiled Map Support
PPacker now provides full support for Tiled map editor files, enabling seamless integration of TMX/TSX files into MonoGame projects.

## ✨ New Features

### Tiled Map Integration
- **TMX File Processing**: Direct support for Tiled map files (`.tmx`) as input sources
- **TSX Tileset Support**: Full external tileset (`.tsx`) loading and processing
- **Multi-Layer Support**: Handle tile layers, object layers, and image layers
- **Data Format Support**: CSV, Base64, GZIP, and ZLIB compression formats
- **MonoGame-Ready Output**: Generates optimized JSON format for MonoGame tile rendering

### Atlas Integration
- **Automatic Tileset Packing**: Tileset images are automatically included in atlas generation
- **Sprite Mapping**: Maps tileset images to atlas sprites for efficient rendering
- **Mixed Content Support**: Combine regular sprites and tiled maps in a single atlas

### Enhanced Configuration
- **TMX Input Support**: New `tmxPath` configuration option for direct TMX file input
- **Map Output**: New `mapPath` configuration option for MonoGame map data export
- **Flexible Workflows**: Support both sprite-only and map-integrated workflows

## 🔧 Technical Improvements

### New Models and Processors
- **TiledModels.cs**: Complete XML serialization models for TMX/TSX formats
- **MapData.cs**: MonoGame-optimized output format with JSON serialization
- **TiledMapProcessor.cs**: Comprehensive TMX/TSX parsing and conversion engine

### Enhanced Core Components
- **AtlasPacker.cs**: Integrated TMX processing with existing sprite pipeline
- **Program.cs**: Extended validation and workflow support for TMX files
- **PackerConfig.cs**: New configuration options for Tiled map integration

## 🐛 Bug Fixes

### v1.0.7.1 (Latest on feature/tile-tweaks branch)
- **Fixed TMX Input Validation**: Resolved "Image file not found" error when processing TMX files
- **TSX Subfolder Support**: Fixed relative path resolution for tilesets located in subfolders
- **Improved Error Handling**: Better validation logic for mixed input types

## 📁 Examples and Documentation

### New Examples
- **examples/maps/**: Complete Tiled map integration examples
  - Sample TMX file with desert tileset
  - Subfolder test case demonstrating TSX path resolution
  - Configuration examples for various scenarios
- **examples/tiled-map/**: Alternative example setup
- **Comprehensive Test Cases**: Full test coverage for Tiled map functionality

## 🔗 Supported Formats

### Input Formats
- PNG sprite images (existing)
- Aseprite JSON data (existing) 
- **TMX map files (NEW)**
- **TSX tileset files (NEW)**

### Output Formats
- PNG atlas images (existing)
- JSON atlas data (existing)
- JSON animation data (existing)
- **JSON map data for MonoGame (NEW)**

## 🛠 Usage Examples

### Basic Tiled Map Processing
```json
{
  "inputs": [
    {
      "tmxPath": "sample-map.tmx",
      "prefix": "desert_"
    }
  ],
  "output": {
    "imagePath": "output/atlas.png",
    "dataPath": "output/atlas.json",
    "mapPath": "output/map.json"
  }
}
```

### Mixed Content (Sprites + Maps)
```json
{
  "inputs": [
    {
      "imagePath": "sprites/player.png",
      "prefix": "player_"
    },
    {
      "tmxPath": "maps/level1.tmx",
      "prefix": "level1_"
    }
  ],
  "output": {
    "imagePath": "output/game-atlas.png",
    "dataPath": "output/game-atlas.json",
    "mapPath": "output/levels.json"
  }
}
```

## 📊 Compatibility

- **.NET 8.0**: Continues to target .NET 8.0
- **MonoGame Ready**: Output formats optimized for MonoGame consumption
- **Tiled Compatibility**: Supports Tiled map editor v1.10+ format specification
- **Cross-Platform**: Windows, macOS, and Linux support maintained

## 🔄 Migration from v1.0.6

Existing configurations and workflows remain fully compatible. New Tiled map features are opt-in through new configuration options.

### Breaking Changes
- None - fully backward compatible

### Recommended Updates
- Add `mapPath` to output configuration when using TMX files
- Use `tmxPath` input option for direct Tiled map processing

## 🧪 Testing

- **8 New Test Cases**: Comprehensive coverage for Tiled map functionality
- **Example Validation**: All examples tested and verified working
- **Edge Case Coverage**: Subfolder scenarios, mixed content, various data formats

## 📈 Performance

- **Efficient Processing**: TMX files processed in single pass with tileset caching
- **Memory Optimized**: Proper resource disposal for large map files  
- **Atlas Integration**: No performance impact on existing sprite packing workflows

## 🏗 For Developers

### Key Classes Added
- `TiledMap`, `TiledTileset`, `TiledLayer` - XML parsing models
- `MapData`, `MapTileset`, `MapTileLayer` - MonoGame output models  
- `TiledMapProcessor` - Core processing engine

### Integration Points
- Existing `AtlasPacker` workflow extended seamlessly
- New validation logic in `Program.cs`
- Enhanced configuration in `PackerConfig`

## 📝 Full Changelog

### v1.0.7.1 (feature/tile-tweaks)
- Fix TMX input validation and TSX subfolder path resolution
- Add comprehensive subfolder test case

### v1.0.7 
- Add comprehensive Tiled map support with TMX/TSX parsing
- Integrate atlas generation with Tiled map processing  
- Add MonoGame-optimized map data output format
- Add extensive examples and documentation
- Add full test coverage for new functionality

---

## Download

- **Standalone Windows Executable**: Self-contained 69.4MB executable (no .NET installation required)
- **Source Code**: Available on GitHub with full documentation
- **NuGet Package**: .NET 8.0 compatible package available

## Support

For issues, feature requests, or contributions, please visit the GitHub repository.

---

**Full Diff**: v1.0.6...v1.0.7  
**Total Changes**: 25 files changed, 2000+ lines added