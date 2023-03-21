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

        public void DrawHeading(Vector3 direction, Color color, Boolean invert = false)
        {
            var scaledDirection = direction.normalized * radius;

            var inversion = invert ? -1 : 1;

            Draw.Line(position, position + scaledDirection, 1, color);
            Draw.Cone(position + scaledDirection + direction * inversion * radius / 10, -direction * inversion, radius / 25, radius / 10, color);

            Draw.LineDashed(position, position - scaledDirection, 1, color);
            Draw.Cone(position - scaledDirection - direction * inversion * radius / 20, direction * inversion, radius / 50, radius / 20, color);
        }

        public void DrawReference(Vector3 direction, Color color)
        {
            var scaledDirection = direction.normalized * radius;

            Draw.Torus(position + scaledDirection, direction, radius / 40, 10, color);
            Draw.Torus(position - scaledDirection, direction, radius / 50, 10, color);
        }

        public void DrawSelection(Vector3? direction, Color color)
        {
            if (direction is null)
            {
                return;
            }
            var scaledDirection = direction.Value.normalized * radius;

            Draw.Torus(position + scaledDirection, direction.Value, radius / 20, 10, color);
            Draw.Torus(position - scaledDirection, direction.Value, radius / 30, 10, color);
        }
    }
}
