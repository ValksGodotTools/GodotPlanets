namespace Planets;

public class Sphere
{
    private SphereMesh Mesh { get; }

    public Sphere(Node parent, Vector3 pos)
    {
        Mesh = new();
        parent.AddChild(new MeshInstance3D
        {
            Mesh = Mesh,
            Position = pos
        });
    }

    public Sphere SetColor(Color color)
    {
        Mesh.Material = new StandardMaterial3D
        {
            AlbedoColor = color
        };

        return this;
    }

    public Sphere SetRadius(float radius)
    {
        Mesh.Radius = radius;
        Mesh.Height = radius * 2;

        return this;
    }

    public Sphere SetRings(int rings)
    {
        Mesh.Rings = rings;

        return this;
    }

    public Sphere SetRadialSegments(int segments)
    {
        Mesh.RadialSegments = Mathf.Max(4, segments);

        return this;
    }
}
