using CableRouting.Demo.Core.Calculation;
using CableRouting.Demo.Core.Model;

namespace CableRouting.Demo.Tests;

public class CableStockTests
{
    [Fact]
    public void Calculate_GroupsByApplication_AndAppliesSlack()
    {
        var project = BuildProject();

        var report = ProjectCalculator.Calculate(
            project,
            new CalculationOptions { SlackPercent = 10, SlackMetersPerEnd = 1 });

        var stock = report.CableStock;
        Assert.Equal(4, stock.CableCount);
        Assert.Equal(1, stock.UnroutedCables);
        Assert.Equal(63.0, stock.TotalLengthM, 2);

        var scs = Assert.Single(stock.Rows, row => row.Application == "СКС");
        Assert.Equal(3, scs.CableCount);
        Assert.Equal(50.0, scs.TotalLengthM, 2);
        Assert.Equal(24.0, scs.MaxLengthM, 2);

        var cctv = Assert.Single(stock.Rows, row => row.Application == "Видеонаблюдение");
        Assert.Equal(1, cctv.CableCount);
        Assert.Equal(13.0, cctv.TotalLengthM, 2);
        Assert.Equal(13.0, cctv.MaxLengthM, 2);
    }

    [Fact]
    public void Calculate_WithoutSlack_UsesPureRouteLength()
    {
        var project = BuildProject();

        var report = ProjectCalculator.Calculate(
            project,
            new CalculationOptions { SlackPercent = 0, SlackMetersPerEnd = 0 });

        var scs = Assert.Single(report.CableStock.Rows, row => row.Application == "СКС");
        Assert.Equal(40.0, scs.TotalLengthM, 2);
        Assert.Equal(20.0, scs.MaxLengthM, 2);
    }

    [Fact]
    public void Calculate_Throws_ForUnknownDevice()
    {
        var project = new Project
        {
            Nodes = new[]
            {
                TestProject.MakeNode("A", 0, 0),
                TestProject.MakeNode("B", 10, 0)
            },
            Trays = new[] { TestProject.MakeTray("T1", "A", "B") },
            Devices = new[]
            {
                TestProject.MakeDevice("D1", "A"),
                TestProject.MakeDevice("D2", "B")
            },
            Cables = new[] { TestProject.MakeCable("K1", "D1", "UNKNOWN") }
        };

        var ex = Assert.Throws<InvalidOperationException>(() => ProjectCalculator.Calculate(project));
        Assert.Contains("неизвестное устройство", ex.Message);
    }

    [Fact]
    public void Calculate_Throws_ForNonPositiveDiameter()
    {
        var baseProject = BuildProject();
        var project = new Project
        {
            Nodes = baseProject.Nodes,
            Trays = baseProject.Trays,
            Devices = baseProject.Devices,
            Cables = new[] { TestProject.MakeCable("K1", "D1", "D2", diameterMm: 0) }
        };

        var ex = Assert.Throws<InvalidOperationException>(() => ProjectCalculator.Calculate(project));
        Assert.Contains("положительный диаметр", ex.Message);
    }

    private static Project BuildProject()
    {
        return TestProject.MakeProject(
            new[]
            {
                TestProject.MakeNode("A", 0, 0),
                TestProject.MakeNode("B", 10, 0),
                TestProject.MakeNode("C", 20, 0),
                TestProject.MakeNode("Z", 0, 10)
            },
            new[]
            {
                TestProject.MakeTray("T1", "A", "B"),
                TestProject.MakeTray("T2", "B", "C")
            },
            new[]
            {
                TestProject.MakeDevice("D1", "A", DeviceKind.Cabinet),
                TestProject.MakeDevice("D2", "B"),
                TestProject.MakeDevice("D3", "C"),
                TestProject.MakeDevice("D4", "Z")
            },
            new[]
            {
                TestProject.MakeCable("K1", "D1", "D2"),
                TestProject.MakeCable("K2", "D1", "D3"),
                TestProject.MakeCable("K3", "D2", "D3"),
                TestProject.MakeCable("K5", "D1", "D2", diameterMm: 8, application: "Видеонаблюдение"),
                TestProject.MakeCable("K4", "D4", "D1")
            });
    }
}
