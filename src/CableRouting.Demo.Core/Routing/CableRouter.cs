using CableRouting.Demo.Core.Model;

namespace CableRouting.Demo.Core.Routing;

/// <summary>
/// Поиск кратчайшего пути по графу трасс. Вес ребра — геометрическая длина участка.
/// Граф неориентированный: кабель можно проложить в любую сторону.
/// </summary>
public sealed class CableRouter
{
    private readonly Dictionary<string, TrayNode> _nodes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, TraySegment> _segments = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<Edge>> _adjacency = new(StringComparer.Ordinal);

    public CableRouter(IReadOnlyList<TrayNode> nodes, IReadOnlyList<TraySegment> trays)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(trays);

        foreach (var node in nodes)
        {
            if (!_nodes.TryAdd(node.Id, node))
                throw new InvalidOperationException($"Узел трассы с Id '{node.Id}' объявлен дважды.");
        }

        foreach (var node in nodes)
            _adjacency[node.Id] = new List<Edge>();

        foreach (var tray in trays)
        {
            if (!_segments.TryAdd(tray.Id, tray))
                throw new InvalidOperationException($"Участок трассы с Id '{tray.Id}' объявлен дважды.");

            if (!_nodes.TryGetValue(tray.FromNodeId, out var from))
                throw new InvalidOperationException(
                    $"Участок '{tray.Id}' ссылается на неизвестный узел '{tray.FromNodeId}'.");

            if (!_nodes.TryGetValue(tray.ToNodeId, out var to))
                throw new InvalidOperationException(
                    $"Участок '{tray.Id}' ссылается на неизвестный узел '{tray.ToNodeId}'.");

            if (tray.FromNodeId == tray.ToNodeId)
                throw new InvalidOperationException($"Участок '{tray.Id}' соединяет узел сам с собой.");

            if (tray.UsableAreaMm2 <= 0)
                throw new InvalidOperationException(
                    $"У участка '{tray.Id}' не задана полезная площадь сечения.");

            double lengthMm = from.Position.DistanceTo(to.Position);
            _adjacency[tray.FromNodeId].Add(new Edge(tray.ToNodeId, tray.Id, lengthMm));
            _adjacency[tray.ToNodeId].Add(new Edge(tray.FromNodeId, tray.Id, lengthMm));
        }
    }

    public bool ContainsNode(string nodeId) => _nodes.ContainsKey(nodeId);

    public TrayNode GetNode(string nodeId) =>
        _nodes.TryGetValue(nodeId, out var node)
            ? node
            : throw new KeyNotFoundException($"Узел трассы '{nodeId}' не найден.");

    public TraySegment GetSegment(string segmentId) =>
        _segments.TryGetValue(segmentId, out var segment)
            ? segment
            : throw new KeyNotFoundException($"Участок трассы '{segmentId}' не найден.");

    public double GetSegmentLengthMm(string segmentId)
    {
        var segment = GetSegment(segmentId);
        return GetNode(segment.FromNodeId).Position.DistanceTo(GetNode(segment.ToNodeId).Position);
    }

    /// <summary>Кратчайший путь между узлами. Если пути нет — <see cref="RouteResult.Unreachable"/>.</summary>
    public RouteResult Route(string fromNodeId, string toNodeId)
    {
        if (!_nodes.ContainsKey(fromNodeId))
            throw new KeyNotFoundException($"Узел трассы '{fromNodeId}' не найден.");
        if (!_nodes.ContainsKey(toNodeId))
            throw new KeyNotFoundException($"Узел трассы '{toNodeId}' не найден.");

        if (fromNodeId == toNodeId)
            return RouteResult.Success(new[] { fromNodeId }, Array.Empty<string>(), 0);

        var distance = new Dictionary<string, double>(StringComparer.Ordinal);
        var previousNode = new Dictionary<string, string>(StringComparer.Ordinal);
        var previousSegment = new Dictionary<string, string>(StringComparer.Ordinal);
        var queue = new PriorityQueue<string, double>();

        foreach (var id in _nodes.Keys)
            distance[id] = double.PositiveInfinity;
        distance[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);

        while (queue.TryDequeue(out var current, out var currentDistance))
        {
            if (currentDistance > distance[current])
                continue;
            if (current == toNodeId)
                break;

            foreach (var edge in _adjacency[current])
            {
                double candidate = currentDistance + edge.LengthMm;
                if (candidate >= distance[edge.ToNodeId])
                    continue;

                distance[edge.ToNodeId] = candidate;
                previousNode[edge.ToNodeId] = current;
                previousSegment[edge.ToNodeId] = edge.SegmentId;
                queue.Enqueue(edge.ToNodeId, candidate);
            }
        }

        if (double.IsPositiveInfinity(distance[toNodeId]))
            return RouteResult.Unreachable(
                $"Между узлами '{fromNodeId}' и '{toNodeId}' нет пути по трассам.");

        var nodePath = new List<string>();
        var segmentPath = new List<string>();
        for (string? at = toNodeId; at is not null; at = previousNode.GetValueOrDefault(at))
        {
            nodePath.Add(at);
            if (at == fromNodeId)
                break;
            segmentPath.Add(previousSegment[at]);
        }

        nodePath.Reverse();
        segmentPath.Reverse();
        return RouteResult.Success(nodePath, segmentPath, distance[toNodeId]);
    }

    private sealed record Edge(string ToNodeId, string SegmentId, double LengthMm);
}
