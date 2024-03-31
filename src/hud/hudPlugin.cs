using System.Reflection;
using BepInEx;
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

        RegisterDetectionOfHudNeed();
        LoadAssemblies();

        // Load the UI from the asset bundle
        var myFirstWindowUxml = AssetManager.GetAsset<VisualTreeAsset>(
            // The case-insensitive path to the asset in the bundle is composed of:
            // - The mod GUID:
            $"{ModGuid}/" +
            // - The name of the asset bundle:
            "hud_ui/" +
            // - The path to the asset in your Unity project (without the "Assets/" part)
            "ui/myfirstwindow/myfirstwindow.uxml"
        );

        // Create the window options object
        var windowOptions = new WindowOptions
        {
            // The ID of the window. It should be unique to your mod.
            WindowId = "hud_MyFirstWindow",
            // The transform of parent game object of the window.
            // If null, it will be created under the main canvas.
            Parent = null,
            // Whether or not the window can be hidden with F2.
            IsHidingEnabled = true,
            // Whether to disable game input when typing into text fields.
            DisableGameInputForTextFields = true,
            MoveOptions = new MoveOptions
            {
                // Whether or not the window can be moved by dragging.
                IsMovingEnabled = true,
                // Whether or not the window can only be moved within the screen bounds.
                CheckScreenBounds = true
            }
        };

        // Create the window
        var myFirstWindow = Window.Create(windowOptions, myFirstWindowUxml);
        // Add a controller for the UI to the window's game object
        var myFirstWindowController = myFirstWindow.gameObject.AddComponent<MyFirstWindowController>();

        // Register Flight AppBar button
        Appbar.RegisterAppButton(
            ModName,
            ToolbarFlightButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            isOpen => myFirstWindowController.IsWindowOpen = isOpen
        );

        // Register KSC AppBar Button
        Appbar.RegisterKSCAppButton(
            ModName,
            ToolbarKscButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            () => myFirstWindowController.IsWindowOpen = !myFirstWindowController.IsWindowOpen
        );

        Logger.LogInfo("OnInitialized : end");
    }

    private static void LoadAssemblies()
    {
        var currentFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;
        var unityAssembly = Assembly.LoadFrom(Path.Combine(currentFolder, "hud.Unity.dll"));
        CustomControls.RegisterFromAssembly(unityAssembly);
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
        // Harmony.CreateAndPatchAll(typeof(HudPlugin).Assembly);
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
