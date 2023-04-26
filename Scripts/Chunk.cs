namespace Planets;

public class Chunk
{
    public Dictionary<int, Vector3[]> Edges { get; } = new();

    private int NumMidPoints { get; }
    private int NumEdgePoints { get; }

    private Node Parent { get; }

    public Chunk(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int subdivisions)
    {
        Parent = parent;

        NumMidPoints = Mathf.Max(0, (int)Mathf.Pow(2, subdivisions) - 1);
        NumEdgePoints = NumMidPoints + 2;

        for (int i = 0; i < Edges.Count; i++)
            Edges[i] = new Vector3[NumEdgePoints];

        Edges[0] = GenerateEdgePoints(posA, posB, subdivisions);
        Edges[1] = GenerateEdgePoints(posA, posC, subdivisions);
        Edges[2] = GenerateEdgePoints(posB, posC, subdivisions);

        // Debug
        foreach (var edge in Edges)
            foreach (var point in edge.Value)
                new DebugSphere(Parent, point);
    }

    private Vector3[] GenerateEdgePoints(Vector3 posA, Vector3 posB, int subdivisions)
    {
        var points = new Vector3[NumEdgePoints];

        // Generate first point
        points[0] = posA;

        // Generate last point
        points[NumEdgePoints - 1] = posB;

        // Generate points between first and last
        GenerateEdgeMidPoints(points, subdivisions);

        return points;
    }

    private void GenerateEdgeMidPoints(Vector3[] points, int subdivisions)
    {
        var posA = points[0];
        var posB = points[NumEdgePoints - 1];

        for (int i = 0; i < NumMidPoints; i++)
        {
            // Calculate mid points
            var t = (float)(i + 1) / (NumMidPoints + 1);
            var pos = posA.Lerp(posB, t);

            points[i + 1] = pos;
        }
    }
}
