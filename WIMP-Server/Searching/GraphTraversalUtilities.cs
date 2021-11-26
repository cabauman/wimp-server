using System;
using System.Collections.Generic;
using System.Linq;

namespace WIMP_Server.Searching;

public static class GraphTraversalUtilities
{
    public static IEnumerable<TNode> BreadthFirstSearchWithinDistance<TNode>(
        TNode startNode,
        int maximumDistance,
        Func<TNode, int> idSelector,
        Func<TNode, IEnumerable<TNode>> neighboursSelector)
    {
        var nodeQueue = new Queue<Tuple<TNode, int>>();
        var visitedNodes = new Dictionary<int, TNode>();

        nodeQueue.Enqueue(Tuple.Create(startNode, 0));

        while (nodeQueue.Count > 0)
        {
            var (node, distance) = nodeQueue.Dequeue();
            var nodeId = idSelector(node);

            if (!visitedNodes.ContainsKey(nodeId))
            {
                visitedNodes.Add(nodeId, node);

                if (distance >= maximumDistance)
                {
                    continue;
                }

                foreach (var neighbour in neighboursSelector(node))
                {
                    nodeQueue.Enqueue(Tuple.Create(neighbour, distance + 1));
                }
            }
        }

        return visitedNodes.Values.ToList();
    }
}
