using Shapes;
using UnityEngine;

namespace hud
{
    // TODO clean up magic numbers
    internal class TrackingDrawing
    {
        private readonly Vector3 position;
        private readonly float radius;

        public TrackingDrawing(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }

        public void DrawHeading(Vector3? direction, Color color, Boolean invert = false)
        {
            if (direction is null)
            {
                return;
            }
            var scaledDirection = direction.Value.normalized * radius;

            var inversion = invert ? -1 : 1;

            Draw.Line(position, position + scaledDirection, 1, color);
            Draw.Cone(position + scaledDirection + direction.Value * inversion * radius / 10, -direction.Value * inversion, radius / 25, radius / 10, color);

            Draw.LineDashed(position, position - scaledDirection, 1, color);
            Draw.Cone(position - scaledDirection - direction.Value * inversion * radius / 20, direction.Value * inversion, radius / 50, radius / 20, color);
        }

        public void DrawReference(Vector3? direction, Color color)
        {
            if (direction is null)
            {
                return;
            }
            var scaledDirection = direction.Value.normalized * radius;

            Draw.Torus(position + scaledDirection * 1.01f, direction.Value, radius / 25, 5, color);
            Draw.Torus(position - scaledDirection * 1.01f, direction.Value, radius / 50, 5, color);
        }

        public void DrawSelection(Vector3? direction, Color color)
        {
            if (direction is null)
            {
                return;
            }
            var scaledDirection = direction.Value.normalized * radius;

            Draw.Torus(position + scaledDirection * 0.99f, direction.Value, radius / 25, 5, color);
            Draw.Torus(position - scaledDirection * 0.99f, direction.Value, radius / 50, 5, color);
        }
    }
}
