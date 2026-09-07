using System;
using ChumFrenzy.Services;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace ChumFrenzy.Patches
{
    [HarmonyPatch(typeof(StardewValley.Object))]
    public static class ObjectPatch
    {
        [HarmonyPatch("loadDisplayName")]
        [HarmonyPostfix]
        public static void LoadDisplayName_Postfix(StardewValley.Object __instance, ref string __result)
        {
            try
            {
                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    if (__instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetFish) && !string.IsNullOrEmpty(targetFish))
                    {
                        var data = ItemRegistry.GetData(targetFish);
                        if (data != null)
                        {
                            string format = ModEntry.Instance.Helper.Translation.Get("item.species_chum.name");
                            __result = string.Format(format, data.DisplayName);
                            return;
                        }
                    }

                    __result = ModEntry.Instance.Helper.Translation.Get("item.species_chum.unknown_name");
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Object.loadDisplayName patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }

        [HarmonyPatch(nameof(StardewValley.Object.getDescription))]
        [HarmonyPostfix]
        public static void GetDescription_Postfix(StardewValley.Object __instance, ref string __result)
        {
            try
            {
                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    if (__instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetFish) && !string.IsNullOrEmpty(targetFish))
                    {
                        var data = ItemRegistry.GetData(targetFish);
                        if (data != null)
                        {
                            string format = ModEntry.Instance.Helper.Translation.Get("item.species_chum.description");
                            string text = string.Format(format, data.DisplayName);
                            __result = Game1.parseText(text, Game1.smallFont, 384);
                            return;
                        }
                    }

                    string fallback = ModEntry.Instance.Helper.Translation.Get("item.species_chum.unknown_description");
                    __result = Game1.parseText(fallback, Game1.smallFont, 384);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Object.getDescription patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }

        [HarmonyPatch(nameof(StardewValley.Object.sellToStorePrice))]
        [HarmonyPostfix]
        public static void SellToStorePrice_Postfix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    if (__instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetFish) && !string.IsNullOrEmpty(targetFish))
                    {
                        Item fishItem = ItemRegistry.Create(targetFish);
                        int basePrice = fishItem.salePrice(false);
                        __result = Math.Clamp((int)(basePrice * 0.10f) + 20, 20, 250);
                        return;
                    }

                    __result = 20;
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Object.sellToStorePrice patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }

        [HarmonyPatch(nameof(StardewValley.Object.drawInMenu))]
        [HarmonyPostfix]
        public static void DrawInMenu_Postfix(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow, StardewValley.Object __instance)
        {
            try
            {
                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    if (__instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetFish) && !string.IsNullOrEmpty(targetFish))
                    {
                        ParsedItemData data = ItemRegistry.GetDataOrErrorItem(targetFish);
                        Texture2D texture = data.GetTexture();
                        Rectangle rect = data.GetSourceRect();

                        // Draw mini fish icon in the center of the species chum basket
                        float miniScale = 2.2f * scaleSize;
                        Vector2 center = location + new Vector2(32f, 32f);

                        spriteBatch.Draw(
                            texture,
                            center,
                            rect,
                            color * transparency,
                            0f,
                            new Vector2(rect.Width / 2f, rect.Height / 2f),
                            miniScale,
                            SpriteEffects.None,
                            layerDepth + 0.0001f
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Object.drawInMenu patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }

        [HarmonyPatch(nameof(StardewValley.Object.drawWhenHeld))]
        [HarmonyPostfix]
        public static void DrawWhenHeld_Postfix(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f, StardewValley.Object __instance)
        {
            try
            {
                if (__instance.QualifiedItemId == AssetService.QualifiedSpeciesChumId)
                {
                    if (__instance.modData.TryGetValue("ChumFrenzy.TargetFish", out string? targetFish) && !string.IsNullOrEmpty(targetFish))
                    {
                        ParsedItemData data = ItemRegistry.GetDataOrErrorItem(targetFish);
                        Texture2D texture = data.GetTexture();
                        Rectangle rect = data.GetSourceRect();
                        float layerDepth = Math.Max(0f, (float)(f.StandingPixel.Y + 4) / 10000f);

                        // Draw mini fish icon held
                        Vector2 center = objectPosition + new Vector2(32f, 32f);
                        spriteBatch.Draw(
                            texture,
                            center,
                            rect,
                            Color.White,
                            0f,
                            new Vector2(rect.Width / 2f, rect.Height / 2f),
                            2.0f,
                            SpriteEffects.None,
                            layerDepth + 0.0001f
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error in Object.drawWhenHeld patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
