namespace Planets;

public static class Music
{
    private static AudioStream Load(string path) =>
        GD.Load<AudioStream>($"res://Audio/Songs/{path}");
}
