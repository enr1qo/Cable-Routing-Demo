namespace CableRouting.Demo.Core.Calculation;

/// <summary>Строка заполнения участка трассы.</summary>
public sealed class TrayFillRow
{
    public required string TrayId { get; init; }

    public required string Type { get; init; }

    public required double LengthM { get; init; }

    public required int CableCount { get; init; }

    public required double UsedAreaMm2 { get; init; }

    public required double FillPercent { get; init; }

    public required bool Overloaded { get; init; }
}

/// <summary>Сводка заполнения лотков.</summary>
public sealed class TrayFill
{
    public required IReadOnlyList<TrayFillRow> Rows { get; init; }

    public required int OverloadedCount { get; init; }

    public required double MaxFillPercent { get; init; }
}
