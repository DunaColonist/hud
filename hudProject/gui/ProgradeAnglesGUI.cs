using Hud.Coordinates;
using UnityEngine;

namespace Hud.Gui;

internal class ProgradeAnglesGUI
{
    public void Build(LocalCoordinates coord)
    {
        if (coord is not null)
        {
            AngleDisplay("Horizontal", coord.HorizontalAngle.ToString() + "°");
            AngleDisplay("Vertical", coord.VerticalAngle.ToString() + "°");
        }
        else
        {
            AngleDisplay("Horizontal", "N/A");
            AngleDisplay("Vertical", "N/A");
        }
    }

    private void AngleDisplay(string orientation, string angle)
    {
        var letterWidth = 8;
        GUILayout.BeginHorizontal();
        GUILayout.Label(orientation, GUILayout.Width(letterWidth * 10));
        GUILayout.Label(" : ", GUILayout.Width(letterWidth * 3));
        GUILayout.Label(angle, GUILayout.Width(letterWidth * 3));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
