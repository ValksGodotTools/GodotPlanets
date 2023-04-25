namespace Planets;

public partial class Planet : Node3D
{
    private void GenerateEdgePoints(Vector3 posA, Vector3 posB, int subdivisions)
    {
        World3DUtils.CreateSphere(this, posA, Colors.Orange);
        World3DUtils.CreateSphere(this, posB, Colors.Orange);

        GenerateEdgeMidPoints(posA, posB, subdivisions);
    }

    private void GenerateEdgeMidPoints(Vector3 posA, Vector3 posB, int subdivisions)
    {
        var numPoints = Mathf.Max(0, (int)Mathf.Pow(2, subdivisions) - 1) + 1;

        for (int i = 1; i < numPoints; i++)
        {
            var t = (float)(i) / (numPoints);
            var pos = posA.Lerp(posB, t);

            World3DUtils.CreateSphere(this, pos, Colors.GreenYellow);
        }
    }

    public override void _Ready()
    {
        GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;

        var icosahedron = new Icosahedron();

        var pos1 = icosahedron.Vertices[icosahedron.Triangles[0]];
        var pos2 = icosahedron.Vertices[icosahedron.Triangles[1]];
        var pos3 = icosahedron.Vertices[icosahedron.Triangles[2]];

        GenerateEdgePoints(pos1, pos2, 3);

        AddChild(new MeshInstance3D
        {
            Mesh = World3DUtils.CreateMesh(icosahedron.Vertices, icosahedron.Triangles),
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = Colors.White,
                MetallicSpecular = 1.0f
            }
        });
    }
}

public static class World3DUtils
{
    public static void CreateSphere(Node parent, Vector3 pos, Color? color = null, float radius = 0.05f, int rings = 32)
    {
        var meshInstance = new MeshInstance3D
        {
            Mesh = new SphereMesh
            {
                Radius = radius,
                Height = radius * 2,
                Rings = rings,
                Material = new StandardMaterial3D
                {
                    AlbedoColor = color ?? Colors.White
                }
            },
            Position = pos
        };

        parent.AddChild(meshInstance);
    }

    public static Mesh CreateMesh(Vector3[] vertices, int[] indices)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;

        var normals = new Vector3[vertices.Length];

        for (int i = 0; i < normals.Length; i++)
            normals[i] = vertices[i].Normalized();

        arrays[(int)Mesh.ArrayType.Normal] = normals;
        //arrays[(int)Mesh.ArrayType.TexUv] = uvs;
        //arrays[(int)Mesh.ArrayType.Color] = colors;
        arrays[(int)Mesh.ArrayType.Index] = indices;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }
}

public class Icosahedron
{
    public Vector3[] Vertices  { get; }
    public int[]     Triangles { get; }

    public Icosahedron(float radius = 1)
    {
        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        Vertices = new Vector3[]
        {
            new Vector3(-1,  t,  0).Normalized() * radius,
            new Vector3( 1,  t,  0).Normalized() * radius,
            new Vector3(-1, -t,  0).Normalized() * radius,
            new Vector3( 1, -t,  0).Normalized() * radius,
            new Vector3( 0, -1,  t).Normalized() * radius,
            new Vector3( 0,  1,  t).Normalized() * radius,
            new Vector3( 0, -1, -t).Normalized() * radius,
            new Vector3( 0,  1, -t).Normalized() * radius,
            new Vector3( t,  0, -1).Normalized() * radius,
            new Vector3( t,  0,  1).Normalized() * radius,
            new Vector3(-t,  0, -1).Normalized() * radius,
            new Vector3(-t,  0,  1).Normalized() * radius
        };

        Triangles = new int[] {
            0, 5, 11,
            0, 1, 5,
            0, 7, 1,
            0, 10, 7,
            0, 11, 10,
            1, 9, 5,
            5, 4, 11,
            11, 2, 10,
            10, 6, 7,
            7, 8, 1,
            3, 4, 9,
            3, 2, 4,
            3, 6, 2,
            3, 8, 6,
            3, 9, 8,
            4, 5, 9,
            2, 11, 4,
            6, 10, 2,
            8, 7, 6,
            9, 1, 8
        };
    }
}
