namespace Planets;

public partial class Planet : Node3D
{
    public override void _Ready()
    {
        //GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;

        var icosahedron = new Icosahedron();
        var vertices = icosahedron.Vertices;
        var indices = icosahedron.Triangles;

        var resolution = 128;

        Logger.LogMs(() =>
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                var posA = vertices[indices[i]];
                var posB = vertices[indices[i + 1]];
                var posC = vertices[indices[i + 2]];

                new Chunk(this, posA, posB, posC, resolution);
            }
        });
    }
}
