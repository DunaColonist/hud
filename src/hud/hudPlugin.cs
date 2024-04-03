using System;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using hud.UI;
using hud.Input;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

using KSP.Game;
using KSP.Messages;

namespace hud;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class hudPlugin : BaseSpaceWarpPlugin
{
    [PublicAPI] public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    [PublicAPI] public const string ModName = MyPluginInfo.PLUGIN_NAME;
    [PublicAPI] public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    [PublicAPI] public static hudPlugin Instance { get; set; }

    internal const string ToolbarFlightButtonID = "BTN-hudFlight";
    internal const string ToolbarKscButtonID = "BTN-hudKSC";

    private HudConfig _config;
    private AttitudeControlOverride _controlOverride;
    private HudGui _gui;
    private HudDrawing _drawing;

    private bool hudIsRequired;

    public override void OnInitialized()
    {
        Logger.LogInfo("OnInitialized : start");
        base.OnInitialized();

        _config = new HudConfig(Config);
        _controlOverride = new AttitudeControlOverride();
        _gui = new HudGui(SpaceWarpMetadata, _config, _controlOverride);
        _drawing = new HudDrawing();

        Instance = this;

        RegisterAllHarmonyPatchesInProject();
        RegisterDetectionOfHudNeed();
        LoadAssemblies();

        // Load the UI from the asset bundle
        var hudWindowUxml = AssetManager.GetAsset<VisualTreeAsset>(
            // The case-insensitive path to the asset in the bundle is composed of:
            // - The mod GUID:
            $"{ModGuid}/" +
            // - The name of the asset bundle:
            "hud_ui/" +
            // - The path to the asset in your Unity project (without the "Assets/" part)
            "ui/hudwindow/hudwindow.uxml"
        );

        // Create the window options object
        var windowOptions = new WindowOptions
        {
            WindowId = "hud_HudWindow",
            Parent = null,
            IsHidingEnabled = true,
            DisableGameInputForTextFields = true,
            MoveOptions = new MoveOptions
            {
                IsMovingEnabled = true,
                CheckScreenBounds = true
            }
        };

        // Create the window
        var hudWindow = Window.Create(windowOptions, hudWindowUxml);
        // Add a controller for the UI to the window's game object
        var hudWindowController = hudWindow.gameObject.AddComponent<HudWindowController>();

        // Register Flight AppBar button
        Appbar.RegisterAppButton(
            ModName,
            ToolbarFlightButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            isOpen => hudWindowController.IsWindowOpen = isOpen
        );

        // Register KSC AppBar Button
        Appbar.RegisterKSCAppButton(
            ModName,
            ToolbarKscButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            () => hudWindowController.IsWindowOpen = !hudWindowController.IsWindowOpen
        );

        Logger.LogInfo("OnInitialized : end");
    }

    private static void LoadAssemblies()
    {
        var currentFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;
        var unityAssembly = Assembly.LoadFrom(Path.Combine(currentFolder, "hud.Unity.dll"));
        CustomControls.RegisterFromAssembly(unityAssembly);
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
        Logger.LogInfo("OnEnable : start");
        Camera.onPreRender = (Camera.CameraCallback)System.Delegate.Combine(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
        Camera.onPostRender = (Camera.CameraCallback)System.Delegate.Combine(
            Camera.onPostRender,
            new Camera.CameraCallback(OnCameraPostRender)
        );
        Logger.LogInfo("OnEnable : end");
    }

    public virtual void OnDisable()
    {
        Logger.LogInfo("OnDisable : start");
        Camera.onPreRender = (Camera.CameraCallback)System.Delegate.Remove(
            Camera.onPreRender,
            new Camera.CameraCallback(OnCameraPreRender)
        );
        Camera.onPostRender = (Camera.CameraCallback)System.Delegate.Remove(
            Camera.onPostRender,
            new Camera.CameraCallback(OnCameraPostRender)
        );
        Logger.LogInfo("OnDisable : end");
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
        }
        catch (Exception e)
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
            hudIsRequired = message.CurrentState == GameState.FlightView;
            
            Logger.LogInfo($"hud is required : {hudIsRequired}");
        });
    }
}
