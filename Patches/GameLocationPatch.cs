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
                if (ModEntry.FishingEffects != null && __result != null && who != null)
                {
                    __result = ModEntry.FishingEffects.ApplyCatchModifiers(__instance, bobberTile, who, __result, waterDepth);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in GameLocation.getFish patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
