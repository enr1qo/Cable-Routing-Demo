namespace CableRouting.Demo.Core.Model;

/// <summary>Участок трассы между двумя узлами.</summary>
public sealed class TraySegment
{
    public required string Id { get; init; }

    public required string FromNodeId { get; init; }

    public required string ToNodeId { get; init; }

    public required string Type { get; init; }

    /// <summary>Полезная площадь сечения, мм². По ней считается заполнение лотка.</summary>
    public required double UsableAreaMm2 { get; init; }
}
