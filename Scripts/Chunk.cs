namespace Planets;

public class Chunk
{
    private Node Parent { get; set; }

    public Chunk(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        Parent = parent;

        var vertices = BuildVertices(posA, posB, posC, resolution);
        var indices = BuildIndices(resolution);

        var mesh = World3DUtils.CreateMesh(vertices, indices, false);

        Parent.AddChild(new MeshInstance3D
        {
            Mesh = mesh
        });
    }

    private Vector3[] BuildVertices(Vector3 posA, Vector3 posB, Vector3 posC, int res)
    {
        var vertices = new List<Vector3>();

        // The 3 main corners
        vertices.Add(posA);
        vertices.Add(posB);
        vertices.Add(posC);

        // The edge midpoints
        if (res >= 1)
        {
            var edgeMidpointsLeft = GenerateEdgeMidPoints(posA, posC, res);
            var edgeMidpointsRight = GenerateEdgeMidPoints(posA, posB, res);

            vertices.AddRange(edgeMidpointsLeft); // left
            vertices.AddRange(edgeMidpointsRight); // right
            vertices.AddRange(GenerateEdgeMidPoints(posB, posC, res)); // bottom

            // The center points
            if (res >= 2)
                vertices.AddRange(GenerateCenterPoints(edgeMidpointsLeft, edgeMidpointsRight, res));
        }

        vertices = DeformVertices(vertices);

        return vertices.ToArray();
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
