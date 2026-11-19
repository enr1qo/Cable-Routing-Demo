namespace CableRouting.Demo.Core.Model;

/// <summary>Точка в плоскости чертежа. Координаты — в миллиметрах, как в DWG.</summary>
public readonly record struct Point2D(double X, double Y)
{
    /// <summary>Расстояние до другой точки, мм.</summary>
    public double DistanceTo(Point2D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
