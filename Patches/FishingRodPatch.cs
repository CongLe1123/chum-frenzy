using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace ChumFrenzy.Patches
{
    [HarmonyPatch(typeof(FishingRod), "calculateTimeUntilFishingBite")]
    public static class FishingRodPatch
    {
        public static void Postfix(Vector2 bobberTile, bool isFirstCast, Farmer who, ref float __result)
        {
            try
            {
                if (ModEntry.FishingEffects != null)
                {
                    __result = ModEntry.FishingEffects.ApplyBiteDelayReduction(bobberTile, who, __result);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in FishingRod.calculateTimeUntilFishingBite patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
