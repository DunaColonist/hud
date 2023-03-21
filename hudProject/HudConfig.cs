using BepInEx.Configuration;

namespace hud
{
    internal class HudConfig
    {
        public ConfigEntry<bool> _hudIsEnabled;

        public HudConfig(ConfigFile config)
        {
            const string section = "Settings";
            _hudIsEnabled = config.Bind<bool>(section, "Enable hud", true, "Enable display of hud");
        }
    }
}
