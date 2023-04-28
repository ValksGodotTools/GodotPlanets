﻿namespace Planets;

public static class World3DUtils
{
    public static Mesh CreateMesh(Vector3[] vertices, int[] indices, bool simpleNormals)
    {
        if (simpleNormals)
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
        else
        {
            var st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);

            foreach (var vertex in vertices)
                st.AddVertex(vertex);

            foreach (var index in indices)
                st.AddIndex(index);

            st.GenerateNormals();

            return st.Commit();
        }
    }
}
