namespace Planets;

public class GMesh
{
    public bool SimpleNormals { get; set; }
    public Color[] Colors { get; set; }

    private Vector3[] vertices;
    private int[] indices;

    public GMesh(Vector3[] vertices, int[] indices)
    {
        this.vertices = vertices;
        this.indices = indices;
    }

    public Mesh Generate() => SimpleNormals ? 
        GenerateWithSimpleNormals() : GenerateWithComplexNormals();

    private Mesh GenerateWithSimpleNormals()
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;

        var normals = new Vector3[vertices.Length];

        for (int i = 0; i < normals.Length; i++)
            normals[i] = vertices[i].Normalized();

        arrays[(int)Mesh.ArrayType.Normal] = normals;
        //arrays[(int)Mesh.ArrayType.TexUv] = uvs;

        if (Colors != null)
            arrays[(int)Mesh.ArrayType.Color] = Colors;

        arrays[(int)Mesh.ArrayType.Index] = indices;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }

    private Mesh GenerateWithComplexNormals()
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Colors != null)
                st.SetColor(Colors[i]);

            st.AddVertex(vertices[i]);
        }

        foreach (var index in indices)
            st.AddIndex(index);

        st.GenerateNormals();

        return st.Commit();
    }
}
