namespace Planets;

public static class Sounds
{
    private static AudioStream Load(string path) =>
        GD.Load<AudioStream>($"res://Audio/SFX/{path}");
}
