namespace CableRouting.Demo.Core.Calculation;

/// <summary>Строка метража кабеля по приложению.</summary>
public sealed class CableStockRow
{
    public required string Application { get; init; }

    public required int CableCount { get; init; }

    public required double TotalLengthM { get; init; }

    public required double MaxLengthM { get; init; }
}

/// <summary>Сводка метража кабеля по проекту.</summary>
public sealed class CableStock
{
    public required IReadOnlyList<CableStockRow> Rows { get; init; }

    public required int CableCount { get; init; }

    public required double TotalLengthM { get; init; }

    /// <summary>Кабели, для которых не нашлось пути по трассам.</summary>
    public required int UnroutedCables { get; init; }
}
