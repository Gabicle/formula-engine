
namespace FormulaEngine.Api.Engine;

public class TopologicalSorter
{
    public static IReadOnlyList<CellNode> Sort(DependencyGraph graph)
    {

        var inDegree = new Dictionary<Guid, int>();
        var queue = new Queue<CellNode>();
        var result = new List<CellNode>();

        //init in-degree: no of dependencies a cell needs
        foreach (var node in graph.GetAllNodes())
        {
            inDegree[node.Id] = graph.GetIncoming(node.Id).Count;
        }


        //process zero in-degrees
        foreach (var node in graph.GetAllNodes())
        {
            if (inDegree[node.Id] == 0)
            {
                queue.Enqueue(node);
            }

        }


        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            //process what depends on current
            foreach (var neighborId in graph.GetOutgoing(current.Id))
            {
                inDegree[neighborId]--;

                if (inDegree[neighborId] != 0) continue;
                var neighbor = graph.GetNode(neighborId);
                if (neighbor is not null)
                    queue.Enqueue(neighbor);

            }
        }

        if (result.Count != graph.GetAllNodes().Count())
            throw new InvalidOperationException(
                "Circular dependency detected. The graph contains a cycle.");

        return result;
    }
}
