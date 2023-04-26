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

        // The number of mid points is always 2^n, so 1, 2, 4, 8, 16...
        NumMidPoints = Mathf.Max(0, (int)Mathf.Pow(2, subdivisions) - 1);
        NumEdgePoints = NumMidPoints + 2;

        for (int i = 0; i < Edges.Count; i++)
            Edges[i] = new Vector3[NumEdgePoints];

        Edges[0] = GenerateEdgePoints(posA, posB, subdivisions);
        Edges[1] = GenerateEdgePoints(posA, posC, subdivisions);
        Edges[2] = GenerateEdgePoints(posB, posC, subdivisions);

        CenterPoints = GenerateCenterPoints();

        GenerateMesh();

        // Debug
        foreach (var edge in Edges)
            foreach (var point in edge.Value)
                new DebugPoint(Parent, point);

        foreach (var point in CenterPoints)
            new DebugPoint(Parent, point).SetColor(Colors.Yellow);
    }

    private void GenerateMesh()
    {
        var vertices = new List<Vector3>();

        vertices.AddRange(Edges[0]);
        vertices.AddRange(Edges[1]);
        vertices.AddRange(Edges[2]);
        vertices.AddRange(CenterPoints);

        new DebugPoint(Parent, Edges[0][0])
            .SetColor(Colors.Purple)
            .SetRadius(0.1f);

        new DebugPoint(Parent, Edges[0][1])
            .SetColor(Colors.Purple)
            .SetRadius(0.1f);

        new DebugPoint(Parent, Edges[1][1])
            .SetColor(Colors.Purple)
            .SetRadius(0.1f);

        var r = 0; // Right Edge
        var l = NumEdgePoints; // Left Edge
        var b = NumEdgePoints * 2; // Bottom Edge
        var c = NumEdgePoints * 3; // Center Points

        var indices = new int[]
        {
            r    , r + 1, l + 1,
            r + 2, c    , r + 1,
            c    , l + 1, r + 1,
            c    , l + 2, l + 1,
        };

        var mesh = World3DUtils.CreateMesh(vertices.ToArray(), indices);

        Parent.AddChild(new MeshInstance3D
        {
            Mesh = mesh
        });
    }

    private Vector3[] GenerateCenterPoints()
    {
        // Take note of i such that i >= 2 and i <= n - 1, there will always
        // be i - 1 center points for each row. All up all the rows to get the
        // number of center points.
        var numCenterPoints = GUMath.SumNatrualNumbers(NumMidPoints);

        var centerPoints = new Vector3[numCenterPoints];
        var index = 0;

        // There will never be any center points when i = 0 or i = 1
        // The center edge points at i = n do not count as 'center points'
        // because they are touching the outer edge
        for (int i = 2; i < NumEdgePoints - 1; i++)
        {
            // There will always be i - 1 center points for each row
            var centerPointsCount = i - 1;

            // Get the left and right edge points
            var leftEdgePoint = Edges[0][i];
            var rightEdgePoint = Edges[1][i];

            for (int j = 0; j < centerPointsCount; j++)
            {
                // Calculate the points in between the left and right edge points
                var t = (j + 1f) / (centerPointsCount + 1f);
                var pos = leftEdgePoint.Lerp(rightEdgePoint, t);

                centerPoints[index++] = pos;
            }
        }

        return centerPoints;
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
