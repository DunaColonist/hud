namespace Hud.Coordinates;

internal class Movement
{
    public Vector3d Prograde { get; }
    public Vector3d? Normal { get; }
    public Vector3d? RadialIn { get; }

    public Movement(Vector3d Prograde)
    {
        this.Prograde = Prograde;
    }

    public Movement(Vector3d Prograde, Vector3d Normal, Vector3d RadialIn)
    {
        this.Prograde = Prograde;
        this.Normal = Normal;
        this.RadialIn = RadialIn;
    }
}
