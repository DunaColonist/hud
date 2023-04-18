using BepInEx.Configuration;
using UnityEngine;

namespace hud.gui
{
    internal class ModConfigurationGUI
    {
        public void Build(HudConfig config)
        {
            ConfigToggle(config._hudIsEnabled);
        }

        private void ConfigToggle(ConfigEntry<bool> config)
        {
            var content = new GUIContent(config.Definition.Key, config.Description.Description);
            config.Value = GUILayout.Toggle(config.Value, content);
        }
    }
}
