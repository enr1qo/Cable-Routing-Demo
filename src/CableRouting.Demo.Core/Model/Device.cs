namespace CableRouting.Demo.Core.Model;

/// <summary>Устройство, подключённое к узлу трассы.</summary>
public sealed class Device
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required DeviceKind Kind { get; init; }

    /// <summary>Приложение/подсистема: СКС, видеонаблюдение и т. п.</summary>
    public required string Application { get; init; }

    /// <summary>Узел трассы, к которому подключено устройство.</summary>
    public required string TrayNodeId { get; init; }
}
