namespace Planets;

public class Chunk
{
    private Node Parent { get; set; }

    public Chunk(Node parent, Vector3 posA, Vector3 posB, Vector3 posC, int resolution)
    {
        Parent = parent;

        Vector3[] vertices = BuildVertices(posA, posB, posC, resolution);
        int[] indices = BuildIndices(resolution);

        GMesh gMesh = new(vertices, indices)
        {
            SimpleNormals = false,
            Colors = GenerateColors(vertices)
        };

        Parent.AddChild(new MeshInstance3D
        {
            Mesh = gMesh.Generate(),
            MaterialOverride = new StandardMaterial3D
            {
                VertexColorUseAsAlbedo = true
            }
        });
    }

    private Color[] GenerateColors(Vector3[] vertices)
    {
        FastNoiseLite noise = new()
        {
            Frequency = 0.2f
        };

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            float n = 1 + (noise.GetNoise3Dv(vertices[i]) * 10);

            if (vertices[i].Length() > 9.5f)
                colors[i] = new Color("316231"); // grass
            else
                colors[i] = new Color(1, 1, 0.73f); // sand
        }

        return colors;
    }

    private Vector3[] BuildVertices(Vector3 posA, Vector3 posB, Vector3 posC, int res)
    {
        List<Vector3> vertices = new()
        {
            // The 3 main corners
            posA,
            posB,
            posC
        };

        // The edge midpoints
        if (res >= 1)
        {
            Vector3[] edgeMidpointsLeft = GenerateEdgeMidPoints(posA, posC, res);
            Vector3[] edgeMidpointsRight = GenerateEdgeMidPoints(posA, posB, res);

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
        FastNoiseLite noise = new()
        {
            Frequency = 0.003f
        };

        int noiseStrength = 1000;
        int planetRadius = 10;

        for (int i = 0; i < vertices.Count; i++)
        {
            float n = noise.GetNoise3Dv(vertices[i] * noiseStrength);

            vertices[i] = vertices[i].Normalized() * (planetRadius + n);
        }

        return vertices;
    }

    private int[] BuildIndices(int res)
    {
        List<int> indices = new();

        // If the resolution is 0 then there are no midpoints and thus there
        // is only 1 triangle
        if (res <= 0)
        {
            indices.AddRange(new int[] { 0, 1, 2 });
            return indices.ToArray();
        }

        // Index Legend
        // The 3 main corners
        int topMiddle = 0;
        int bottomRight = 1;
        int bottomLeft = 2;

        // The first point in each midpoint array
        int leftFirst = 3; // top left
        int rightFirst = 3 + res; // top right
        int bottomFirst = 3 + (res * 2); // bottom right

        // The center points
        int center = 3 + (res * 3);
        var centerBottomRight = center + GUMath.SumNaturalNumbers(res) - 1;
        var centerBottomLeft = centerBottomRight - res + 2;

        // The last point in each midpoint array
        int leftLast = rightFirst - 1;
        int rightLast = bottomFirst - 1;
        int bottomLast = 3 + (res * 3) - 1;

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
            List<int> triOuterEdgeRight = IndicesOuterEdgeRight(res, rightFirst, center);
            List<int> triOuterEdgeLeft  = IndicesOuterEdgeLeft(res, leftFirst, center);
            List<int> triOuterBottom    = IndicesOuterBottom(res, bottomFirst, centerBottomRight);
            
            indices.AddRange(triOuterEdgeRight);
            indices.AddRange(triOuterEdgeLeft);
            indices.AddRange(triOuterBottom);
        }

        if (res >= 3)
        {
            // Draw the center triangles
            List<int> triCenter = IndicesCenter(res, center);

            indices.AddRange(triCenter);
        }

        return indices.ToArray();
    }

    private List<int> IndicesOuterEdgeRight(int res, int rightFirst, int center)
    {
        List<int> indices = new();

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
        List<int> indices = new();

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
        List<int> indices = new();

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
        List<int> indices = new();

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
        Vector3[] centerPoints = new Vector3[GUMath.SumNaturalNumbers(resolution)];

        int index = 0;

        for (int row = 1; row < resolution; row++)
        {
            for (int i = 0; i < row; i++)
            {
                float t = (float)(i + 1) / (row + 1);

                Vector3 pos = edgeMidpointsLeft[row].Lerp(edgeMidpointsRight[row], t);

                centerPoints[index++] = pos;
            }
        }

        return centerPoints;
    }

    private Vector3[] GenerateEdgeMidPoints(Vector3 posA, Vector3 posB, int resolution)
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            // Calculate mid points
            float t = (i + 1f) / (resolution + 1f);
            Vector3 pos = posA.Lerp(posB, t);

            points[i] = pos;
        }

        return points;
    }
}
