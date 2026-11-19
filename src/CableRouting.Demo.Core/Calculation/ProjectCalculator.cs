using CableRouting.Demo.Core.Model;
using CableRouting.Demo.Core.Routing;

namespace CableRouting.Demo.Core.Calculation;

/// <summary>Считает маршруты, метраж кабеля и заполнение лотков по проекту.</summary>
public static class ProjectCalculator
{
    public static ProjectReport Calculate(Project project, CalculationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(project);
        options ??= new CalculationOptions();
        options.Validate();

        var devices = new Dictionary<string, Device>(StringComparer.Ordinal);
        foreach (var device in project.Devices)
        {
            if (!devices.TryAdd(device.Id, device))
                throw new InvalidOperationException($"Устройство с Id '{device.Id}' объявлено дважды.");
        }

        var router = new CableRouter(project.Nodes, project.Trays);
        foreach (var device in project.Devices)
        {
            if (!router.ContainsNode(device.TrayNodeId))
                throw new InvalidOperationException(
                    $"Устройство '{device.Id}' ссылается на неизвестный узел '{device.TrayNodeId}'.");
        }

        var cableIds = new HashSet<string>(StringComparer.Ordinal);
        var routes = new List<CableRoute>(project.Cables.Count);
        foreach (var cable in project.Cables)
        {
            if (!cableIds.Add(cable.Id))
                throw new InvalidOperationException($"Кабель с Id '{cable.Id}' объявлен дважды.");
            if (cable.DiameterMm <= 0)
                throw new InvalidOperationException($"У кабеля '{cable.Id}' должен быть положительный диаметр.");
            if (!devices.TryGetValue(cable.FromDeviceId, out var from))
                throw new InvalidOperationException(
                    $"Кабель '{cable.Id}' ссылается на неизвестное устройство '{cable.FromDeviceId}'.");
            if (!devices.TryGetValue(cable.ToDeviceId, out var to))
                throw new InvalidOperationException(
                    $"Кабель '{cable.Id}' ссылается на неизвестное устройство '{cable.ToDeviceId}'.");

            var route = router.Route(from.TrayNodeId, to.TrayNodeId);
            double lengthM = route.Found
                ? route.LengthMm / 1000.0 * (1.0 + options.SlackPercent / 100.0)
                  + options.SlackMetersPerEnd * 2.0
                : 0.0;

            routes.Add(new CableRoute
            {
                Cable = cable,
                From = from,
                To = to,
                Route = route,
                LengthM = lengthM
            });
        }

        return new ProjectReport
        {
            Routes = routes,
            CableStock = BuildStock(routes),
            TrayFill = BuildTrayFill(project, router, routes, options)
        };
    }

    private static CableStock BuildStock(IReadOnlyList<CableRoute> routes)
    {
        var byApplication = new Dictionary<string, List<CableRoute>>(StringComparer.OrdinalIgnoreCase);
        foreach (var route in routes)
        {
            if (!route.Found)
                continue;

            if (!byApplication.TryGetValue(route.Cable.Application, out var list))
            {
                list = new List<CableRoute>();
                byApplication[route.Cable.Application] = list;
            }
            list.Add(route);
        }

        var rows = byApplication
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new CableStockRow
            {
                Application = pair.Key,
                CableCount = pair.Value.Count,
                TotalLengthM = Round(pair.Value.Sum(route => route.LengthM)),
                MaxLengthM = Round(pair.Value.Max(route => route.LengthM))
            })
            .ToList();

        var routed = routes.Where(route => route.Found).ToList();
        return new CableStock
        {
            Rows = rows,
            CableCount = routed.Count,
            TotalLengthM = Round(routed.Sum(route => route.LengthM)),
            UnroutedCables = routes.Count - routed.Count
        };
    }

    private static TrayFill BuildTrayFill(
        Project project,
        CableRouter router,
        IReadOnlyList<CableRoute> routes,
        CalculationOptions options)
    {
        var usedArea = new Dictionary<string, double>(StringComparer.Ordinal);
        var cableCount = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var route in routes)
        {
            if (!route.Found)
                continue;

            double areaMm2 = Math.PI * Math.Pow(route.Cable.DiameterMm / 2.0, 2.0);
            foreach (var segmentId in route.Route.SegmentIds)
            {
                usedArea[segmentId] = usedArea.GetValueOrDefault(segmentId) + areaMm2;
                cableCount[segmentId] = cableCount.GetValueOrDefault(segmentId) + 1;
            }
        }

        var rows = project.Trays
            .OrderBy(tray => tray.Id, StringComparer.Ordinal)
            .Select(tray =>
            {
                double used = usedArea.GetValueOrDefault(tray.Id);
                double fillPercent = used / tray.UsableAreaMm2 * 100.0;
                return new TrayFillRow
                {
                    TrayId = tray.Id,
                    Type = tray.Type,
                    LengthM = Round(router.GetSegmentLengthMm(tray.Id) / 1000.0),
                    CableCount = cableCount.GetValueOrDefault(tray.Id),
                    UsedAreaMm2 = Round(used),
                    FillPercent = Round(fillPercent),
                    Overloaded = fillPercent > options.TrayFillLimitPercent
                };
            })
            .ToList();

        return new TrayFill
        {
            Rows = rows,
            OverloadedCount = rows.Count(row => row.Overloaded),
            MaxFillPercent = rows.Count == 0 ? 0.0 : rows.Max(row => row.FillPercent)
        };
    }

    private static double Round(double value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
