using Godot;

namespace Planets;

public class Icosahedron
{
    public Vector3[] Vertices { get; }
    public int[] Triangles { get; }

    public Icosahedron(float radius = 1)
    {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        Vertices =
        [
            new Vector3(-1,  t,  0).Normalized() * radius,
            new Vector3( 1,  t,  0).Normalized() * radius,
            new Vector3(-1, -t,  0).Normalized() * radius,
            new Vector3( 1, -t,  0).Normalized() * radius,
            new Vector3( 0, -1,  t).Normalized() * radius,
            new Vector3( 0,  1,  t).Normalized() * radius,
            new Vector3( 0, -1, -t).Normalized() * radius,
            new Vector3( 0,  1, -t).Normalized() * radius,
            new Vector3( t,  0, -1).Normalized() * radius,
            new Vector3( t,  0,  1).Normalized() * radius,
            new Vector3(-t,  0, -1).Normalized() * radius,
            new Vector3(-t,  0,  1).Normalized() * radius
        ];

        Triangles = [
            0, 5, 11,
            0, 1, 5,
            0, 7, 1,
            0, 10, 7,
            0, 11, 10,
            1, 9, 5,
            5, 4, 11,
            11, 2, 10,
            10, 6, 7,
            7, 8, 1,
            3, 4, 9,
            3, 2, 4,
            3, 6, 2,
            3, 8, 6,
            3, 9, 8,
            4, 5, 9,
            2, 11, 4,
            6, 10, 2,
            8, 7, 6,
            9, 1, 8
        ];
    }
}
