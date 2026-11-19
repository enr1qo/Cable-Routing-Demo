using CableRouting.Demo.Core.Calculation;
using CableRouting.Demo.Core.Model;

namespace CableRouting.Demo.Tests;

public class TrayFillTests
{
    [Fact]
    public void Calculate_SumsCableAreas_AndFlagsOverload()
    {
        var project = TestProject.MakeProject(
            new[]
            {
                TestProject.MakeNode("A", 0, 0),
                TestProject.MakeNode("B", 10, 0)
            },
            new[] { TestProject.MakeTray("T1", "A", "B", usableAreaMm2: 1000) },
            new[]
            {
                TestProject.MakeDevice("D1", "A", DeviceKind.Cabinet),
                TestProject.MakeDevice("D2", "B")
            },
            new[]
            {
                TestProject.MakeCable("K1", "D1", "D2", diameterMm: 10),
                TestProject.MakeCable("K2", "D1", "D2", diameterMm: 10),
                TestProject.MakeCable("K3", "D1", "D2", diameterMm: 6)
            });

        var report = ProjectCalculator.Calculate(
            project,
            new CalculationOptions { SlackPercent = 0, SlackMetersPerEnd = 0, TrayFillLimitPercent = 10 });

        var row = Assert.Single(report.TrayFill.Rows);
        Assert.Equal(3, row.CableCount);
        Assert.Equal(185.35, row.UsedAreaMm2, 2);
        Assert.Equal(18.54, row.FillPercent, 2);
        Assert.True(row.Overloaded);
        Assert.Equal(1, report.TrayFill.OverloadedCount);
        Assert.Equal(18.54, report.TrayFill.MaxFillPercent, 2);
    }

    [Fact]
    public void Calculate_LeavesUnusedTrayAtZero()
    {
        var project = TestProject.MakeProject(
            new[]
            {
                TestProject.MakeNode("A", 0, 0),
                TestProject.MakeNode("B", 10, 0),
                TestProject.MakeNode("C", 20, 0)
            },
            new[]
            {
                TestProject.MakeTray("T1", "A", "B", usableAreaMm2: 1000),
                TestProject.MakeTray("T2", "B", "C", usableAreaMm2: 1000)
            },
            new[]
            {
                TestProject.MakeDevice("D1", "A", DeviceKind.Cabinet),
                TestProject.MakeDevice("D2", "B")
            },
            new[] { TestProject.MakeCable("K1", "D1", "D2", diameterMm: 10) });

        var report = ProjectCalculator.Calculate(
            project,
            new CalculationOptions { SlackPercent = 0, SlackMetersPerEnd = 0 });

        var t2 = Assert.Single(report.TrayFill.Rows, row => row.TrayId == "T2");
        Assert.Equal(0, t2.CableCount);
        Assert.Equal(0.0, t2.UsedAreaMm2);
        Assert.Equal(0.0, t2.FillPercent);
        Assert.False(t2.Overloaded);
    }

    [Fact]
    public void Calculate_CountsCableOnEverySegmentOfRoute()
    {
        var project = TestProject.MakeProject(
            new[]
            {
                TestProject.MakeNode("A", 0, 0),
                TestProject.MakeNode("B", 10, 0),
                TestProject.MakeNode("C", 20, 0)
            },
            new[]
            {
                TestProject.MakeTray("T1", "A", "B", usableAreaMm2: 1000),
                TestProject.MakeTray("T2", "B", "C", usableAreaMm2: 1000)
            },
            new[]
            {
                TestProject.MakeDevice("D1", "A", DeviceKind.Cabinet),
                TestProject.MakeDevice("D2", "C")
            },
            new[] { TestProject.MakeCable("K1", "D1", "D2", diameterMm: 10) });

        var report = ProjectCalculator.Calculate(
            project,
            new CalculationOptions { SlackPercent = 0, SlackMetersPerEnd = 0 });

        Assert.All(report.TrayFill.Rows, row =>
        {
            Assert.Equal(1, row.CableCount);
            Assert.Equal(78.54, row.UsedAreaMm2, 2);
        });
    }
}
