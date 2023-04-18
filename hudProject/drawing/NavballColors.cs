using UnityEngine;

namespace hud.drawing
{
    internal class NavballColors
    {
        public readonly Color background = new Color(1, 1, 1, 0.01f);

        public readonly Color sky = color100(21, 64, 78);
        public readonly Color ground = color100(67, 42, 19);

        public readonly Color north = color100(80, 16, 0);
        public readonly Color south = color100(2, 36, 0);
        public readonly Color east = color100(8, 20, 54);
        public readonly Color west = color100(8, 20, 54);

        public readonly Color horizonEdge = Color.grey;
        public readonly Color horizonFill = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.05f);

        public readonly Color up = color100(80, 66, 0);

        public readonly Color prograde = color100(0, 100, 40);
        public readonly Color normal = color100(47, 14, 47);
        public readonly Color radial = color100(1, 54, 54);

        public readonly Color target = Color.grey;
        public readonly Color maneuver = color100(100, 82, 59);

        private static Color color100(int r, int g, int b, double a = 1)
        {
            var scale = 100f;
            return new Color(r / scale, g / scale, b / scale, (float)a);
        }
    }
}
