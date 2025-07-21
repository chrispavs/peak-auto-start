using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AutoStart;

class ChrisPavsAutoStartPatches
{
    private static bool started = false;

    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Update))]
    [HarmonyPostfix]
    private static void MainMenu_Update(MainMenu __instance)
    {
        if (!started)
        {
            started = true;
            if (Plugin.offlineModeConfig?.Value ?? false)
            {
                Plugin.Log.LogInfo($"Launching in offline (solo) mode.");
                __instance.PlaySoloClicked();
            }
            else
            {
                Plugin.Log.LogInfo($"Launching in online (multiplayer) mode.");
                __instance.GoToAirport();
            }
        }
    }
}

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static ConfigEntry<bool>? offlineModeConfig;

    private void Awake()
    {
        Log = Logger;

        // Log our awake here so we can see it in LogOutput.log file.
        Log.LogInfo($"Plugin {Name} is loaded!");

        // Apply our patches.
        Harmony.CreateAndPatchAll(typeof(ChrisPavsAutoStartPatches));

        // Create config options.
        offlineModeConfig = Config.Bind("Settings", "OfflineMode", false, "If true, the game will start and automatically load into an offline lobby. If false, the game will start and automatically load into an online lobby.");
    }
}
