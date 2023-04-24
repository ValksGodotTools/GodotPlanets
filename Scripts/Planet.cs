namespace Planets;

public partial class Planet : Node3D
{
    public override void _Ready()
    {
        GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;

        var icosahedron = new Icosahedron();

        AddChild(new MeshInstance3D
        {
            Mesh = World3DUtils.GenerateMesh(icosahedron.Vertices, icosahedron.Triangles),
            MaterialOverride = GD.Load<Material>("res://Scripts/3D FPS/material.tres")
        });
    }
}

public static class World3DUtils
{
    public static Mesh GenerateMesh(Vector3[] vertices, int[] indices)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        //arrays[(int)Mesh.ArrayType.Normal] = normals;
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
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };
    }
}
