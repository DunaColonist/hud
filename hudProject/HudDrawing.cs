﻿using Hud.Coordinates;
using Hud.Input;
using Hud.Shapes;
using KSP.Sim.impl;
using Shapes;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hud;

internal class HudDrawing
{
    private static float _previousRadius = 0;

    public static void OnPostRender(Camera cam)
    {
        DrawCommand.OnPostRenderBuiltInRP(cam);
    }

    // Documentation available at : https://acegikmo.com/shapes/docs
    public void DrawHud(HudConfig config, AttitudeControlOverride controlOverride, Camera cam)
    {
        if (!config.HudIsEnabled.Value)
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

            // 200% : worst case the center of mass is an extremity of the bouding sphere so it ensure that the hud is around the vessel
            var observedRadius = simulationObjectView.Vessel.BoundingSphere.radius * 200 / 100;
            if (_previousRadius == 0) {
                _previousRadius = observedRadius;
            }
            var change = Mathf.Abs(_previousRadius - observedRadius) / _previousRadius;
            var radius = (change <= 0.10) ? _previousRadius : observedRadius;
            _previousRadius = radius;


            NavballColors colors = new();

            var coord = new LocalCoordinates(vessel);

            DrawTracking(coord, radius, vessel, controlOverride, colors);
            DrawSphere(coord, radius, colors);
        }
    }

    private void DrawTracking(LocalCoordinates coord, float radius, VesselComponent vessel, AttitudeControlOverride controlOverride, NavballColors colors)
    {
        var trackingShapes = new TrackingShapes(coord.CenterOfMass, radius);
        var situation = vessel.Situation;

        // TODO find  better way to not display a random prograde
        if (situation != VesselSituations.PreLaunch && situation != VesselSituations.Landed && situation != VesselSituations.Splashed)
        {
            trackingShapes.DrawHeading(coord.Movement.Prograde, colors.Prograde);
        }

        trackingShapes.DrawReference(coord.Movement.Normal, colors.Normal);
        trackingShapes.DrawReference(coord.Movement.RadialIn, colors.Radial);

        if (controlOverride.IsEnabled)
        {
            Vector3 controlVector = coord.ControlVector(controlOverride);
            trackingShapes.DrawHeading(controlVector, Color.blue, true);
        }
        else
        {
            trackingShapes.DrawHeading(coord.Attitude.Up, colors.Up, true);
        }

        trackingShapes.DrawSelection(coord.Direction.Target, colors.Target);
        trackingShapes.DrawSelection(coord.Direction.Maneuver, colors.Maneuver);
    }

    private void DrawSphere(LocalCoordinates coord, float radius, NavballColors colors)
    {
        var shapes = new SphereShapes(coord.CenterOfMass, radius);

        shapes.DrawGraduatedTorus(
            coord.Horizon.Sky,
            coord.HorizontalHeading,
            new Color[] { colors.Sky, colors.Sky, colors.Ground, colors.Ground },
            new Color[] { colors.Ground, colors.Sky, colors.Ground, colors.Ground },
            true);

        shapes.DrawGraduatedTorus(
            coord.Horizon.North,
            coord.Horizon.East,
            new Color[] { colors.HorizonEdge, colors.HorizonEdge, colors.HorizonEdge, colors.HorizonEdge },
            new Color[] { colors.East, colors.North, colors.West, colors.South },
            false);
    }
}
