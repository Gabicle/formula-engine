using FormulaEngine.Api.Engine;

namespace FormulaEngine.Tests;

public class TopologicalSorterTests
{
    private static CellNode MakeNode(string key) =>
        new CellNode(Guid.NewGuid(), Guid.NewGuid(), key, 0, null);

    [Fact]
    public void Sort_ValidGraph_ReturnsNodesInCorrectOrder()
    {
        // Arrange
        var units = MakeNode("Units");
        var price = MakeNode("Price");
        var revenue = MakeNode("Revenue");
        var cogs = MakeNode("COGS");
        var grossProfit = MakeNode("GrossProfit");

        var graph = new DependencyGraph();
        graph.AddNode(units);
        graph.AddNode(price);
        graph.AddNode(revenue);
        graph.AddNode(cogs);
        graph.AddNode(grossProfit);

        graph.AddEdge(units.Id, revenue.Id);
        graph.AddEdge(price.Id, revenue.Id);
        graph.AddEdge(units.Id, cogs.Id);
        graph.AddEdge(revenue.Id, grossProfit.Id);
        graph.AddEdge(cogs.Id, grossProfit.Id);

        // Act
        var result = TopologicalSorter.Sort(graph);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(5, resultList.Count);
        Assert.True(resultList.IndexOf(units) < resultList.IndexOf(revenue));
        Assert.True(resultList.IndexOf(price) < resultList.IndexOf(revenue));
        Assert.True(resultList.IndexOf(units) < resultList.IndexOf(cogs));
        Assert.True(resultList.IndexOf(revenue) < resultList.IndexOf(grossProfit));
        Assert.True(resultList.IndexOf(cogs) < resultList.IndexOf(grossProfit));
    }

    [Fact]
    public void Sort_GraphWithCycle_ThrowsInvalidOperationException()
    {
        // Arrange
        var a = MakeNode("A");
        var b = MakeNode("B");
        var c = MakeNode("C");

        var graph = new DependencyGraph();
        graph.AddNode(a);
        graph.AddNode(b);
        graph.AddNode(c);

        graph.AddEdge(a.Id, b.Id);
        graph.AddEdge(b.Id, c.Id);
        graph.AddEdge(c.Id, a.Id);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => TopologicalSorter.Sort(graph));
    }

    [Fact]
    public void Sort_SingleNode_ReturnsThatNode()
    {
        // Arrange
        var units = MakeNode("Units");
        var graph = new DependencyGraph();
        graph.AddNode(units);

        // Act
        var result = TopologicalSorter.Sort(graph);

        // Assert
        Assert.Single(result);
        Assert.Equal(units.Id, result[0].Id);
    }
}