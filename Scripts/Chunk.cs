namespace Planets;

public class Chunk
{
    public Vector3[] Vertices { get; private set; }

    private Dictionary<int, Vector3[]> Edges { get; } = new();
    private Vector3[] CenterPoints { get; }

    private int NumMidPoints { get; }
    private int NumEdgePoints { get; }

    private int Resolution { get; }
    private int NumTriangles { get; }

    private Node Parent { get; }

    public Chunk(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        Parent = parent;

        // 0 subdivisions is not supported because I'm bad at coding
        Resolution = Mathf.Max(2, resolution + 1);

        NumTriangles = Resolution * Resolution;

        NumEdgePoints = Resolution + 1;
        NumMidPoints = NumEdgePoints - 2;

        for (int i = 0; i < Edges.Count; i++)
            Edges[i] = new Vector3[NumEdgePoints];

        Edges[0] = GenerateEdgePoints(posA, posB);
        Edges[1] = GenerateEdgePoints(posA, posC);
        Edges[2] = GenerateEdgePoints(posB, posC);

        CenterPoints = GenerateCenterPoints();

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        var rightEdgeIndices = new List<int>();

        Vertices = BuildVertices();
        var indices = BuildIndices();

        var mesh = World3DUtils.CreateMesh(Vertices, indices);

        Parent.AddChild(new MeshInstance3D
        {
            Mesh = mesh
        });
    }

    private List<Vector3> DeformVertices(List<Vector3> vertices)
    {
        var noise = new FastNoiseLite
        {
            Frequency = 0.003f
        };

        var noiseStrength = 1000;
        var planetRadius = 12;

        for (int i = 0; i < vertices.Count; i++)
        {
            var n = noise.GetNoise3Dv(vertices[i] * noiseStrength);

            vertices[i] = vertices[i].Normalized() * (planetRadius + n);
        }

        return vertices;
    }

    private Vector3[] BuildVertices()
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

        // Deform the vertices
        vertices = DeformVertices(vertices);

        return vertices.ToArray();
    }

    private int[] BuildIndices()
    {
        var r = 3; // right edge index
        var l = 3 + NumMidPoints; // left edge index
        var b = 3 + NumMidPoints * 2; // bottom edge index
        var c = 3 + NumMidPoints * 3; // center points index

        var indices = new List<int>();

        if (Resolution == 0)
        {
            indices.AddRange(new int[] { 0, 2, 1 });
        }
        else
        {
            // Variables were created here for easy debugging (to see the lengths 
            // of each index array)
            var mainCorners = BuildTrianglesMainCorners(l, r, b);
            var special = BuildTrianglesSpecial(l, r, b, c);
            var leftEdge = BuildTrianglesLeftEdge(l, c);
            var rightEdge = BuildTrianglesRightEdge(r, c);
            var bottomEdge = BuildTrianglesBottomEdge(b, c);
            var centerUpside = BuildTrianglesCenterUpside(c);
            var centerFlipside = BuildTrianglesCenterFlipside(c);

            indices.AddRange(mainCorners);
            indices.AddRange(special);
            indices.AddRange(leftEdge);
            indices.AddRange(rightEdge);
            indices.AddRange(bottomEdge);

            indices.AddRange(centerUpside);

            if (Resolution > 4)
                indices.AddRange(centerFlipside);
        }

        return indices.ToArray();
    }

    private int[] BuildTrianglesCenterUpside(int c)
    {
        var indices = new List<int>();

        // Up-side Triangles
        indices.AddRange(new int[] {
            c + 1, c + 2, c + 0
        });

        var r = 0;
        var numCenterUpsideTriangles = (Resolution - 3) * (Resolution - 3) - GUMath.SumNaturalNumbers(Resolution - 3);

        for (int i = 1; i < numCenterUpsideTriangles; i++)
        {
            if (i >= GUMath.SumNaturalNumbers(3 + r))
                r++;

            indices.AddRange(new int[] {
                c + r + i + 2, c + r + i + 3, c + i
            });
        }

        return indices.ToArray();
    }

    private int[] BuildTrianglesCenterFlipside(int c)
    {
        var indices = new List<int>();

        // Flip-side Triangles
        indices.AddRange(new int[]
        {
            c + 4, c + 2, c + 1
        });

        var numCenterFlipsideTriangles = (Resolution - 4) * (Resolution - 4) - GUMath.SumNaturalNumbers(Resolution - 4);
        var x = 0;

        for (int i = 1; i < numCenterFlipsideTriangles; i++)
        {
            if (i >= GUMath.SumNaturalNumbers(3 + x))
                x++;

            indices.AddRange(new int[] {
                c + i + 6 + (x * 2), c + i + 3 + x, c + i + 2 + x
            });
        }

        return indices.ToArray();
    }

    private int[] BuildTrianglesBottomEdge(int b, int c)
    {
        var indices = new List<int>();

        var cValue = c + CenterPoints.Length - NumMidPoints;

        // Add right side triangles
        for (int i = 0; i < NumMidPoints - 1; i++)
            indices.AddRange(new int[] {
                b + i, 
                b + i + 1,
                cValue + i + 1
            });

        // Add flip side triangles
        for (int i = 0; i < NumMidPoints - 2; i++)
            indices.AddRange(new int[] {
                b + i + 1,
                cValue + i + 2,
                cValue + i + 1
            });

        return indices.ToArray();
    }

    private int[] BuildTrianglesRightEdge(int r, int c)
    {
        var indices = new List<int>();

        // Add right side triangles
        for (int i = 0; i < NumMidPoints - 1; i++)
        {
            indices.AddRange(new int[] {
                r + i,
                r + i + 1,
                c + GUMath.SumNaturalNumbers(i + 1)
            });
        }
        
        // Add flip side triangles
        for (int i = 0; i < NumMidPoints - 2; i++)
        {
            indices.AddRange(new int[]
            {
                c + GUMath.SumNaturalNumbers(i + 1),
                r + i + 1,
                c + GUMath.SumNaturalNumbers(i + 2)
            });
        }

        return indices.ToArray();
    }

    private int[] BuildTrianglesLeftEdge(int l, int c)
    {
        var indices = new List<int>();

        var cIndex = 0;

        // Add right side triangles
        for (int i = 0; i < NumMidPoints - 1; i++)
        {
            cIndex = i == 0 ? c : c + i + GUMath.SumNaturalNumbers(i + 1);

            indices.AddRange(new int[] {
                l + i, 
                cIndex, 
                l + i + 1
            });
        }

        // Add flip side triangles
        for (int i = 0; i < NumMidPoints - 2; i++)
        {
            if (i == 0)
            {
                indices.AddRange(new int[] {
                    c,
                    c + GUMath.SumNaturalNumbers(i + 3) - 1,
                    l + i + 1
                });
            }
            else
            {
                indices.AddRange(new int[] {
                    c + GUMath.SumNaturalNumbers(i + 2) - 1,
                    c + GUMath.SumNaturalNumbers(i + 3) - 1,
                    l + i + 1
                });
            }
        }

        return indices.ToArray();
    }

    private int[] BuildTrianglesMainCorners(int l, int r, int b)
    {
        var top = 0;
        var bottomLeft = 1;
        var bottomRight = 2;

        return new int[]
        {
            // The main 3 corners
            top        , r                   , l                   ,
            bottomLeft , l + NumMidPoints - 1, b + NumMidPoints - 1,
            bottomRight, b                   , r + NumMidPoints - 1,
        };
    }

    private int[] BuildTrianglesSpecial(int l, int r, int b, int c)
    {
        if (Resolution <= 2)
        {
            return new int[]
            {
                r, b, l
            };
        }
        else
        {
            return new int[]
            {
                // There is a 'special' triangle next to each corner
                r, c, l,
                b, c + CenterPoints.Length - NumMidPoints + 1, r + NumMidPoints - 1,
                l + NumMidPoints - 1, c + CenterPoints.Length - 1, b + NumMidPoints - 1
            };
        }
    }

    private Vector3[] GenerateCenterPoints()
    {
        // Take note of i such that i >= 2 and i <= n - 1, there will always
        // be i - 1 center points for each row. All up all the rows to get the
        // number of center points.
        var numCenterPoints = GUMath.SumNaturalNumbers(NumMidPoints);

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

    private Vector3[] GenerateEdgePoints(Vector3 posA, Vector3 posB)
    {
        var points = new Vector3[NumEdgePoints];

        // Generate first point
        points[0] = posA;

        // Generate last point
        points[NumEdgePoints - 1] = posB;

        // Generate points between first and last
        GenerateEdgeMidPoints(points);

        return points;
    }

    private void GenerateEdgeMidPoints(Vector3[] points)
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
