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

        // Add the 3 corners
        vertices.Add(Edges[0][0]);
        vertices.Add(Edges[1][NumEdgePoints - 1]);
        vertices.Add(Edges[2][0]);

        // Add midpoints from each edge
        for (int i = 0; i < 3; i++)
        {
            // RemoveAt() is slow so that's why we use LinkedList instead of List
            var midpoints = new LinkedList<Vector3>(Edges[i]);

            // Remove the duplicate corner vertices
            midpoints.RemoveFirst();
            midpoints.RemoveLast();

            vertices.AddRange(midpoints);
        }
        
        // Add the center points
        vertices.AddRange(CenterPoints);

        var top = 0;
        var bottomLeft = 1;
        var bottomRight = 2;
        var r = 3; // right edge index
        var l = 3 + NumMidPoints; // left edge index
        var b = 3 + NumMidPoints * 2; // bottom edge index
        var c = 3 + NumMidPoints * 3; // center points index

        var mainCornerIndices = new int[]
        {
            // The main 3 corners
            top        , r                   , l                   ,
            bottomLeft , l + NumMidPoints - 1, b + NumMidPoints - 1,
            bottomRight, b                   , r + NumMidPoints - 1
        };

        var leftEdgeIndices = new List<int>();

        var cIndex = 0;

        for (int i = 0; i < NumMidPoints - 1; i++)
        {
            cIndex = i == 0 ? c : c + i + GUMath.SumNatrualNumbers(i + 1);

            leftEdgeIndices.AddRange(new int[] {
                l + i, cIndex, l + i + 1
            });
        }

        new DebugPoint(Parent, vertices[l + 0])
            .SetColor(Colors.Red)
            .SetRadius(0.04f);

        new DebugPoint(Parent, vertices[c])
            .SetColor(Colors.Green)
            .SetRadius(0.04f);

        new DebugPoint(Parent, vertices[l + 1])
            .SetColor(Colors.Blue)
            .SetRadius(0.04f);

        var indices = new List<int>();
        indices.AddRange(mainCornerIndices);
        indices.AddRange(leftEdgeIndices);

        var mesh = World3DUtils.CreateMesh(vertices.ToArray(), indices.ToArray());

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
