namespace Planets;

public partial class Planet : Node3D
{
    public override void _Ready()
    {
        //GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;

        var icosahedron = new Icosahedron();
        var vertices = icosahedron.Vertices;
        var indices = icosahedron.Triangles;

        var subdivisions = 20;

        for (int i = 0; i < 3; i += 3)
        {
            var posA = vertices[indices[i]];
            var posB = vertices[indices[i + 1]];
            var posC = vertices[indices[i + 2]];

            new Chunk(this, posA, posB, posC, subdivisions);
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
