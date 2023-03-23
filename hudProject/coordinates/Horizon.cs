namespace hud.coordinates
{
    internal class Horizon
    {
        public readonly Vector3d north;
        public readonly Vector3d east;
        public readonly Vector3d sky;

        public Horizon(Vector3d north, Vector3d east, Vector3d sky)
        {
            this.north = north;
            this.east = east;
            this.sky = sky;
        }
    }
}
