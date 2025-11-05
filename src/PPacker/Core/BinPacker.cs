namespace PPacker.Core;

/// <summary>
/// Simple rectangle structure for collision detection
/// </summary>
public readonly struct PackingRect
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public PackingRect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

/// <summary>
/// Represents a rectangle for bin packing
/// </summary>
public class PackingRectangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Rotated { get; set; }
    
    /// <summary>
    /// Original dimensions before any transformations
    /// </summary>
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }

    public PackingRectangle(int width, int height, string name)
    {
        Width = width;
        Height = height;
        Name = name;
        OriginalWidth = width;
        OriginalHeight = height;
    }

    /// <summary>
    /// Area of the rectangle
    /// </summary>
    public int Area => Width * Height;

    /// <summary>
    /// Perimeter of the rectangle
    /// </summary>
    public int Perimeter => 2 * (Width + Height);
}

/// <summary>
/// Simple bin packing algorithm using Bottom-Left Fill approach
/// </summary>
public class BinPacker
{
    private readonly int _maxWidth;
    private readonly int _maxHeight;
    private readonly int _padding;
    private readonly bool _allowRotation;
    private readonly List<PackingRectangle> _packedRectangles = new();

    public BinPacker(int maxWidth, int maxHeight, int padding = 1, bool allowRotation = false)
    {
        _maxWidth = maxWidth;
        _maxHeight = maxHeight;
        _padding = padding;
        _allowRotation = allowRotation;
    }

    /// <summary>
    /// Pack rectangles into the bin
    /// </summary>
    /// <param name="rectangles">Rectangles to pack</param>
    /// <returns>Packed rectangles with positions, or null if packing failed</returns>
    public PackingResult? Pack(List<PackingRectangle> rectangles)
    {
        _packedRectangles.Clear();

        // Sort rectangles by area (largest first) for better packing efficiency
        var sortedRectangles = rectangles
            .OrderByDescending(r => r.Area)
            .ThenByDescending(r => Math.Max(r.Width, r.Height))
            .ToList();

        foreach (var rect in sortedRectangles)
        {
            if (!TryPackRectangle(rect))
            {
                // Packing failed
                return null;
            }
        }

        // Calculate the actual used area
        var actualWidth = _packedRectangles.Max(r => r.X + r.Width);
        var actualHeight = _packedRectangles.Max(r => r.Y + r.Height);

        return new PackingResult
        {
            PackedRectangles = new List<PackingRectangle>(_packedRectangles),
            ActualWidth = actualWidth,
            ActualHeight = actualHeight,
            Efficiency = CalculateEfficiency(actualWidth, actualHeight)
        };
    }

    private bool TryPackRectangle(PackingRectangle rect)
    {
        // Try to fit the rectangle without rotation first
        var position = FindBestPosition(rect.Width + _padding * 2, rect.Height + _padding * 2);
        if (position != null)
        {
            rect.X = position.Value.X + _padding;
            rect.Y = position.Value.Y + _padding;
            rect.Rotated = false;
            _packedRectangles.Add(rect);
            return true;
        }

        // If rotation is allowed, try rotated version
        if (_allowRotation && rect.Width != rect.Height)
        {
            position = FindBestPosition(rect.Height + _padding * 2, rect.Width + _padding * 2);
            if (position != null)
            {
                rect.X = position.Value.X + _padding;
                rect.Y = position.Value.Y + _padding;
                // Swap dimensions for rotation
                (rect.Width, rect.Height) = (rect.Height, rect.Width);
                rect.Rotated = true;
                _packedRectangles.Add(rect);
                return true;
            }
        }

        return false;
    }

    private (int X, int Y)? FindBestPosition(int width, int height)
    {
        // Check if it fits at all
        if (width > _maxWidth || height > _maxHeight)
            return null;

        var bestPosition = (X: 0, Y: 0);
        var bestWaste = int.MaxValue;
        var found = false;

        // Try all possible positions using a simple grid approach
        for (int y = 0; y <= _maxHeight - height; y++)
        {
            for (int x = 0; x <= _maxWidth - width; x++)
            {
                if (CanPlaceAt(x, y, width, height))
                {
                    var waste = CalculateWaste(x, y, width, height);
                    if (waste < bestWaste)
                    {
                        bestWaste = waste;
                        bestPosition = (x, y);
                        found = true;
                    }
                }
            }
        }

        return found ? bestPosition : null;
    }

    private bool CanPlaceAt(int x, int y, int width, int height)
    {
        var newRect = new PackingRect(x, y, width, height);

        foreach (var existing in _packedRectangles)
        {
            var existingRect = new PackingRect(
                existing.X - _padding, 
                existing.Y - _padding, 
                existing.Width + _padding * 2, 
                existing.Height + _padding * 2);

            if (RectanglesOverlap(newRect, existingRect))
                return false;
        }

        return true;
    }

    private static bool RectanglesOverlap(PackingRect rect1, PackingRect rect2)
    {
        return rect1.X < rect2.X + rect2.Width &&
               rect1.X + rect1.Width > rect2.X &&
               rect1.Y < rect2.Y + rect2.Height &&
               rect1.Y + rect1.Height > rect2.Y;
    }

    private int CalculateWaste(int x, int y, int width, int height)
    {
        // Simple waste calculation - prefer bottom-left positions
        return x + y;
    }

    private double CalculateEfficiency(int usedWidth, int usedHeight)
    {
        var usedArea = _packedRectangles.Sum(r => r.Width * r.Height);
        var totalArea = usedWidth * usedHeight;
        return totalArea > 0 ? (double)usedArea / totalArea : 0.0;
    }
}

/// <summary>
/// Result of the packing operation
/// </summary>
public class PackingResult
{
    public List<PackingRectangle> PackedRectangles { get; set; } = new();
    public int ActualWidth { get; set; }
    public int ActualHeight { get; set; }
    public double Efficiency { get; set; }
}