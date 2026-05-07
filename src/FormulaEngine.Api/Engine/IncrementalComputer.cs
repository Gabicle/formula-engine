
namespace FormulaEngine.Api.Engine;

public class IncrementalComputer
{
    public static IReadOnlySet<Guid> FindAffectedNodes(
        DependencyGraph graph,
        Guid changedNodeId)
    {
        var affected = new HashSet<Guid>();
        var queue = new Queue<Guid>();

        //add all nodes that depend on changed nodes
        foreach (var neighborId in graph.GetOutgoing(changedNodeId))
        {
            queue.Enqueue(neighborId);
        }

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            //skip if already visited
            if (!affected.Add(currentId))
            {
                continue;
            }

            //add neighbors that depend on current to queue
            foreach (var neighbor in graph.GetOutgoing(currentId))
            {
                if (!affected.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }

        }
        return affected;
    }
}
