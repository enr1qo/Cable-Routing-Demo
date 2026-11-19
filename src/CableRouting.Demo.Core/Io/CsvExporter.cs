using System.Globalization;
using System.Text;
using CableRouting.Demo.Core.Calculation;

namespace CableRouting.Demo.Core.Io;

/// <summary>
/// Выгрузка отчётов в CSV для Excel: разделитель ';', UTF-8 с BOM, десятичная точка.
/// Простая замена Excel-выгрузке реального проекта, чтобы демо не зависело от Interop.
/// </summary>
public static class CsvExporter
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public static void WriteCableStock(string path, CableStock stock)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Application;CableCount;TotalLengthM;MaxLengthM");
        foreach (var row in stock.Rows)
        {
            sb.Append(Escape(row.Application)).Append(';')
              .Append(row.CableCount).Append(';')
              .Append(row.TotalLengthM.ToString("0.00", Invariant)).Append(';')
              .Append(row.MaxLengthM.ToString("0.00", Invariant)).AppendLine();
        }
        Write(path, sb);
    }

    public static void WriteTrayFill(string path, TrayFill fill)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TrayId;Type;LengthM;CableCount;UsedAreaMm2;FillPercent;Overloaded");
        foreach (var row in fill.Rows)
        {
            sb.Append(Escape(row.TrayId)).Append(';')
              .Append(Escape(row.Type)).Append(';')
              .Append(row.LengthM.ToString("0.00", Invariant)).Append(';')
              .Append(row.CableCount).Append(';')
              .Append(row.UsedAreaMm2.ToString("0.00", Invariant)).Append(';')
              .Append(row.FillPercent.ToString("0.00", Invariant)).Append(';')
              .Append(row.Overloaded ? "yes" : "no").AppendLine();
        }
        Write(path, sb);
    }

    private static string Escape(string value) =>
        value.Contains(';') || value.Contains('"')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

    private static void Write(string path, StringBuilder sb)
    {
        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(fullPath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }
}
