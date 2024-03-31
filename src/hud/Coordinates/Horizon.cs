namespace hud.Coordinates;

internal class Horizon
{
    public Vector3d North { get; }
    public Vector3d East { get; }
    public Vector3d Sky { get; }

    public Horizon(Vector3d North, Vector3d East, Vector3d Sky)
    {
        this.North = North;
        this.East = East;
        this.Sky = Sky;
    }
}
