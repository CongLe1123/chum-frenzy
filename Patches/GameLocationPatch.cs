using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ChumFrenzy.Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.getFish))]
    public static class GameLocationPatch
    {
        public static void Postfix(float millisecondsAfterNibble, string bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Item __result, GameLocation __instance)
        {
            try
            {
                if (ModEntry.FishingEffects != null && who != null && __instance != null)
                {
                    var modified = ModEntry.FishingEffects.ApplyCatchModifiers(__instance, bobberTile, who, __result, waterDepth);
                    if (modified != null)
                    {
                        __result = modified;
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance?.Monitor?.Log($"Error in GameLocation.getFish patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(StardewValley.Locations.MineShaft), nameof(StardewValley.Locations.MineShaft.getFish))]
    public static class MineShaftPatch
    {
        public static void Postfix(float millisecondsAfterNibble, string bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref Item __result, GameLocation __instance)
        {
            try
            {
                if (ModEntry.FishingEffects != null && who != null && __instance != null)
                {
                    var modified = ModEntry.FishingEffects.ApplyCatchModifiers(__instance, bobberTile, who, __result, waterDepth);
                    if (modified != null)
                    {
                        __result = modified;
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance?.Monitor?.Log($"Error in MineShaft.getFish patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
