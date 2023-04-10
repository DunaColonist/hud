using SpaceWarp.API.Assets;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using SpaceWarp.API.Mods.JSON;
using UnityEngine;
using KSP.UI.Binding;
using BepInEx.Configuration;

using hud.coordinates;

namespace hud
{
    internal class HudGui
    {
        private string _text;

        private string _buttonId;

        private bool _isWindowOpen;
        private Rect _windowRect;

        private HudConfig _config;

        // XXX should we only use ModInfo and not MyPluginInfo ?
        public HudGui(ModInfo modInfo, HudConfig config)
        {
            _text = MyPluginInfo.PLUGIN_NAME;
            var modId = MyPluginInfo.PLUGIN_GUID;
            _buttonId = "BTN-" + modId + "-Flight";
            RegisterFlightAppBarButton(modInfo, _text, _buttonId);

            _config = config;
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
               

            GUILayout.BeginVertical();

            GUILayout.Space(10);

            ConfigToggle(_config._hudIsEnabled);

            var vessel = KSP.Game.GameManager.Instance.Game.ViewController.GetActiveSimVessel();
            if (vessel is not null)
            {
                var coord = new LocalCoordinates(vessel);

                var horizontalAngle = Vector3d.SignedAngle(coord.horizontalHeading, coord.horizon.north, -coord.horizon.sky);
                AngleDisplay("Horizontal", ((int)horizontalAngle).ToString());

                var verticalAngle = Vector3d.SignedAngle(coord.heading, coord.horizontalHeading, -Vector3d.Cross(coord.horizontalHeading, coord.horizon.sky));
                AngleDisplay("Vertical", ((int)verticalAngle).ToString());
            } else
            {
                AngleDisplay("Horizontal", "N/A");
                AngleDisplay("Vertical", "N/A");
            }

            GUILayout.Space(10);

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }

        public void Update()
        {
            _isWindowOpen = false;
            GameObject.Find(_buttonId)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
        }

        public void ConfigToggle(ConfigEntry<bool> config)
        {
            config.Value = GUILayout.Toggle( config.Value, new GUIContent(config.Definition.Key, config.Description.Description));
        }

        public void AngleDisplay(string orientation, string angle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(orientation + " : ");
            GUILayout.Label(angle);
            GUILayout.EndHorizontal();
        }

        private Texture2D GetIcon(ModInfo modInfo)
        {
            return AssetManager.GetAsset<Texture2D>($"{modInfo.ModID}/images/icon.png");
        }

        private void RegisterFlightAppBarButton(ModInfo modInfo, string text, string id)
        {
            _buttonId = id;
            Appbar.RegisterAppButton(text, id, GetIcon(modInfo), isOpen =>
            {
                _isWindowOpen = isOpen;
                GameObject.Find(id)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
            }
           );
        }
    }
}
