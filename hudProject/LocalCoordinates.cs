using KSP.Sim.impl;

namespace hud
{
    internal class LocalCoordinates
    {
        public readonly Vector3d centerOfMass;

        public readonly Vector3d north;
        public readonly Vector3d east;
        public readonly Vector3d sky;

        public readonly Vector3d forward;
        public readonly Vector3d up;

        public readonly Vector3d surfaceMovementPrograde;

        public readonly Vector3d orbitMovementPrograde;
        public readonly Vector3d orbitMovementNormal;
        public readonly Vector3d orbitMovementRadialIn;

        public readonly Vector3d horizontalHeading;

        public readonly Vector3d? targetDirection;
        public readonly Vector3d? maneuverDirection;

        public LocalCoordinates(VesselComponent vessel)
        {
            var telemetry = vessel._telemetryComponent;
            var frame = vessel.transform?.coordinateSystem;

            centerOfMass = frame.ToLocalPosition(vessel.CenterOfMass);

            north = frame.ToLocalVector(telemetry.HorizonNorth).normalized;
            east = frame.ToLocalVector(telemetry.HorizonEast).normalized;
            sky = frame.ToLocalVector(telemetry.HorizonUp).normalized;

            forward = frame.ToLocalVector(vessel.MOI.coordinateSystem.forward).normalized;
            up = frame.ToLocalVector(vessel.MOI.coordinateSystem.up).normalized;

            surfaceMovementPrograde = frame.ToLocalVector(telemetry.SurfaceMovementPrograde).normalized;

            orbitMovementPrograde = frame.ToLocalVector(telemetry.OrbitMovementPrograde).normalized;
            orbitMovementNormal = frame.ToLocalVector(telemetry.OrbitMovementNormal).normalized;
            orbitMovementRadialIn = frame.ToLocalVector(telemetry.OrbitMovementRadialIn).normalized;

            if (telemetry.SurfaceHorizontalSpeed > 1)
            {
                horizontalHeading = Vector3d.Exclude(sky, surfaceMovementPrograde).normalized;
            } else
            {
                horizontalHeading = Vector3d.Exclude(sky, forward).normalized;
            }

            if (telemetry.HasTargetObject)
            {
                targetDirection = frame.ToLocalVector(telemetry.TargetDirection).normalized;
            }
            if (telemetry.HasManeuver)
            {
                maneuverDirection = frame.ToLocalVector(telemetry.ManeuverDirection).normalized;
            }
        }

    }
}
