using Shapes;
using UnityEngine;

namespace Hud.Shapes;

// XX can not draw a partial Torus only a partial Disc
// but Disc is limited too because it is 2D form : so it has no 3D thickness...
internal class SphereShapes
{
    internal class Thickness
    {
        public readonly float Circle;
        public readonly float BigTick;
        public readonly float MediumTick;
        public readonly float SmallTick;

        public Thickness(float radius) 
        {
            Circle = radius / 100;
            BigTick = radius / 100;
            MediumTick = radius / 200;
            SmallTick = radius / 300;
        }
    }

    internal class Length
    {
        public readonly float BigTick;
        public readonly float MediumTick;
        public readonly float SmallTick;

        public Length(float radius) 
        {
            BigTick = radius / 40;
            MediumTick = radius / 80;
            SmallTick = radius / 160;
        }
    }

    private readonly Vector3 _position;
    private readonly float _radius;
    private readonly Thickness _thickness;
    private readonly Length _length;

    public SphereShapes(Vector3 position, float radius)
    {
        _position = position;
        _radius = radius;
        _thickness = new Thickness(_radius);
        _length = new Length(_radius);
    }

    public void DrawGraduatedTorus(Vector3 up, Vector3 right, Color[] quarterColors, Color[] tickColors, bool invertTick)
    {
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

            var currentRadius = Mathf.Cos(currentAngle) * _radius;
            var nextRadius = Mathf.Cos(nextAngle) * _radius;

            var currentOffset = Mathf.Sin(currentAngle) * _radius;
            var nextOffset = Mathf.Sin(nextAngle) * _radius;

            var start = _position + (horizontal * currentRadius) + (vertical * currentOffset);
            var end = _position + (horizontal * nextRadius) + (vertical * nextOffset);
            SpatialShapes.DrawLine(start, end, color, _thickness.Circle);

            var radial = (start - _position).normalized;

            var tickDirection = invertTick ? -1 : 1;
            if (i == 0)
            {
                var localOffset = radial * _radius * _length.BigTick * tickDirection;
                SpatialShapes.DrawLine(start, start - localOffset, bigTickColor, _thickness.BigTick);
            }
            else if (i == steps / 2)
            {
                var localOffset = radial * _radius * _length.MediumTick * tickDirection;
                SpatialShapes.DrawLine(start, start - localOffset, color, _thickness.MediumTick);
            }
            else if (i % 2 == 0)
            {
                var localOffset = radial * _radius * _length.SmallTick * tickDirection;
                SpatialShapes.DrawLine(start, start - localOffset, color, _thickness.SmallTick);
            }
        }
    }
}
