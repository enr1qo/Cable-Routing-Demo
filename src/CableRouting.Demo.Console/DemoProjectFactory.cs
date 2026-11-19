using CableRouting.Demo.Core.Model;

namespace CableRouting.Demo.Sample;

/// <summary>
/// Синтетический проект для демо: два шкафа, розетки, камеры и лотки.
/// Данные вымышленные, координаты — в метрах, внутри модели переводятся в миллиметры.
/// </summary>
public static class DemoProjectFactory
{
    public static Project Create()
    {
        var nodes = new List<TrayNode>
        {
            Node("N-CAB-A", 0, 0),
            Node("N-BB-1", 6, 0),
            Node("N-BB-2", 12, 0),
            Node("N-BB-3", 18, 0),
            Node("N-CAB-B", 24, 0),
            Node("N-FL-1", 6, 6),
            Node("N-FL-2", 12, 6),
            Node("N-FL-3", 18, 6),
            Node("N-FAR", 12, 12),
            Node("N-ISOLATED", 40, 40)
        };

        var trays = new List<TraySegment>
        {
            Tray("T01", "N-CAB-A", "N-BB-1", "LADDER-100", 4000),
            Tray("T02", "N-BB-1", "N-BB-2", "LADDER-100", 4000),
            Tray("T03", "N-BB-2", "N-BB-3", "LADDER-100", 4000),
            Tray("T04", "N-BB-3", "N-CAB-B", "LADDER-100", 4000),
            Tray("T05", "N-BB-1", "N-FL-1", "LADDER-50", 1500),
            Tray("T06", "N-BB-2", "N-FL-2", "LADDER-50", 1500),
            Tray("T07", "N-BB-3", "N-FL-3", "LADDER-50", 1500),
            Tray("T08", "N-FL-1", "N-FAR", "CORRUGATED-20", 250),
            Tray("T09", "N-FL-2", "N-FAR", "CORRUGATED-20", 250),
            Tray("T10", "N-FL-3", "N-FAR", "CORRUGATED-20", 250)
        };

        var devices = new List<Device>
        {
            Device("CAB-A", "CAB-A", DeviceKind.Cabinet, "Общее", "N-CAB-A"),
            Device("CAB-B", "CAB-B", DeviceKind.Cabinet, "Общее", "N-CAB-B"),
            Device("S01", "S01", DeviceKind.Socket, "СКС", "N-FL-1"),
            Device("S02", "S02", DeviceKind.Socket, "СКС", "N-FL-1"),
            Device("S03", "S03", DeviceKind.Socket, "СКС", "N-FL-2"),
            Device("S04", "S04", DeviceKind.Socket, "СКС", "N-FL-2"),
            Device("S05", "S05", DeviceKind.Socket, "СКС", "N-FL-3"),
            Device("S06", "S06", DeviceKind.Socket, "СКС", "N-FAR"),
            Device("C01", "C01", DeviceKind.Camera, "Видеонаблюдение", "N-FL-3"),
            Device("C02", "C02", DeviceKind.Camera, "Видеонаблюдение", "N-FAR"),
            Device("C03", "C03", DeviceKind.Camera, "Видеонаблюдение", "N-FL-1"),
            Device("S99", "S99", DeviceKind.Socket, "СКС", "N-ISOLATED")
        };

        var cables = new List<Cable>
        {
            Cable("K-SCS-01", "СКС", 6, "S01", "CAB-A"),
            Cable("K-SCS-02", "СКС", 6, "S02", "CAB-A"),
            Cable("K-SCS-03", "СКС", 6, "S03", "CAB-B"),
            Cable("K-SCS-04", "СКС", 6, "S04", "CAB-B"),
            Cable("K-SCS-05", "СКС", 6, "S05", "CAB-B"),
            Cable("K-SCS-06", "СКС", 6, "S06", "CAB-A"),
            Cable("K-SCS-07", "СКС", 6, "S06", "CAB-A"),
            Cable("K-SCS-08", "СКС", 6, "S06", "CAB-A"),
            Cable("K-SCS-09", "СКС", 6, "S06", "CAB-B"),
            Cable("K-SCS-10", "СКС", 6, "S06", "CAB-B"),
            Cable("K-CCTV-01", "Видеонаблюдение", 8, "C01", "CAB-B"),
            Cable("K-CCTV-02", "Видеонаблюдение", 8, "C02", "CAB-A"),
            Cable("K-CCTV-03", "Видеонаблюдение", 8, "C03", "CAB-A"),
            Cable("K-CCTV-04", "Видеонаблюдение", 8, "C02", "CAB-B"),
            // Специально оставлен без пути: устройство на изолированном узле.
            Cable("K-SCS-99", "СКС", 6, "S99", "CAB-A")
        };

        return new Project
        {
            Nodes = nodes,
            Trays = trays,
            Devices = devices,
            Cables = cables
        };
    }

    private static TrayNode Node(string id, double xMeters, double yMeters) => new()
    {
        Id = id,
        Position = new Point2D(xMeters * 1000.0, yMeters * 1000.0)
    };

    private static TraySegment Tray(string id, string from, string to, string type, double areaMm2) => new()
    {
        Id = id,
        FromNodeId = from,
        ToNodeId = to,
        Type = type,
        UsableAreaMm2 = areaMm2
    };

    private static Device Device(string id, string name, DeviceKind kind, string application, string nodeId) => new()
    {
        Id = id,
        Name = name,
        Kind = kind,
        Application = application,
        TrayNodeId = nodeId
    };

    private static Cable Cable(string id, string application, double diameterMm, string from, string to) => new()
    {
        Id = id,
        Application = application,
        DiameterMm = diameterMm,
        FromDeviceId = from,
        ToDeviceId = to
    };
}
