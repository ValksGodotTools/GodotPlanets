using Godot;

namespace Planets;

public class DebugPoint : Sphere
{
    public DebugPoint(Node parent, Vector3 pos, string text) : base(parent, pos)
    {
        SetRadius(0.03f);
        SetColor(Colors.Green);
        SetRings(8);
        SetRadialSegments(16);

        parent.AddChild(new Label3D
        {
            Text = text,
            Position = _position + new Vector3(0, 0.05f, 0),
            FontSize = 12
        });
    }
}
