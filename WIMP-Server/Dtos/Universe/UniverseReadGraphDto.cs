using System.Collections.Generic;

namespace WIMP_Server.Dtos.Universe;

public class Node
{
    public int SystemId { get; set; }

    public string SystemName { get; set; }
}

public class Edge
{
    public int SourceSystemId { get; set; }
    public int DestinationSystemId { get; set; }
}

public class UniverseReadGraphDto
{
    public IEnumerable<Node> Systems { get; set; }

    public IEnumerable<Edge> Edges { get; set; }
}
