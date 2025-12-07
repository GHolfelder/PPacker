using PPacker.Core;
using Xunit;

namespace PPacker.Tests.Core
{
    public class BinPackerBoundaryTests
    {
        [Fact]
        public void BinPacker_ShouldFitExactNumberOfSprites_InSingleRow()
        {
            // Arrange: 512x512 atlas with 32x32 sprites, no padding
            var packer = new BinPacker(512, 512, 0, false);
            var rectangles = new List<PackingRectangle>();
            
            // 512 / 32 = 16 sprites should fit exactly in one row
            for (int i = 0; i < 16; i++)
            {
                rectangles.Add(new PackingRectangle(32, 32, $"sprite_{i}"));
            }
            
            // Act
            var result = packer.Pack(rectangles);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(16, result.PackedRectangles.Count);
            
            // Check that all sprites are in the first row and don't exceed boundaries
            var firstRow = result.PackedRectangles.Where(r => r.Y == 0).OrderBy(r => r.X).ToList();
            Assert.Equal(16, firstRow.Count);
            
            // Verify each sprite's position and that none exceed the boundary
            for (int i = 0; i < firstRow.Count; i++)
            {
                var sprite = firstRow[i];
                Assert.Equal(i * 32, sprite.X); // Should be at exact positions 0, 32, 64, ..., 480
                Assert.Equal(0, sprite.Y);
                Assert.Equal(32, sprite.Width);
                Assert.Equal(32, sprite.Height);
                
                // Critical check: sprite should not extend beyond atlas boundary
                Assert.True(sprite.X + sprite.Width <= 512, 
                    $"Sprite {sprite.Name} at x={sprite.X} with width={sprite.Width} extends beyond atlas width of 512");
            }
        }
        
        [Fact]
        public void BinPacker_ShouldReject17thSprite_WhenRowIsFull()
        {
            // Arrange: Try to pack 17 sprites in a 512-wide atlas
            var packer = new BinPacker(512, 32, 0, false); // Only 32 pixels tall to force single row
            var rectangles = new List<PackingRectangle>();
            
            for (int i = 0; i < 17; i++)
            {
                rectangles.Add(new PackingRectangle(32, 32, $"sprite_{i}"));
            }
            
            // Act
            var result = packer.Pack(rectangles);
            
            // Assert: Should fail because 17 * 32 = 544 > 512
            Assert.Null(result);
        }
        
        [Fact]
        public void BinPacker_ShouldPackSprites_InMultipleRows()
        {
            // Arrange: Pack 32 sprites in a 512x64 atlas (should fit in 2 rows)
            var packer = new BinPacker(512, 64, 0, false);
            var rectangles = new List<PackingRectangle>();
            
            for (int i = 0; i < 32; i++)
            {
                rectangles.Add(new PackingRectangle(32, 32, $"sprite_{i}"));
            }
            
            // Act
            var result = packer.Pack(rectangles);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.PackedRectangles.Count);
            
            // Check first row (y=0)
            var firstRow = result.PackedRectangles.Where(r => r.Y == 0).OrderBy(r => r.X).ToList();
            Assert.Equal(16, firstRow.Count);
            
            // Check second row (y=32)
            var secondRow = result.PackedRectangles.Where(r => r.Y == 32).OrderBy(r => r.X).ToList();
            Assert.Equal(16, secondRow.Count);
            
            // Verify positioning
            for (int i = 0; i < 16; i++)
            {
                Assert.Equal(i * 32, firstRow[i].X);
                Assert.Equal(0, firstRow[i].Y);
                Assert.Equal(i * 32, secondRow[i].X);
                Assert.Equal(32, secondRow[i].Y);
            }
        }
    }
}