namespace Planets;

public partial class Planet : Node3D
{
    private void GenerateEdgePoints(Vector3 posA, Vector3 posB, int subdivisions)
    {
        // Generate first point
        new DebugSphere(this, posA);

        // Generate points between first and last
        GenerateEdgeMidPoints(posA, posB, subdivisions);

        // Generate last point
        new DebugSphere(this, posB);
    }

    private void GenerateEdgeMidPoints(Vector3 posA, Vector3 posB, int subdivisions)
    {
        var numPoints = Mathf.Max(0, (int)Mathf.Pow(2, subdivisions) - 1) + 1;

        for (int i = 1; i < numPoints; i++)
        {
            var t = (float)(i) / (numPoints);
            var pos = posA.Lerp(posB, t);

            new DebugSphere(this, pos);
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
        GenerateEdgePoints(pos1, pos3, 3);
        GenerateEdgePoints(pos2, pos3, 3);

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

public class DebugSphere : Sphere
{
    public DebugSphere(Node parent, Vector3 pos) : base(parent, pos)
    {
        SetRadius(0.05f);
        SetColor(Colors.Green);
        SetRings(16);
    }
}
