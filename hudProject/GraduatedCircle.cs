using Shapes;
using UnityEngine;

namespace hud
{
    // XX can not draw a partial Torus only a partial Disc
    // but Disc is limited too because it is 2D form : so it has no 3D thickness...
    internal class GraduatedCircle
    {
        internal class Thickness
        {
            public static readonly long circle = 5;

            public static readonly long bigTick = 5;
            public static readonly long mediumTick = 3;
            public static readonly long smallTick = 1;
        }

        internal class Length
        {
            public static readonly float bigTick = 0.1f;
            public static readonly float mediumTick = 0.05f;
            public static readonly float smallTick = 0.02f;
        }

        private readonly Vector3 position;
        private readonly float radius;

        public GraduatedCircle(Vector3 position, Vector3 up, Vector3 right, float radius, Color[] quarterColors, Color[] tickColors,  bool invertTick)
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

                var start = position + horizontal * currentRadius + vertical * currentOffset;
                var end = position + horizontal * nextRadius + vertical * nextOffset;
                Draw.Line(start, end, Thickness.circle, color);

                var radial = (start - position).normalized;

                var tickDirection = invertTick ? -1 : 1;
                if (i == 0)
                {
                    Draw.Line(start, start - radial * radius * Length.bigTick * tickDirection, Thickness.bigTick, bigTickColor);
                } else if (i == steps / 2)
                {
                    Draw.Line(start, start - radial * radius * Length.mediumTick * tickDirection, Thickness.mediumTick, color);
                }
                else if (i % 2 == 0)
                {
                    Draw.Line(start, start - radial * radius * Length.smallTick * tickDirection, Thickness.smallTick, color);
                }
            }
        }
    }
}
