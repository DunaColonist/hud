using BepInEx;
using HarmonyLib;
using hud.input;
using KSP.Game;
using KSP.Messages;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace hud;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class hudPlugin : BaseSpaceWarpPlugin
{
    private HudConfig _config;
    private AttitudeControlOverride _controlOverride;
    private HudGui _gui;
    private HudDrawing _drawing;

    private Boolean hudIsRequired;

    public override void OnInitialized()
    {
        Logger.LogInfo("OnInitialized : start");
        base.OnInitialized();

        _config = new HudConfig(Config);
        _controlOverride = new AttitudeControlOverride();
        _gui = new HudGui(SpaceWarpMetadata, _config, _controlOverride);
        _drawing = new HudDrawing();

        RegisterAllHarmonyPatchesInProject();
        RegisterDetectionOfHudNeed();

        Logger.LogInfo("OnInitialized : end");
    }

    private void OnGUI()
    {
        if (_gui is not null)
        {
            _gui.OnGUI();
        }
    }


    public virtual void OnEnable()
    {
        Camera.onPreRender = (Camera.CameraCallback) System.Delegate.Combine(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
        Camera.onPostRender = (Camera.CameraCallback)System.Delegate.Combine(
            Camera.onPostRender,
            new Camera.CameraCallback(OnCameraPostRender)
        );
    }

    public virtual void OnDisable()
    {
        Camera.onPreRender = (Camera.CameraCallback) System.Delegate.Remove(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
        Camera.onPostRender = (Camera.CameraCallback)System.Delegate.Remove(
            Camera.onPostRender,
            new Camera.CameraCallback(OnCameraPostRender)
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
        if (!hudIsRequired)
        {
            return;
        }
        try
        {
            _drawing.DrawHud(_config, _controlOverride, cam);
        } catch (Exception e)
        {
            Logger.LogError($"Error during drawing of hud : {e.GetType()} {e.Message}");
        }
        
    }

    private void OnCameraPostRender(Camera cam)
    {
        if (!hudIsRequired)
        {
            return;
        }
        HudDrawing.OnPostRender(cam);
    }

    private void RegisterAllHarmonyPatchesInProject()
    {
        Harmony.CreateAndPatchAll(typeof(hudPlugin).Assembly);
    }

    private void RegisterDetectionOfHudNeed()
    {
        Game.Messages.Subscribe<GameStateChangedMessage>(msg =>
        {
            var message = (GameStateChangedMessage)msg;

            if (message.CurrentState == GameState.FlightView)
            {
                hudIsRequired = true;
            }
            else if (message.PreviousState == GameState.FlightView)
            {
                hudIsRequired = false;
            }
        });
    }
}
