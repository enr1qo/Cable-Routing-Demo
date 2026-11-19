using CableRouting.Demo.Core.Calculation;
using CableRouting.Demo.Core.Io;

namespace CableRouting.Demo.Tests;

public class CsvExporterTests
{
    [Fact]
    public void WriteCableStock_CreatesUtf8BomFileWithHeader()
    {
        string directory = CreateTempDirectory();
        try
        {
            var stock = new CableStock
            {
                Rows = new[]
                {
                    new CableStockRow
                    {
                        Application = "СКС",
                        CableCount = 2,
                        TotalLengthM = 12.5,
                        MaxLengthM = 7.5
                    }
                },
                CableCount = 2,
                TotalLengthM = 12.5,
                UnroutedCables = 0
            };
            string path = Path.Combine(directory, "cable-stock.csv");

            CsvExporter.WriteCableStock(path, stock);

            byte[] bytes = File.ReadAllBytes(path);
            Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes.Take(3));
            string text = File.ReadAllText(path);
            Assert.Contains("Application;CableCount;TotalLengthM;MaxLengthM", text);
            Assert.Contains("СКС;2;12.50;7.50", text);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void WriteTrayFill_EscapesSemicolonAndWritesStatus()
    {
        string directory = CreateTempDirectory();
        try
        {
            var fill = new TrayFill
            {
                Rows = new[]
                {
                    new TrayFillRow
                    {
                        TrayId = "T1",
                        Type = "LADDER;100",
                        LengthM = 6.0,
                        CableCount = 3,
                        UsedAreaMm2 = 185.35,
                        FillPercent = 61.78,
                        Overloaded = true
                    }
                },
                OverloadedCount = 1,
                MaxFillPercent = 61.78
            };
            string path = Path.Combine(directory, "tray-fill.csv");

            CsvExporter.WriteTrayFill(path, fill);

            string text = File.ReadAllText(path);
            Assert.Contains("TrayId;Type;LengthM;CableCount;UsedAreaMm2;FillPercent;Overloaded", text);
            Assert.Contains("\"LADDER;100\"", text);
            Assert.Contains("T1;\"LADDER;100\";6.00;3;185.35;61.78;yes", text);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            "cable-routing-demo-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
