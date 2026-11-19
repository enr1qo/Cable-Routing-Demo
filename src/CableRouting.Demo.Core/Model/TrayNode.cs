namespace CableRouting.Demo.Core.Model;

/// <summary>Узел трассы: точка, в которой сходятся участки лотка.</summary>
public sealed class TrayNode
{
    public required string Id { get; init; }

    public required Point2D Position { get; init; }
}
