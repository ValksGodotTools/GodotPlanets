namespace Planets;

public partial class Planet : Node3D
{
    public override void _Ready()
    {
        //GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;

        Icosahedron icosahedron = new();
        Vector3[] vertices = icosahedron.Vertices;
        int[] indices = icosahedron.Triangles;

        int resolution = 128;

        Logger.LogMs(() =>
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 posA = vertices[indices[i]];
                Vector3 posB = vertices[indices[i + 1]];
                Vector3 posC = vertices[indices[i + 2]];

                new Chunk(this, posA, posB, posC, resolution);
            }
        });
    }
}
