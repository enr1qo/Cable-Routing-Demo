namespace CableRouting.Demo.Core.Calculation;

/// <summary>Полный отчёт по проекту: маршруты, метраж кабеля, заполнение лотков.</summary>
public sealed class ProjectReport
{
    public required IReadOnlyList<CableRoute> Routes { get; init; }

    public required CableStock CableStock { get; init; }

    public required TrayFill TrayFill { get; init; }
}
