using System.Collections;
using System.Collections.Specialized;
using hud.Coordinates;
using hud.Input;
using KSP.Sim;
using UnityEngine;

namespace hud.Gui;

internal class AttitudeControlOverrideGUI
{
    public void Build(LocalCoordinates coord, AttitudeControlOverride controlOverride)
    {
        controlOverride.IsEnabled = GUILayout.Toggle(controlOverride.IsEnabled, new GUIContent(
                       "Enable override", "Control the craft attitude from this popup."));

        GUILayout.BeginVertical();
        AddCardinalPoints(controlOverride);
        AddAngle("horizontal", ref controlOverride.HorizontalAngle);
        AddAngle("vertical", ref controlOverride.VerticalAngle);
        GUILayout.EndVertical();

        var vessel = KSP.Game.GameManager.Instance.Game.ViewController.GetActiveSimVessel();

        // XXX deplace somewhere not in GUI ;)
        if (controlOverride.IsEnabled)
        {
            if (vessel.Autopilot.AutopilotMode != AutopilotMode.Autopilot)
            {
                vessel.Autopilot.Activate(AutopilotMode.Autopilot);
            }

            if (controlOverride.HasChanged())
            {
                controlOverride.ResetChange();
                var vector = new Vector(
                    vessel.transform.coordinateSystem,
                    coord.ControlVector(controlOverride));
                vessel.Autopilot.SAS.SetTargetOrientation(vector, true);
            }
        }
    }

    private void AddCardinalPoints(AttitudeControlOverride controlOverride)
    {
        var cardinalPoints = new OrderedDictionary
        {
            { "N", 0 },
            { "E", 90 },
            { "S", 180 },
            { "W", -90 }
        };

        GUILayout.BeginHorizontal();
        foreach (DictionaryEntry point in cardinalPoints)
        {
            if (GUILayout.Button((string)point.Key))
            {
                controlOverride.HorizontalAngle = (int)point.Value;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void AddAngle(string name, ref int angle)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(name, LetterWidth(10));
        GUILayout.Label(" : ", LetterWidth(3));
        var inputAngle = GUILayout.TextField(angle.ToString(), LetterWidth(6));
        angle = int.Parse(inputAngle);
        GUILayout.Label("°", LetterWidth(1));

        GUILayout.FlexibleSpace();

        AddAngleSteps(ref angle);

        GUILayout.EndHorizontal();

        if (angle > 180 )
        {
            angle -= 360;
        }

        if (angle < -180)
        {
            angle += 360;
        }
    }

    private void AddAngleSteps(ref int angle)
    {
        var steps = new int[] { 1, 5, 10 };

        GUILayout.BeginHorizontal();
        foreach (int step in steps)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("+" + step))
            {
                angle += step;
            }

            if (GUILayout.Button("-" + step))
            {
                angle -= step;
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }

    private GUILayoutOption LetterWidth(int letters)
    {
        var letterWidth = 8;
        return GUILayout.Width(letterWidth * letters);
    }
}
