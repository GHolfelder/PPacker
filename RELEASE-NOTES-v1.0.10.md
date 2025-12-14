# Release Notes - v1.0.10

## New Features

### Enhanced Tilemap Animation Support

- **Improved Animation Data**: Animation frames now include precise texture coordinates (`sourceX`, `sourceY`, `sourceWidth`, `sourceHeight`) for direct MonoGame integration
- **Automatic Coordinate Calculation**: PPacker now calculates exact texture coordinates for each animation frame based on tileset layout (margin and spacing)
- **Better MonoGame Integration**: No need to manually calculate tile positions - animation data includes ready-to-use rectangles for `SpriteBatch.Draw()`
- **Comprehensive Testing**: Added thorough test coverage for animation parsing, conversion, and coordinate calculation

### Technical Improvements

- Enhanced `MapAnimationFrame` model with texture coordinate properties
- Improved `ConvertTile` method to include tileset layout information
- Added proper margin and spacing support for animation frame coordinates
- Updated TiledMapProcessor to copy all tileset properties for accurate calculations

### Documentation

- **New Documentation**: Added comprehensive [TILEMAP-ANIMATIONS.md](TILEMAP-ANIMATIONS.md) guide
- **MonoGame Examples**: Includes complete code samples for loading and using animation data
- **Performance Guidelines**: Best practices for animation timing and rendering optimization
- **Updated README**: Enhanced feature descriptions and integration examples

## Breaking Changes

None. This release is fully backward compatible with existing functionality.

## Migration

For users upgrading from previous versions:

1. **Existing Projects**: Continue to work without changes
2. **Animation Data**: Old `tileId` and `duration` properties are preserved
3. **New Features**: Take advantage of new `sourceX`, `sourceY`, `sourceWidth`, `sourceHeight` properties for enhanced MonoGame integration
4. **Documentation**: Refer to [TILEMAP-ANIMATIONS.md](TILEMAP-ANIMATIONS.md) for implementing animation systems

## Examples

### Before (v1.0.9 and earlier)
```json
{
  "animation": [
    {
      "tileId": 21,
      "duration": 100
    }
  ]
}
```

### After (v1.0.10)
```json
{
  "animation": [
    {
      "tileId": 21,
      "duration": 100,
      "sourceX": 166,
      "sourceY": 67,
      "sourceWidth": 32,
      "sourceHeight": 32
    }
  ]
}
```

## Files Changed

- `src/PPacker/Models/MapData.cs` - Enhanced MapAnimationFrame with texture coordinates
- `src/PPacker/Models/TiledModels.cs` - Added margin/spacing properties to TiledTilesetRef
- `src/PPacker/Core/TiledMapProcessor.cs` - Improved animation frame conversion with coordinate calculation
- `tests/PPacker.Tests/Core/TiledMapProcessorTests.cs` - Added comprehensive animation tests
- `README.md` - Updated feature descriptions
- `TILEMAP-ANIMATIONS.md` - New comprehensive documentation

## Contributors

This release includes contributions and testing to ensure robust tilemap animation support for MonoGame developers.