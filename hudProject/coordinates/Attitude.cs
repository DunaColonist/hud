namespace Hud.Coordinates;

internal class Attitude
{
    public Vector3d Forward { get; }
    public Vector3d Up { get; }

    public Attitude(Vector3d Forward, Vector3d Up)
    {
        this.Forward = Forward;
        this.Up = Up;
    }
}
