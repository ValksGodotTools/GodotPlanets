using System;

namespace Planets;

public partial class CameraController : Node3D
{
    private float    Sensitivity      { get; } = 0.005f;

    private Camera3D Camera           { get; set; }
    private bool     HoldingLeftClick { get; set; }
    private Node3D   Orbit            { get; set; }

    public override void _Ready()
    {
        Orbit = GetNode<Node3D>("Orbit");
        Camera = Orbit.GetNode<Camera3D>("Camera");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton button)
        {
            HoldingLeftClick = button.ButtonIndex == MouseButton.Left && button.Pressed;

            if (button.IsZoomIn())
            {
                Camera.Position -= new Vector3(0, 0, 0.1f);
            }

            if (button.IsZoomOut())
            {
                Camera.Position += new Vector3(0, 0, 0.1f);
            }
        }

        if (@event is InputEventMouseMotion motion)
            Rotate(motion);
    }

    private void Rotate(InputEventMouseMotion motion)
    {
        if (!HoldingLeftClick)
            return;

        var vel = motion.Relative * Sensitivity;

        var rot = Rotation;

        rot.X -= vel.Y;
        rot.Y -= vel.X;

        // If the rot.X is not clamped then the rotation gets too chaotic
        rot.X = Mathf.Clamp(rot.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f);
        Rotation = rot;
    }
}
