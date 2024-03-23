using Hud.Gui;
using Hud.Input;
using KSP.UI.Binding;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods.JSON;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;

namespace Hud;

internal class HudGui
{
    private readonly string _text;
    private readonly string _buttonId;

    private readonly HudConfig _config;
    private readonly AttitudeControlOverride _controlOverride;

    private bool _isWindowOpen;
    private Rect _windowRect;

    // XXX should we only use ModInfo and not MyPluginInfo ?
    public HudGui(ModInfo modInfo, HudConfig config, AttitudeControlOverride controlOverride)
    {
        _text = MyPluginInfo.PLUGIN_NAME;
        var modId = MyPluginInfo.PLUGIN_GUID;
        _buttonId = "BTN-" + modId + "-Flight";
        RegisterFlightAppBarButton(modInfo, _text, _buttonId);

        _config = config;
        _controlOverride = controlOverride;
    }

    private void CloseWindows()
    {
        _isWindowOpen = false;
        GameObject.Find(_buttonId)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
    }

    public void OnGUI()
    {
        GUI.skin = Skins.ConsoleSkin;

        if (_isWindowOpen)
        {
            _windowRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                _windowRect,
                FillWindow,
                _text,
                GUILayout.Height(50),
                GUILayout.Width(350)
            );
        }
    }

    private void FillWindow(int windowID)
    {
        if (GUI.Button(new Rect(_windowRect.width - 18, 2, 16, 16), "x"))
        {
            CloseWindows();
        }

        new MainGUI().Build(_config, _controlOverride);
    }

    public void Update()
    {
        _isWindowOpen = false;
        GameObject.Find(_buttonId)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
    }

    private Texture2D GetIcon(ModInfo modInfo)
    {
        return AssetManager.GetAsset<Texture2D>($"{modInfo.ModID}/images/icon.png");
    }

    private void RegisterFlightAppBarButton(ModInfo modInfo, string text, string id)
    {
        Appbar.RegisterAppButton(text, id, GetIcon(modInfo), isOpen =>
        {
            _isWindowOpen = isOpen;
            GameObject.Find(id)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
        }
       );
    }
}
