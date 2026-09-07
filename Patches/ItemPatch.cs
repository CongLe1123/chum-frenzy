using System;
using ChumFrenzy.Services;
using HarmonyLib;
using StardewValley;

namespace ChumFrenzy.Patches
{
    [HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
    public static class ItemPatch
    {
        public static void Postfix(ISalable other, ref bool __result, Item __instance)
        {
            try
            {
                if (!__result || other is not Item otherItem)
                    return;

                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    __instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetA);
                    otherItem.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetB);

                    if (!string.Equals(targetA, targetB, StringComparison.OrdinalIgnoreCase))
                    {
                        __result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Item.canStackWith patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
