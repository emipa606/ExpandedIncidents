using Verse;

namespace ExpandedIncidents.Harmony;

[StaticConstructorOnStartup]
public static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new HarmonyLib.Harmony("com.github.harmony.rimworld.mod.expandedincidents").PatchAll();
    }
}