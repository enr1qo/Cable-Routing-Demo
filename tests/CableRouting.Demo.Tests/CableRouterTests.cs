using CableRouting.Demo.Core.Model;
using CableRouting.Demo.Core.Routing;

namespace CableRouting.Demo.Tests;

public class CableRouterTests
{
    [Fact]
    public void Route_PicksShorterPath()
    {
        var nodes = new[]
        {
            TestProject.MakeNode("A", 0, 0),
            TestProject.MakeNode("B", 10, 0),
            TestProject.MakeNode("C", 20, 0),
            TestProject.MakeNode("D", 10, 10)
        };
        var trays = new[]
        {
            TestProject.MakeTray("T1", "A", "B"),
            TestProject.MakeTray("T2", "B", "C"),
            TestProject.MakeTray("T3", "A", "D"),
            TestProject.MakeTray("T4", "D", "C")
        };

        var router = new CableRouter(nodes, trays);
        var route = router.Route("A", "C");

        Assert.True(route.Found);
        Assert.Equal(new[] { "A", "B", "C" }, route.NodeIds);
        Assert.Equal(new[] { "T1", "T2" }, route.SegmentIds);
        Assert.Equal(20_000.0, route.LengthMm, 6);
    }

    [Fact]
    public void Route_ReturnsUnreachable_WhenNoPathExists()
    {
        var nodes = new[]
        {
            TestProject.MakeNode("A", 0, 0),
            TestProject.MakeNode("B", 10, 0),
            TestProject.MakeNode("Z", 0, 10)
        };
        var trays = new[] { TestProject.MakeTray("T1", "A", "B") };

        var router = new CableRouter(nodes, trays);
        var route = router.Route("A", "Z");

        Assert.False(route.Found);
        Assert.Empty(route.NodeIds);
        Assert.Empty(route.SegmentIds);
        Assert.NotNull(route.Error);
        Assert.Contains("нет пути", route.Error);
    }

    [Fact]
    public void Route_ReturnsZeroLength_ForSameNode()
    {
        var nodes = new[] { TestProject.MakeNode("A", 0, 0) };
        var router = new CableRouter(nodes, Array.Empty<TraySegment>());

        var route = router.Route("A", "A");

        Assert.True(route.Found);
        Assert.Equal(new[] { "A" }, route.NodeIds);
        Assert.Empty(route.SegmentIds);
        Assert.Equal(0.0, route.LengthMm);
    }

    [Fact]
    public void Route_Throws_ForUnknownNode()
    {
        var nodes = new[] { TestProject.MakeNode("A", 0, 0) };
        var router = new CableRouter(nodes, Array.Empty<TraySegment>());

        Assert.Throws<KeyNotFoundException>(() => router.Route("A", "B"));
    }

    [Fact]
    public void Constructor_Throws_ForTrayWithUnknownNode()
    {
        var nodes = new[] { TestProject.MakeNode("A", 0, 0) };
        var trays = new[] { TestProject.MakeTray("T1", "A", "B") };

        var ex = Assert.Throws<InvalidOperationException>(() => new CableRouter(nodes, trays));
        Assert.Contains("неизвестный узел", ex.Message);
    }

    [Fact]
    public void Constructor_Throws_ForNonPositiveArea()
    {
        var nodes = new[]
        {
            TestProject.MakeNode("A", 0, 0),
            TestProject.MakeNode("B", 10, 0)
        };
        var trays = new[] { TestProject.MakeTray("T1", "A", "B", usableAreaMm2: 0) };

        var ex = Assert.Throws<InvalidOperationException>(() => new CableRouter(nodes, trays));
        Assert.Contains("площадь", ex.Message);
    }
}
