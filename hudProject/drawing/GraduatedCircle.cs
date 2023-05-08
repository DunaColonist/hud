using Shapes;
using UnityEngine;

namespace Hud.Drawing;

// XX can not draw a partial Torus only a partial Disc
// but Disc is limited too because it is 2D form : so it has no 3D thickness...
internal class GraduatedCircle
{
    internal class Thickness
    {
        public static readonly long Circle = 5;

        public static readonly long BigTick = 5;
        public static readonly long MediumTick = 3;
        public static readonly long SmallTick = 1;
    }

    internal class Length
    {
        public static readonly float BigTick = 0.1f;
        public static readonly float MediumTick = 0.05f;
        public static readonly float SmallTick = 0.02f;
    }

    private readonly Vector3 position;
    private readonly float radius;

    public GraduatedCircle(Vector3 position, Vector3 up, Vector3 right, float radius, Color[] quarterColors, Color[] tickColors, bool invertTick)
    {
        this.position = position;
        this.radius = radius;

        DrawQuarter(up, right, quarterColors[0], tickColors[0], invertTick);
        DrawQuarter(-right, up, quarterColors[1], tickColors[1], invertTick);
        DrawQuarter(-up, -right, quarterColors[2], tickColors[2], invertTick);
        DrawQuarter(right, -up, quarterColors[3], tickColors[3], invertTick);
    }

    private void DrawQuarter(Vector3 vertical, Vector3 horizontal, Color color, Color bigTickColor, bool invertTick)
    {
        var steps = 18;

        for (int i = 0; i < steps; i++)
        {
            var stepRad = Mathf.PI / (steps * 2);

            var currentAngle = i * stepRad;
            var nextAngle = (i + 1) * stepRad;

            var currentRadius = Mathf.Cos(currentAngle) * radius;
            var nextRadius = Mathf.Cos(nextAngle) * radius;

            var currentOffset = Mathf.Sin(currentAngle) * radius;
            var nextOffset = Mathf.Sin(nextAngle) * radius;

            var start = position + (horizontal * currentRadius) + (vertical * currentOffset);
            var end = position + (horizontal * nextRadius) + (vertical * nextOffset);
            Draw.Line(start, end, Thickness.Circle, color);

            var radial = (start - position).normalized;

            var tickDirection = invertTick ? -1 : 1;
            if (i == 0)
            {
                var localOffset = radial * radius * Length.BigTick * tickDirection;
                Draw.Line(start, start - localOffset, Thickness.BigTick, bigTickColor);
            }
            else if (i == steps / 2)
            {
                var localOffset = radial * radius * Length.MediumTick * tickDirection;
                Draw.Line(start, start - localOffset, Thickness.MediumTick, color);
            }
            else if (i % 2 == 0)
            {
                var localOffset = radial * radius * Length.SmallTick * tickDirection;
                Draw.Line(start, start - localOffset, Thickness.SmallTick, color);
            }
        }
    }
}
