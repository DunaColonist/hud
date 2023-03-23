namespace hud.coordinates
{
    internal class Movement
    {
        public readonly Vector3d prograde;
        public readonly Vector3d? normal;
        public readonly Vector3d? radialIn;

        public Movement(Vector3d prograde)
        {
            this.prograde = prograde;
        }

        public Movement(Vector3d prograde, Vector3d normal, Vector3d radialIn)
        {
            this.prograde = prograde;
            this.normal = normal;
            this.radialIn = radialIn;
        }
    }
}
