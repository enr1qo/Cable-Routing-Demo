using CableRouting.Demo.Core.Model;

namespace CableRouting.Demo.Tests;

internal static class TestProject
{
    public static TrayNode MakeNode(string id, double xMeters, double yMeters) => new()
    {
        Id = id,
        Position = new Point2D(xMeters * 1000.0, yMeters * 1000.0)
    };

    public static TraySegment MakeTray(
        string id,
        string fromNodeId,
        string toNodeId,
        double usableAreaMm2 = 1000.0,
        string type = "LADDER") => new()
    {
        Id = id,
        FromNodeId = fromNodeId,
        ToNodeId = toNodeId,
        Type = type,
        UsableAreaMm2 = usableAreaMm2
    };

    public static Device MakeDevice(
        string id,
        string trayNodeId,
        DeviceKind kind = DeviceKind.Other,
        string application = "СКС") => new()
    {
        Id = id,
        Name = id,
        Kind = kind,
        Application = application,
        TrayNodeId = trayNodeId
    };

    public static Cable MakeCable(
        string id,
        string fromDeviceId,
        string toDeviceId,
        double diameterMm = 6.0,
        string application = "СКС") => new()
    {
        Id = id,
        Application = application,
        DiameterMm = diameterMm,
        FromDeviceId = fromDeviceId,
        ToDeviceId = toDeviceId
    };

    public static Project MakeProject(
        IReadOnlyList<TrayNode> nodes,
        IReadOnlyList<TraySegment> trays,
        IReadOnlyList<Device> devices,
        IReadOnlyList<Cable> cables) => new()
    {
        Nodes = nodes,
        Trays = trays,
        Devices = devices,
        Cables = cables
    };
}
