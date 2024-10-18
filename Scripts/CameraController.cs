using Godot;
using GodotUtils;

namespace Planets;

public partial class CameraController : Node3D
{
    private Camera3D _camera;
    private Node3D _orbit;
    private float _sensitivity = 0.005f;
    private bool _holdingLeftClick;

    public override void _Ready()
    {
        _orbit = GetNode<Node3D>("Orbit");
        _camera = _orbit.GetNode<Camera3D>("Camera");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton button)
        {
            _holdingLeftClick = button.IsLeftClickPressed();

            if (button.IsZoomIn())
            {
                _camera.Position -= new Vector3(0, 0, 0.1f);
            }

            if (button.IsZoomOut())
            {
                _camera.Position += new Vector3(0, 0, 0.1f);
            }
        }

        if (@event is InputEventMouseMotion motion)
        {
            Rotate(motion);
        }
    }

    private void Rotate(InputEventMouseMotion motion)
    {
        if (!_holdingLeftClick)
        {
            return;
        }

        Vector2 vel = motion.Relative * _sensitivity;
        Vector3 rot = Rotation;

        rot.X -= vel.Y;
        rot.Y -= vel.X;
        rot.X = Mathf.Clamp(rot.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f); // If the rot.X is not clamped then the rotation will become too chaotic

        Rotation = rot;
    }
}
