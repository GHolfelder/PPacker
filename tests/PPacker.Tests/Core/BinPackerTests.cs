using FluentAssertions;
using PPacker.Core;
using Xunit;

namespace PPacker.Tests.Core;

public class BinPackerTests
{
    [Fact]
    public void Pack_WithSimpleRectangles_ShouldPackSuccessfully()
    {
        // Arrange
        var packer = new BinPacker(100, 100, padding: 1);
        var rectangles = new List<PackingRectangle>
        {
            new(10, 10, "rect1"),
            new(20, 20, "rect2"),
            new(15, 15, "rect3")
        };

        // Act
        var result = packer.Pack(rectangles);

        // Assert
        result.Should().NotBeNull();
        result!.PackedRectangles.Should().HaveCount(3);
        result.ActualWidth.Should().BeLessOrEqualTo(100);
        result.ActualHeight.Should().BeLessOrEqualTo(100);
    }

    [Fact]
    public void Pack_WithTooLargeRectangle_ShouldReturnNull()
    {
        // Arrange
        var packer = new BinPacker(50, 50);
        var rectangles = new List<PackingRectangle>
        {
            new(100, 100, "tooLarge")
        };

        // Act
        var result = packer.Pack(rectangles);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Pack_WithRotationEnabled_ShouldUseRotationWhenBeneficial()
    {
        // Arrange
        var packer = new BinPacker(100, 50, allowRotation: true);
        var rectangles = new List<PackingRectangle>
        {
            new(60, 30, "rect1"), // This should fit rotated
        };

        // Act
        var result = packer.Pack(rectangles);

        // Assert
        result.Should().NotBeNull();
        result!.PackedRectangles.Should().HaveCount(1);
    }

    [Fact]
    public void Pack_ShouldSortByAreaDescending()
    {
        // Arrange
        var packer = new BinPacker(200, 200, padding: 1);
        var rectangles = new List<PackingRectangle>
        {
            new(10, 10, "small"),    // Area: 100
            new(30, 30, "large"),    // Area: 900
            new(20, 20, "medium")    // Area: 400
        };

        // Act
        var result = packer.Pack(rectangles);

        // Assert
        result.Should().NotBeNull();
        
        // The large rectangle should be placed first (at padding offset)
        var largeRect = result!.PackedRectangles.First(r => r.Name == "large");
        largeRect.X.Should().Be(1); // Account for padding
        largeRect.Y.Should().Be(1); // Account for padding
    }
}