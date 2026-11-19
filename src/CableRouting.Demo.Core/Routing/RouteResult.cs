namespace CableRouting.Demo.Core.Routing;

/// <summary>Результат поиска пути по графу трасс.</summary>
public sealed class RouteResult
{
    private RouteResult(
        bool found,
        IReadOnlyList<string> nodeIds,
        IReadOnlyList<string> segmentIds,
        double lengthMm,
        string? error)
    {
        Found = found;
        NodeIds = nodeIds;
        SegmentIds = segmentIds;
        LengthMm = lengthMm;
        Error = error;
    }

    public bool Found { get; }

    public IReadOnlyList<string> NodeIds { get; }

    public IReadOnlyList<string> SegmentIds { get; }

    /// <summary>Геометрическая длина пути, мм.</summary>
    public double LengthMm { get; }

    public string? Error { get; }

    public static RouteResult Success(
        IReadOnlyList<string> nodeIds,
        IReadOnlyList<string> segmentIds,
        double lengthMm) =>
        new(true, nodeIds, segmentIds, lengthMm, null);

    public static RouteResult Unreachable(string error) =>
        new(false, Array.Empty<string>(), Array.Empty<string>(), 0, error);
}
