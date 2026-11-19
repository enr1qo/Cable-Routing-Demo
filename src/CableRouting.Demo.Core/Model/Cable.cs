namespace CableRouting.Demo.Core.Model;

/// <summary>Кабель между двумя устройствами.</summary>
public sealed class Cable
{
    public required string Id { get; init; }

    public required string Application { get; init; }

    public required double DiameterMm { get; init; }

    public required string FromDeviceId { get; init; }

    public required string ToDeviceId { get; init; }
}
