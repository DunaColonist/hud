using Hud.Input;
using KSP.Sim;
using KSP.Sim.impl;
using UnityEngine;

namespace Hud.Coordinates;

internal class LocalCoordinates
{
    public Vector3d CenterOfMass { get; }

    public Horizon Horizon { get; }
    public Attitude Attitude { get; }
    public Movement Movement { get; }
    public Direction Direction { get; }

    public Vector3d Heading { get; }
    public Vector3d HorizontalHeading { get; }

    public int HorizontalAngle { get; }
    public int VerticalAngle { get; }

    public LocalCoordinates(VesselComponent Vessel)
    {
        var frame = Vessel.transform.coordinateSystem;
        CenterOfMass = frame.ToLocalPosition(Vessel.CenterOfMass);

        var telemetry = Vessel._telemetryComponent;

        Horizon = new Horizon(
            North: frame.ToLocalVector(telemetry.HorizonNorth).normalized,
            East: frame.ToLocalVector(telemetry.HorizonEast).normalized,
            Sky: frame.ToLocalVector(telemetry.HorizonUp).normalized);

        Attitude = new Attitude(
            Forward: frame.ToLocalVector(Vessel.MOI.coordinateSystem.forward).normalized,
            Up: frame.ToLocalVector(Vessel.MOI.coordinateSystem.up).normalized);

        var speedMode = Vessel.speedMode;
        switch (speedMode)
        {
            case SpeedDisplayMode.Target:
                {
                    Movement = new Movement(Prograde: frame.ToLocalVector(telemetry.TargetPrograde).normalized);
                    break;
                }

            case SpeedDisplayMode.Orbit:
                {
                    Movement = new Movement(
                        Prograde: frame.ToLocalVector(telemetry.OrbitMovementPrograde).normalized,
                        Normal: frame.ToLocalVector(telemetry.OrbitMovementNormal).normalized,
                        RadialIn: frame.ToLocalVector(telemetry.OrbitMovementRadialIn).normalized);
                    break;
                }

            case SpeedDisplayMode.Surface:
            default:
                {
                    Movement = new Movement(Prograde: frame.ToLocalVector(telemetry.SurfaceMovementPrograde).normalized);
                    break;
                }
        }

        // TODO fix/improve that in order to make sense
        if (telemetry.SurfaceHorizontalSpeed > 1)
        {
            Heading = Movement.Prograde;
            HorizontalHeading = Vector3d.Exclude(Horizon.Sky, Heading).normalized;
        }
        else
        {
            Heading = Attitude.Forward;
            HorizontalHeading = Vector3d.Exclude(Horizon.Sky, Heading).normalized;
        }

        Vector3d? target = null;
        if (telemetry.HasTargetObject)
        {
            target = frame.ToLocalVector(telemetry.TargetDirection).normalized;
        }

        Vector3d? maneuver = null;
        if (telemetry.HasManeuver)
        {
            maneuver = frame.ToLocalVector(telemetry.ManeuverDirection).normalized;
        }

        Direction = new Direction(Target: target, Maneuver: maneuver);

        HorizontalAngle = (int)Vector3d.SignedAngle(HorizontalHeading, Horizon.North, -Horizon.Sky);
        VerticalAngle = (int)Vector3d.SignedAngle(Heading, HorizontalHeading, -Vector3d.Cross(HorizontalHeading, Horizon.Sky));
    }

    public Vector3 ControlVector(AttitudeControlOverride controlOverride)
    {
        var horizontalAngle = Convert(controlOverride.HorizontalAngle);
        var horizontal =
            (Math.Cos(horizontalAngle) * Horizon.North) +
            (Math.Sin(-horizontalAngle) * Horizon.East);

        var verticalAngle = Convert(controlOverride.VerticalAngle);
        var vertical =
            (Math.Cos(verticalAngle) * horizontal) +
            (Math.Sin(-verticalAngle) * Horizon.Sky);

        return vertical.normalized;
    }

    private double Convert(double angle)
    {
        return -angle * (2 * Math.PI) / 360;
    }
}
