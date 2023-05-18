using Shapes;
using UnityEngine;

namespace Hud.Shapes;

internal class SpatialShapes
{
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float thickness, DashStyle dashStyle = null)
    {
        var blendMode = ShapesBlendMode.Opaque;
        var geometry = LineGeometry.Volumetric3D;
        var endCaps = LineEndCap.Square;
        var thicknessSpace = ThicknessSpace.Meters;

        Draw.Line(blendMode, geometry, endCaps, thicknessSpace, start, end, color, color, thickness, dashStyle);
    }

    public static void DrawCone(Vector3d pos, Vector3d normal, float radius, float length, Color color)
    {
        var blendMode = ShapesBlendMode.Opaque;
        var sizeSpace = ThicknessSpace.Meters;
        var rot = Quaternion.LookRotation(normal);
        var fillCap = true;

        Draw.Cone(blendMode, sizeSpace, pos, rot, radius, length, fillCap, color);
    }

    public static void DrawTorus(Vector3d pos, Vector3d normal, float radius, float thickness, Color color) 
    {
        var blendMode = ShapesBlendMode.Opaque;
        var spaceRadius = ThicknessSpace.Meters;
        var spaceThickness = ThicknessSpace.Meters;
        var rot = Quaternion.LookRotation(normal);
        
        Draw.Torus(blendMode, spaceRadius, spaceThickness, pos, rot, radius, thickness, color);
    }

}
