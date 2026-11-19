using CableRouting.Demo.Core.Model;
using CableRouting.Demo.Core.Routing;

namespace CableRouting.Demo.Core.Calculation;

/// <summary>Проложенный кабель: маршрут, устройства и длина с запасом.</summary>
public sealed class CableRoute
{
    public required Cable Cable { get; init; }

    public required Device From { get; init; }

    public required Device To { get; init; }

    public required RouteResult Route { get; init; }

    /// <summary>Длина с запасом, м.</summary>
    public required double LengthM { get; init; }

    public bool Found => Route.Found;
}
