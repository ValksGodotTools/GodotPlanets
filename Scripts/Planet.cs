namespace Planets;

public partial class Planet : Node3D
{
    public override void _Ready()
    {
        //GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;

        var icosahedron = new Icosahedron();
        var vertices = icosahedron.Vertices;
        var indices = icosahedron.Triangles;

        var resolution = 1;

        for (int i = 0; i < 3; i += 3)
        {
            var posA = vertices[indices[i]];
            var posB = vertices[indices[i + 1]];
            var posC = vertices[indices[i + 2]];

            new ChunkV2(this, posA, posB, posC, resolution);
        }
    }
}

public class DebugPoint : Sphere
{
    public DebugPoint(Node parent, Vector3 pos) : base(parent, pos)
    {
        SetRadius(0.03f);
        SetColor(Colors.Green);
        SetRings(8);
        SetRadialSegments(16);
    }
}
