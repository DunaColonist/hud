using BepInEx.Configuration;

namespace hud;

public class HudConfig
{
    public ConfigEntry<bool> HudIsEnabled { get; }

    public HudConfig(ConfigFile config)
    {
        const string section = "Settings";
        HudIsEnabled = config.Bind<bool>(section, "Enable display of hud", true, "When disabled, the hud is not displayed.");
    }
}
