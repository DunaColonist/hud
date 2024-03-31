using hud.Coordinates;
using hud.Input;
using UnityEngine;

namespace hud.Gui;

internal class MainGUI
{
    public void Build(HudConfig config, AttitudeControlOverride controlOverride)
    {
        var vessel = KSP.Game.GameManager.Instance.Game.ViewController.GetActiveSimVessel();
        LocalCoordinates coord = null;
        if (vessel is not null)
        {
            coord = new LocalCoordinates(vessel);
        }

        GUILayout.BeginVertical();

        GUILayout.Space(10);
        SectionSeparator("Mod configuration");
        new ModConfigurationGUI().Build(config);

        GUILayout.Space(10);
        SectionSeparator("Prograde");
        new ProgradeAnglesGUI().Build(coord);

        GUILayout.Space(10);
        SectionSeparator("Attitude control");
        new AttitudeControlOverrideGUI().Build(coord, controlOverride);

        GUILayout.Space(10);
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, 10000, 500));
    }

    private void SectionSeparator(string text)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("-----");
        GUILayout.FlexibleSpace();
        GUILayout.Label(text);
        GUILayout.FlexibleSpace();
        GUILayout.Label("-----");
        GUILayout.EndHorizontal();
    }
}
