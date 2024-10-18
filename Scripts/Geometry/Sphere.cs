using Godot;

namespace Planets;

public class Sphere
{
    protected Vector3 _position;

    private readonly SphereMesh _mesh;

    public Sphere(Node parent, Vector3 pos)
    {
        _position = pos;
        _mesh = new();
        
        parent.AddChild(new MeshInstance3D
        {
            Mesh = _mesh,
            Position = pos
        });
    }

    public Sphere SetColor(Color color)
    {
        _mesh.Material = new StandardMaterial3D
        {
            AlbedoColor = color
        };

        return this;
    }

    public Sphere SetRadius(float radius)
    {
        _mesh.Radius = radius;
        _mesh.Height = radius * 2;

        return this;
    }

    public Sphere SetRings(int rings)
    {
        _mesh.Rings = rings;

        return this;
    }

    public Sphere SetRadialSegments(int segments)
    {
        _mesh.RadialSegments = Mathf.Max(4, segments);

        return this;
    }
}
