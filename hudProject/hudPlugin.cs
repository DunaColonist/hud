using BepInEx;
using HarmonyLib;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace hud;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class hudPlugin : BaseSpaceWarpPlugin
{
    private HudConfig _config;
    private HudGui _gui;

    public override void OnInitialized()
    {
        Logger.LogInfo("OnInitialized : start");
        base.OnInitialized();

        _config = new HudConfig(Config);
        _gui = new HudGui(SpaceWarpMetadata, _config);

        RegisterAllHarmonyPatchesInProject();

        Logger.LogInfo("OnInitialized : end");
    }

    private void OnGUI()
    {
        _gui.OnGUI();
    }


    public virtual void OnEnable()
    {
        Camera.onPreRender = (Camera.CameraCallback) System.Delegate.Combine(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
    }

    public virtual void OnDisable()
    {
        Camera.onPreRender = (Camera.CameraCallback) System.Delegate.Remove(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
    }

    private void OnCameraPreRender(Camera cam)
    {
        if (cam is null)
        {
            return;
        }
        if (cam.name != "FlightCameraPhysics_Main")
        {
            return;
        }
        HudDrawing.DrawHud(_config, cam);
    }

    private void RegisterAllHarmonyPatchesInProject()
    {
        Harmony.CreateAndPatchAll(typeof(hudPlugin).Assembly);
    }
}
