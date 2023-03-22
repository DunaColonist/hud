using KSP.Sim.impl;
using Shapes;
using UnityEngine;
using UnityEngine.Rendering;

namespace hud
{
    internal class HudDrawing
    {
        public static void OnPostRender(Camera cam)
        {
            DrawCommand.OnPostRenderBuiltInRP(cam);
        }

        // Documentation available at : https://acegikmo.com/shapes/docs
        public void DrawHud(HudConfig config, UnityEngine.Camera cam)
        {
            if (!config._hudIsEnabled.Value)
            {
                return;
            }
            using (Draw.Command(cam, CameraEvent.AfterForwardAlpha))
            {
                Draw.ResetMatrix();

                var vessel = KSP.Game.GameManager.Instance.Game.ViewController.GetActiveSimVessel();
                if (vessel is null)
                {
                    return;
                }

                var simulationObjectView = KSP.Game.GameManager.Instance.Game.SpaceSimulation.ModelViewMap.FromModel(vessel.SimulationObject);
                if (simulationObjectView is null)
                {
                    return;
                }

                var radius = simulationObjectView.Vessel.BoundingSphere.radius * 200 / 100;

                NavballColors colors = new();
                Draw.TorusThicknessSpace = ThicknessSpace.Pixels;
                Draw.TorusRadiusSpace = ThicknessSpace.Meters;
                Draw.LineThicknessSpace = ThicknessSpace.Pixels;

                var coord = new LocalCoordinates(vessel);

                DrawTracking(coord, radius, vessel, colors);
                DrawSphere(coord, radius, vessel, colors);
            }
        }

        private void DrawTracking(LocalCoordinates coord, float radius, VesselComponent vessel, NavballColors colors)
        {
            var drawing = new TrackingDrawing(coord.centerOfMass, radius);
            var situation = vessel.Situation;

            if (situation == VesselSituations.Flying)
            {
                drawing.DrawHeading(coord.surfaceMovementPrograde, colors.prograde);
            }

            if (situation == VesselSituations.SubOrbital || situation == VesselSituations.Orbiting || situation == VesselSituations.Escaping)
            {
                drawing.DrawHeading(coord.orbitMovementPrograde, colors.prograde);
                drawing.DrawReference(coord.orbitMovementNormal, colors.normal);
                drawing.DrawReference(coord.orbitMovementRadialIn, colors.radial);
            }

            drawing.DrawHeading(coord.up, colors.up, true);

            drawing.DrawSelection(coord.targetDirection, colors.target);
            drawing.DrawSelection(coord.maneuverDirection, colors.maneuver);
        }

        private void DrawSphere(LocalCoordinates coor, float radius, VesselComponent vessel, NavballColors colors)
        {
            var coord = new LocalCoordinates(vessel);

            new GraduatedCircle(coord.centerOfMass, coord.sky, coord.horizontalHeading, radius,
                new Color[] { colors.sky, colors.sky, colors.ground, colors.ground },
                new Color[] { colors.ground, colors.sky, colors.ground, colors.ground },
                true
            );

            new GraduatedCircle(coord.centerOfMass, coord.north, coord.east, radius,
                new Color[] { colors.horizonEdge, colors.horizonEdge, colors.horizonEdge, colors.horizonEdge },
                new Color[] { colors.east, colors.north, colors.west, colors.south },
                false
            );
        }
    }
}
