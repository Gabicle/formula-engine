using FormulaEngine.Api.Engine;

namespace FormulaEngine.Tests;

public class IncrementalComputerTests
{
    private static CellNode MakeNode(string key) =>
        new CellNode(Guid.NewGuid(), Guid.NewGuid(), key, 0, null);

    [Fact]
    public void FindAffectedNodes_ChangedNode_ReturnsAllDownstreamNodes()
    {
        // Arrange
        var units = MakeNode("Units");
        var cogs = MakeNode("COGS");
        var revenue = MakeNode("Revenue");
        var grossProfit = MakeNode("GrossProfit");

        var graph = new DependencyGraph();
        graph.AddNode(units);
        graph.AddNode(cogs);
        graph.AddNode(revenue);
        graph.AddNode(grossProfit);

        graph.AddEdge(units.Id, cogs.Id);
        graph.AddEdge(units.Id, revenue.Id);
        graph.AddEdge(cogs.Id, grossProfit.Id);
        graph.AddEdge(revenue.Id, grossProfit.Id);

        // Act
        var affected = IncrementalComputer.FindAffectedNodes(graph, units.Id);

        // Assert
        Assert.Equal(3, affected.Count);
        Assert.Contains(cogs.Id, affected);
        Assert.Contains(revenue.Id, affected);
        Assert.Contains(grossProfit.Id, affected);
    }

    [Fact]
    public void FindAffectedNodes_NodeWithNoOutgoingEdges_ReturnsEmptySet()
    {
        // Arrange
        var units = MakeNode("Units");
        var graph = new DependencyGraph();
        graph.AddNode(units);

        // Act
        var affected = IncrementalComputer.FindAffectedNodes(graph, units.Id);

        // Assert
        Assert.Empty(affected);
    }

    [Fact]
    public void FindAffectedNodes_ChangedNodeNotIncludedInResult()
    {
        // Arrange
        var units = MakeNode("Units");
        var revenue = MakeNode("Revenue");

        var graph = new DependencyGraph();
        graph.AddNode(units);
        graph.AddNode(revenue);
        graph.AddEdge(units.Id, revenue.Id);

        // Act
        var affected = IncrementalComputer.FindAffectedNodes(graph, units.Id);

        // Assert
        Assert.DoesNotContain(units.Id, affected);
    }
}