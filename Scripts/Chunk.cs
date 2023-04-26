namespace Planets;

public class Chunk
{
    private Dictionary<int, Vector3[]> Edges { get; } = new();
    private Vector3[] CenterPoints { get; }

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

        // Take note of i such that i >= 2 and i <= n - 1, there will always
        // be i - 1 center points for each row
        var numCenterPoints = GUMath.SumNatrualNumbers(NumMidPoints);

        CenterPoints = new Vector3[numCenterPoints];

        Edges[0] = GenerateEdgePoints(posA, posB, subdivisions);
        Edges[1] = GenerateEdgePoints(posA, posC, subdivisions);
        Edges[2] = GenerateEdgePoints(posB, posC, subdivisions);

        // Debug
        foreach (var edge in Edges)
            foreach (var point in edge.Value)
                new DebugPoint(Parent, point);

        GenerateCenterPoints();
    }

    private void GenerateCenterPoints()
    {
        for (int i = 2; i < Edges[0].Length - 1; i++)
        {
            var centerPoints = i - 1;

            var leftEdgePoint = Edges[0][i];
            var rightEdgePoint = Edges[1][i];

            for (int j = 0; j < centerPoints; j++)
            {
                var t = (j + 1f) / (centerPoints + 1f);
                var pos = leftEdgePoint.Lerp(rightEdgePoint, t);

                new DebugPoint(Parent, pos)
                    .SetColor(Colors.Yellow);
            }
        }
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
            var t = (i + 1f) / (NumMidPoints + 1f);
            var pos = posA.Lerp(posB, t);

            points[i + 1] = pos;
        }
    }
}
