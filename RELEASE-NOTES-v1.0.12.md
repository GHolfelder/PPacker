# PPacker v1.0.12 Release Notes

**Release Date**: January 18, 2026

## 🚀 New Features

### Fixed Sprite Positioning

PPacker now supports **fixed positioning** for sprites, allowing you to place specific assets (like fonts, UI elements, or key textures) at exact coordinates in the atlas while other sprites pack dynamically around them.

#### Configuration

```json
{
  "inputs": [
    {
      "imagepath": "./fonts/Roboto_Condensed_0.png",
      "fixedPosition": {
        "x": 0,
        "y": 0
      },
      "priority": 100
    },
    {
      "imagepath": "./player/sprites.png", 
      "priority": 50
    }
  ]
}
```

#### Key Features

- **Fixed Position Control**: Use `fixedPosition.x` and `fixedPosition.y` to specify exact atlas coordinates
- **Priority-Based Packing**: Higher `priority` values are packed first for both fixed and dynamic sprites
- **Automatic Validation**: Built-in checks for position conflicts, bounds validation, and overlap detection
- **Rotation Compatibility**: Fixed sprites respect global `allowRotation` setting when beneficial
- **Two-Phase Packing**: Fixed sprites placed first, then dynamic sprites pack optimally around them

#### Use Cases

- **Font Atlases**: Position bitmap fonts at predictable locations (e.g., top-left corner)
- **UI Elements**: Ensure consistent positioning for critical interface components
- **Key Textures**: Guarantee specific sprites are at known coordinates for direct texture sampling
- **Legacy Compatibility**: Maintain existing atlas layouts when adding new sprites

## 🛠️ Technical Implementation

### New Configuration Properties

- **`fixedPosition`** (object, optional): Specifies exact x,y coordinates for sprite placement
  - `x` (number): X coordinate in pixels
  - `y` (number): Y coordinate in pixels
- **`priority`** (number, default: 0): Packing order priority (higher values packed first)

### Validation Features

- Fixed position bounds checking against atlas dimensions
- Overlap detection between multiple fixed sprites
- Error reporting for configuration conflicts
- TMX input compatibility (fixed positioning not supported for map inputs)

### Packing Algorithm

1. **Phase 1**: Place all fixed-position sprites at their specified coordinates
2. **Phase 2**: Pack remaining sprites dynamically using priority-based ordering
3. **Collision Detection**: Ensure dynamic sprites don't overlap with fixed sprites
4. **Space Optimization**: Dynamic packing continues to use efficient bin packing around fixed regions

## 📋 Breaking Changes

**None** - This release is fully backward compatible with existing configurations.

## 🐛 Bug Fixes

- Enhanced sprite-to-input mapping for better metadata preservation
- Improved validation error messages for configuration issues

## 🔧 Internal Improvements

- Extended `PackingRectangle` class with positioning metadata
- Added `FindInputForSprite()` helper method for better sprite tracking
- Enhanced `BinPacker` with two-phase packing algorithm
- Improved validation logic in configuration processing

## 📖 Documentation Updates

- Updated README with fixed positioning examples
- Added configuration validation documentation
- Enhanced troubleshooting guide for positioning conflicts

## 🎯 Usage Examples

### Font Positioning
```json
{
  "inputs": [
    {
      "imagepath": "./fonts/game_font.png",
      "fixedPosition": { "x": 0, "y": 0 },
      "priority": 100
    }
  ]
}
```

### Priority-Based Ordering
```json
{
  "inputs": [
    {
      "imagepath": "./critical_ui.png",
      "priority": 90
    },
    {
      "imagepath": "./background_textures.png", 
      "priority": 10
    }
  ]
}
```

### Mixed Fixed and Dynamic
```json
{
  "inputs": [
    {
      "imagepath": "./ui/health_bar.png",
      "fixedPosition": { "x": 0, "y": 0 }
    },
    {
      "imagepath": "./ui/mana_bar.png",
      "fixedPosition": { "x": 0, "y": 32 }
    },
    {
      "imagepath": "./player/animations.png",
      "dataPath": "./player/animations.json",
      "priority": 50
    }
  ]
}
```

## ⚠️ Important Notes

- Fixed positions are validated at configuration time - conflicts cause immediate errors
- Fixed sprites are never rotated unless explicitly beneficial and `allowRotation` is enabled
- TMX map inputs cannot use fixed positioning (maps are processed separately)
- Fixed position coordinates are in pixels and must be within atlas bounds

## 🔗 Migration Guide

Existing configurations work without changes. To add fixed positioning:

1. Add `fixedPosition` object to specific input entries
2. Optionally add `priority` values for packing order control
3. Run with `--verbose` to verify positioning behavior

## 📊 Performance Impact

- **Minimal**: Two-phase packing adds negligible overhead
- **Memory**: Small increase for positioning metadata
- **Validation**: Quick bounds and overlap checking at startup
- **Output**: Same efficient atlas generation with deterministic positioning

---

**Full Changelog**: [v1.0.11...v1.0.12](https://github.com/MonoGame/PPacker/compare/v1.0.11...v1.0.12)

**Download**: Available via NuGet or GitHub releases