using hud.input;
using KSP.Sim;
using KSP.Sim.impl;
using UnityEngine;

namespace hud.coordinates
{
    internal class LocalCoordinates
    {
        public readonly Vector3d centerOfMass;

        public readonly Horizon horizon;
        public readonly Attitude attitude;
        public readonly Movement movement;
        public readonly Direction direction;

        public readonly Vector3d heading;
        public readonly Vector3d horizontalHeading;

        public readonly int horizontalAngle;
        public readonly int verticalAngle;

        public LocalCoordinates(VesselComponent vessel)
        {

            var frame = vessel.transform.coordinateSystem;
            centerOfMass = frame.ToLocalPosition(vessel.CenterOfMass);

            var telemetry = vessel._telemetryComponent;
            // TODO use vessel.SimulationObject.TargetingMode
            // to find what should be available
            // TODO need to wait for non sandbox mode to be released
            /*
                None,
                Direction,
                DirectionAndVelocity,
                DirectionVelocityAndOrientation
            */

            horizon = new Horizon(
                north: frame.ToLocalVector(telemetry.HorizonNorth).normalized,
                east: frame.ToLocalVector(telemetry.HorizonEast).normalized,
                sky: frame.ToLocalVector(telemetry.HorizonUp).normalized);

            attitude = new Attitude(
                forward : frame.ToLocalVector(vessel.MOI.coordinateSystem.forward).normalized,
                up : frame.ToLocalVector(vessel.MOI.coordinateSystem.up).normalized);

            var speedMode = vessel.speedMode;
            switch(speedMode)
            {
                case SpeedDisplayMode.Target:
                    {
                        movement = new Movement(prograde: frame.ToLocalVector(telemetry.TargetPrograde).normalized);
                        break;
                    }
                case SpeedDisplayMode.Orbit:
                    {
                        movement = new Movement(
                            prograde: frame.ToLocalVector(telemetry.OrbitMovementPrograde).normalized,
                            normal: frame.ToLocalVector(telemetry.OrbitMovementNormal).normalized,
                            radialIn: frame.ToLocalVector(telemetry.OrbitMovementRadialIn).normalized);
                        break;
                    }
                case SpeedDisplayMode.Surface:
                default:
                    {
                        movement = new Movement(prograde: frame.ToLocalVector(telemetry.SurfaceMovementPrograde).normalized);
                        break;
                    }
            };

            // TODO fix/improve that in order to make sense
            if (telemetry.SurfaceHorizontalSpeed > 1)
            {
                heading = movement.prograde;
                horizontalHeading = Vector3d.Exclude(horizon.sky, heading).normalized;
            } else
            {
                heading = attitude.forward;
                horizontalHeading = Vector3d.Exclude(horizon.sky, heading).normalized;
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
            direction = new Direction(target: target, maneuver: maneuver);

            horizontalAngle = (int) Vector3d.SignedAngle(horizontalHeading, horizon.north, -horizon.sky);
            verticalAngle = (int) Vector3d.SignedAngle(heading, horizontalHeading, -Vector3d.Cross(horizontalHeading, horizon.sky));
        }

        public Vector3 ControlVector(AttitudeControlOverride controlOverride)
        {
            var horizontalAngle = convert(controlOverride.HorizontalAngle);
            var horizontal =
                Math.Cos(horizontalAngle) * horizon.north +
                Math.Sin(-horizontalAngle) * horizon.east;

            var verticalAngle = convert(controlOverride.VerticalAngle);
            var vertical =
                Math.Cos(verticalAngle) * horizontal +
                Math.Sin(-verticalAngle) * horizon.sky;

            return vertical.normalized;
        }

        private double convert(double angle)
        {
            return -angle * (2 * Math.PI) / 360;
        }
    }
}
