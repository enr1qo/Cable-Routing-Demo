namespace CableRouting.Demo.Core.Calculation;

/// <summary>Параметры расчёта: запас кабеля и порог заполнения лотка.</summary>
public sealed class CalculationOptions
{
    /// <summary>Запас на разводку и укладку, % от геометрической длины.</summary>
    public double SlackPercent { get; init; } = 10.0;

    /// <summary>Фиксированный запас на каждое подключение, м.</summary>
    public double SlackMetersPerEnd { get; init; } = 1.0;

    /// <summary>Порог заполнения лотка, %.</summary>
    public double TrayFillLimitPercent { get; init; } = 50.0;

    public void Validate()
    {
        if (SlackPercent < 0)
            throw new ArgumentOutOfRangeException(nameof(SlackPercent), "Запас не может быть отрицательным.");
        if (SlackMetersPerEnd < 0)
            throw new ArgumentOutOfRangeException(nameof(SlackMetersPerEnd), "Запас не может быть отрицательным.");
        if (TrayFillLimitPercent <= 0)
            throw new ArgumentOutOfRangeException(nameof(TrayFillLimitPercent), "Порог заполнения должен быть положительным.");
    }
}
