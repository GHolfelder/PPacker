# PPacker v1.0.11 Release Notes

## 🆕 New Features

### Prefix-based TMX Map Naming
- **Intelligent Map Naming**: Maps are now automatically named based on input prefixes
  - Maps with prefix `desert_` are named `desertmap`
  - Maps with prefix `oasis_` are named `oasismap`  
  - Maps without prefixes are named `map` for consistency
- **Consistent Array Output**: All map data is now output as an array structure, even for single maps
- **Conflict Prevention**: Automatic validation prevents duplicate map names during configuration validation
- **Clear Error Messages**: Helpful validation messages guide users when naming conflicts occur

### Benefits for MonoGame Integration
- **Better Organization**: Multiple maps from different TMX files are clearly separated by meaningful names
- **Predictable Loading**: MonoGame code can reliably reference maps by their logical names
- **Conflict-free**: No more naming collisions when processing multiple TMX files
- **Backward Compatible**: Existing configurations continue to work with the new naming scheme

## 🔧 Technical Improvements

### Enhanced TMX Processing
- Maps are now tracked with their input configurations throughout the processing pipeline
- Prefix information is preserved and used for final map naming
- Early validation catches naming conflicts before processing begins

### Code Quality
- Updated `TiledMapProcessor.ConvertToMapData()` method signature for better clarity
- Enhanced error handling and validation in the atlas packing workflow
- Comprehensive test coverage for new naming functionality

## 📋 Example Usage

### Multiple TMX Maps Configuration
```json
{
  "inputs": [
    {
      "tmxPath": "desert-level.tmx",
      "prefix": "desert_"
    },
    {
      "tmxPath": "forest-level.tmx", 
      "prefix": "forest_"
    }
  ],
  "output": {
    "imagePath": "output/atlas.png",
    "dataPath": "output/atlas.json",
    "mapPath": "output/maps.json"
  }
}
```

### Generated Map Data
```json
[
  {
    "name": "desertmap",
    "width": 30,
    "height": 20,
    "tilesets": [...],
    "tileLayers": [...]
  },
  {
    "name": "forestmap",
    "width": 25, 
    "height": 15,
    "tilesets": [...],
    "tileLayers": [...]
  }
]
```

## 🐛 Bug Fixes

### TMX/TSX Property Transfer Fix
- **Fixed Missing Tileset Properties**: Margin and spacing attributes from TSX files are now properly preserved in JSON output
  - Tileset `margin` attribute correctly transferred from TSX to output JSON
  - Tileset `spacing` attribute correctly transferred from TSX to output JSON
  - Previously these values were always showing as 0 regardless of TSX file settings
- **Complete Property Coverage**: All TMX/TSX properties are now comprehensively preserved:
  - ✅ Map-level properties (root element custom properties)
  - ✅ Tile layer properties (layer-specific metadata)
  - ✅ Object layer properties (object group metadata)
  - ✅ Individual object properties (with proper type conversion)
  - ✅ Individual tile properties (static and animated tiles)
  - ✅ Tileset properties (including margin/spacing attributes)
- **Enhanced Test Coverage**: Added comprehensive tests to prevent regression of property transfer functionality

### Impact
This fix ensures that all custom metadata and layout information from Tiled map editors is accurately preserved when converting to MonoGame format, maintaining the complete fidelity of map data.

- Fixed compilation issues in unit tests after API changes
- Improved error messaging for TMX processing failures

## 📦 Distribution
- Windows standalone executable: `PPacker.exe` (self-contained, no .NET required)
- Cross-platform .NET tool available via NuGet

---

**Full Changelog**: [v1.0.10...v1.0.11](https://github.com/MonoGame/PPacker/compare/v1.0.10...v1.0.11)