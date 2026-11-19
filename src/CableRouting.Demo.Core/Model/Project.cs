namespace CableRouting.Demo.Core.Model;

/// <summary>Проект: узлы и участки трассы, устройства и кабели.</summary>
public sealed class Project
{
    public required IReadOnlyList<TrayNode> Nodes { get; init; }

    public required IReadOnlyList<TraySegment> Trays { get; init; }

    public required IReadOnlyList<Device> Devices { get; init; }

    public required IReadOnlyList<Cable> Cables { get; init; }
}
