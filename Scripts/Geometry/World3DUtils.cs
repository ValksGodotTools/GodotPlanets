namespace Planets;

public class GMesh
{
    public bool SimpleNormals { get; set; }
    public Color[] Colors { get; set; }

    private readonly Vector3[] vertices;
    private readonly int[] indices;

    public GMesh(Vector3[] vertices, int[] indices)
    {
        this.vertices = vertices;
        this.indices = indices;
    }

    public Mesh Generate() => SimpleNormals ? 
        GenerateWithSimpleNormals() : GenerateWithComplexNormals();

    private Mesh GenerateWithSimpleNormals()
    {
        Godot.Collections.Array arrays = new();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;

        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < normals.Length; i++)
            normals[i] = vertices[i].Normalized();

        arrays[(int)Mesh.ArrayType.Normal] = normals;
        //arrays[(int)Mesh.ArrayType.TexUv] = uvs;

        if (Colors != null)
            arrays[(int)Mesh.ArrayType.Color] = Colors;

        arrays[(int)Mesh.ArrayType.Index] = indices;

        ArrayMesh mesh = new();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }

    private Mesh GenerateWithComplexNormals()
    {
        SurfaceTool st = new();
        st.Begin(Mesh.PrimitiveType.Triangles);

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Colors != null)
                st.SetColor(Colors[i]);

            st.AddVertex(vertices[i]);
        }

        foreach (int index in indices)
            st.AddIndex(index);

        st.GenerateNormals();

        return st.Commit();
    }
}
