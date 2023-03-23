namespace hud.coordinates
{
    internal class Direction
    {
        public readonly Vector3d? target;
        public readonly Vector3d? maneuver;

        public Direction(Vector3d? target, Vector3d? maneuver)
        {
            this.target = target;
            this.maneuver = maneuver;
        }
    }
}
