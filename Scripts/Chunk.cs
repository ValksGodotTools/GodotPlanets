using System.Linq;

namespace Planets;

public class ChunkV2
{
    private Node Parent { get; set; }

    public ChunkV2(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        Parent = parent;
        
        Vertices = BuildVertices(posA, posB, posC, resolution);
        var indices = BuildIndices(resolution);

        var mesh = World3DUtils.CreateMesh(Vertices, indices);

        Parent.AddChild(new MeshInstance3D
        {
            Mesh = mesh
        });
    }

    private Vector3[] Vertices { get; set; }
    private Color[] ColorArr { get; set; }

    private int[] BuildIndices(int res)
    {
        var indices = new List<int>();

        // If the resolution is 0 then there are no midpoints and thus there
        // is only 1 triangle
        if (res <= 0)
        {
            indices.AddRange(new int[] { 0, 1, 2 });
            return indices.ToArray();
        }

        // Index Legend
        // The 3 main corners
        var topMiddle = 0;
        var bottomRight = 1;
        var bottomLeft = 2;

        // The first point in each midpoint array
        var leftFirst = 3; // top left
        var rightFirst = 3 + res; // top right
        var bottomFirst = 3 + res * 2; // bottom right

        // The center points
        var center = 3 + res * 3;
        var centerBottomRight = center + GUMath.SumNaturalNumbers(res) - 1;
        var centerBottomLeft = centerBottomRight - res + 2;

        // The last point in each midpoint array
        var leftLast = rightFirst - 1;
        var rightLast = bottomFirst - 1;
        var bottomLast = 3 + res * 3 - 1;

        // Resolution can be greater than or equal to 1 here
        // Draw the corner triangles
        indices.AddRange(new int[] { leftFirst, topMiddle, rightFirst });
        indices.AddRange(new int[] { bottomLeft, leftLast, bottomLast });
        indices.AddRange(new int[] { bottomFirst, rightLast, bottomRight });

        // If resolution equals 1 then draw the special case triangle in center
        if (res == 1)
            indices.AddRange(new int[] { leftFirst, rightFirst, bottomFirst });

        if (res >= 2)
        {
            // Draw the special triangles near each of the 3 main corners
            indices.AddRange(new int[] { center, leftFirst, rightFirst });
            indices.AddRange(new int[] { centerBottomLeft, bottomLast, leftLast });
            indices.AddRange(new int[] { centerBottomRight, rightLast, bottomFirst });

            // Draw the outer edge triangles
            var triOuterEdgeRight = IndicesOuterEdgeRight(res, rightFirst, center);
            var triOuterEdgeLeft  = IndicesOuterEdgeLeft(res, leftFirst, center);
            var triOuterBottom    = IndicesOuterBottom(res, bottomFirst, centerBottomRight);
            
            indices.AddRange(triOuterEdgeRight);
            indices.AddRange(triOuterEdgeLeft);
            indices.AddRange(triOuterBottom);
        }

        if (res >= 3)
        {
            // Draw the center triangles
            var triCenter = IndicesCenter(res, center);

            indices.AddRange(triCenter);
        }

        return indices.ToArray();
    }

    private List<int> IndicesOuterEdgeRight(int res, int rightFirst, int center)
    {
        var indices = new List<int>();

        // Outer Right Edge Triangles
        // Upside
        for (int i = 0; i < res - 1; i++)
        {
            indices.AddRange(new int[]
            {
                rightFirst + i,
                rightFirst + i + 1,
                center + GUMath.SumNaturalNumbers(i + 2) - 1
            });
        }

        // Flipside
        for (int i = 0; i < res - 2; i++)
        {
            indices.AddRange(new int[]
            {
                center + GUMath.SumNaturalNumbers(i + 2) - 1,
                rightFirst + i + 1,
                center + GUMath.SumNaturalNumbers(i + 3) - 1
            });
        }

        return indices;
    }

    private List<int> IndicesOuterEdgeLeft(int res, int leftFirst, int center)
    {
        var indices = new List<int>();

        // Outer Left Edge Triangles
        // Upside
        for (int i = 0; i < res - 1; i++)
        {
            indices.AddRange(new int[]
            {
                leftFirst + i,
                center + GUMath.SumNaturalNumbers(i + 2) - i - 1,
                leftFirst + i + 1
            });
        }

        // Flipside
        for (int i = 0; i < res - 2; i++)
        {
            indices.AddRange(new int[]
            {
                leftFirst + i + 1,
                center + GUMath.SumNaturalNumbers(i + 2) - i - 1,
                center + GUMath.SumNaturalNumbers(i + 3) - i - 2
            });
        }

        return indices;
    }

    private List<int> IndicesOuterBottom(int res, int bottomFirst, int centerBottomRight)
    {
        var indices = new List<int>();

        // Outer Bottom Edge Triangles
        // Upside
        for (int i = 0; i < res - 1; i++)
        {
            indices.AddRange(new int[]
            {
                bottomFirst + i + 1,
                centerBottomRight - i,
                bottomFirst + i
            });
        }

        // Flipside
        for (int i = 0; i < res - 2; i++)
        {
            indices.AddRange(new int[]
            {
                centerBottomRight - i - 1,
                centerBottomRight - i,
                bottomFirst + i + 1
            });
        }

        return indices;
    }

    private List<int> IndicesCenter(int res, int center)
    {
        var indices = new List<int>();

        // Center Triangles
        // Upside
        for (int row = 1; row < res - 1; row++)
        {
            for (int i = 0; i < row; i++)
            {
                var row1 = GUMath.SumNaturalNumbers(row) + i;
                var row2 = GUMath.SumNaturalNumbers(row + 1) + i;

                var x = row2;
                var y = row1;
                var z = row2 + 1;

                indices.AddRange(new int[] {
                    center + x, center + y, center + z
                });
            }
        }

        // Flipside
        for (int row = 1; row < res - 2; row++)
        {
            for (int i = 0; i < row; i++)
            {
                var row1 = GUMath.SumNaturalNumbers(row + 1) + i;
                var row2 = GUMath.SumNaturalNumbers(row + 2) + i;

                var x = row1;
                var y = row1 + 1;
                var z = row2 + 1;

                indices.AddRange(new int[] {
                    center + x, center + y, center + z
                });
            }
        }

        return indices;
    }

    private Vector3[] BuildVertices(Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        var vertices = new List<Vector3>();

        var edgeMidpointsLeft = GenerateEdgeMidPoints(posA, posC, resolution);
        var edgeMidpointsRight = GenerateEdgeMidPoints(posA, posB, resolution);

        vertices.Add(posA);
        vertices.Add(posB);
        vertices.Add(posC);
        vertices.AddRange(edgeMidpointsLeft); // left
        vertices.AddRange(edgeMidpointsRight); // right

        if (resolution > 0)
            vertices.AddRange(GenerateEdgeMidPoints(posB, posC, resolution)); // bottom

        if (resolution > 1)
            vertices.AddRange(GenerateCenterPoints(edgeMidpointsLeft, edgeMidpointsRight, resolution));

        return vertices.ToArray();
    }

    private Vector3[] GenerateCenterPoints(Vector3[] edgeMidpointsLeft, Vector3[] edgeMidpointsRight, int resolution)
    {
        var centerPoints = new Vector3[GUMath.SumNaturalNumbers(resolution)];
        
        var index = 0;

        for (int row = 1; row < resolution; row++)
        {
            for (int i = 0; i < row; i++)
            {
                var t = (float)(i + 1) / (row + 1);

                var pos = edgeMidpointsLeft[row].Lerp(edgeMidpointsRight[row], t);

                centerPoints[index++] = pos;
            }
        }

        return centerPoints;
    }

    private Vector3[] GenerateEdgeMidPoints(Vector3 posA, Vector3 posB, int resolution)
    {
        var points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            // Calculate mid points
            var t = (i + 1f) / (resolution + 1f);
            var pos = posA.Lerp(posB, t);

            points[i] = pos;
        }

        return points;
    }
}

public class Chunk
{
    public Vector3[] Vertices { get; private set; }

    private Dictionary<int, Vector3[]> Edges { get; } = new();
    private Vector3[] CenterPoints { get; set; }

    private int NumMidPoints { get; set; }
    private int NumEdgePoints { get; set; }

    private int Resolution { get; set; }
    private int NumTriangles { get; set; }

    private Node Parent { get; set; }

    public Chunk(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        Logger.LogMs(() =>
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
        });
    }

    private void GenerateMesh()
    {
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
        var planetRadius = 10;

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
