namespace Hud.Coordinates;

internal class Direction
{
    public Vector3d? Target { get; }
    public Vector3d? Maneuver { get; }

    public Direction(Vector3d? Target, Vector3d? Maneuver)
    {
        this.Target = Target;
        this.Maneuver = Maneuver;
    }
}
