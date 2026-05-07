namespace FormulaEngine.Api.Engine;

public class DependencyGraph
{
    private readonly Dictionary<Guid, CellNode> _nodes = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _outgoingEdges = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _incomingEdges = new();

    public void AddNode(CellNode node)
    {
        _nodes[node.Id] = node;
        _outgoingEdges.TryAdd(node.Id, []);
        _incomingEdges.TryAdd(node.Id, []);
    }

    public void AddEdge(Guid fromCellId, Guid toCellId)
    {
        _outgoingEdges[fromCellId].Add(toCellId);
        _incomingEdges[toCellId].Add(fromCellId);
    }

    public CellNode? GetNode(Guid id) => _nodes.GetValueOrDefault(id);

    public IReadOnlySet<Guid> GetOutgoing(Guid id) =>
        _outgoingEdges.GetValueOrDefault(id) ?? [];

    public IReadOnlySet<Guid> GetIncoming(Guid id) =>
        _incomingEdges.GetValueOrDefault(id) ?? [];

    public IEnumerable<CellNode> GetAllNodes() => _nodes.Values;
}
