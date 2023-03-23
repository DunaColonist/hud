namespace hud.coordinates
{
    internal class Attitude
    {
        public readonly Vector3d forward;
        public readonly Vector3d up;

        public Attitude(Vector3d forward, Vector3d up)
        {
            this.forward = forward;
            this.up = up;
        }
    }
}
