using System.Globalization;
using CableRouting.Demo.Core.Calculation;
using CableRouting.Demo.Core.Io;
using CableRouting.Demo.Sample;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = System.Text.Encoding.UTF8;

var project = DemoProjectFactory.Create();
var report = ProjectCalculator.Calculate(project);

Console.WriteLine("=== Cable Routing Demo ===");
Console.WriteLine(
    $"Проект: {project.Nodes.Count} узлов, {project.Trays.Count} участков, " +
    $"{project.Devices.Count} устройств, {project.Cables.Count} кабелей");
Console.WriteLine();

PrintRoutes(report);
PrintCableStock(report.CableStock);
PrintTrayFill(report.TrayFill);

string outputDirectory = args.Length > 0 ? args[0] : "out";
string stockPath = Path.Combine(outputDirectory, "cable-stock.csv");
string fillPath = Path.Combine(outputDirectory, "tray-fill.csv");
CsvExporter.WriteCableStock(stockPath, report.CableStock);
CsvExporter.WriteTrayFill(fillPath, report.TrayFill);
Console.WriteLine($"CSV: {Path.GetFullPath(stockPath)}");
Console.WriteLine($"CSV: {Path.GetFullPath(fillPath)}");

static void PrintRoutes(ProjectReport report)
{
    Console.WriteLine("--- Маршруты ---");
    foreach (var route in report.Routes)
    {
        string length = route.Found
            ? $"{route.LengthM:0.00} м"
            : "НЕТ ПУТИ";
        string path = route.Found
            ? string.Join(" -> ", route.Route.NodeIds)
            : route.Route.Error ?? "";
        Console.WriteLine(
            $"{route.Cable.Id,-12} {route.From.Name,-5} -> {route.To.Name,-5} {length,9}  {path}");
    }
    Console.WriteLine();
}

static void PrintCableStock(CableStock stock)
{
    Console.WriteLine("--- Метраж кабеля ---");
    Console.WriteLine($"{"Приложение",-18} {"Кабелей",8} {"Длина, м",10} {"Макс, м",10}");
    foreach (var row in stock.Rows)
    {
        Console.WriteLine(
            $"{row.Application,-18} {row.CableCount,8} {row.TotalLengthM,10:0.00} {row.MaxLengthM,10:0.00}");
    }
    Console.WriteLine(
        $"{"Итого",-18} {stock.CableCount,8} {stock.TotalLengthM,10:0.00}");

    if (stock.UnroutedCables > 0)
        Console.WriteLine($"Без маршрута: {stock.UnroutedCables}");
    Console.WriteLine();
}

static void PrintTrayFill(TrayFill fill)
{
    Console.WriteLine("--- Заполнение лотков ---");
    Console.WriteLine(
        $"{"Участок",-8} {"Тип",-16} {"Длина, м",8} {"Кабелей",8} " +
        $"{"Площадь",9} {"Заполн.",8} {"Статус",7}");

    foreach (var row in fill.Rows)
    {
        Console.WriteLine(
            $"{row.TrayId,-8} {row.Type,-16} {row.LengthM,8:0.00} {row.CableCount,8} " +
            $"{row.UsedAreaMm2,9:0.00} {row.FillPercent,7:0.00}% " +
            $"{(row.Overloaded ? "OVER" : "OK"),-7}");
    }

    Console.WriteLine($"Перегружено: {fill.OverloadedCount}, максимум: {fill.MaxFillPercent:0.00}%");
}
