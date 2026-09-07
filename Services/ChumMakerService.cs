using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace ChumFrenzy.Services
{
    public class ChumMakerService
    {
        private static readonly HashSet<string> LegendaryFishIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "159", // Crimsonfish
            "160", // Angler
            "163", // Legend
            "682", // Mutant Carp
            "775", // Glacierfish
            "898", // Son of Crimsonfish
            "899", // Ms. Angler
            "900", // Legend II
            "901", // Radioactive Carp
            "902"  // Glacierfish Jr.
        };

        public static bool IsLegendaryFish(Item? item)
        {
            if (item == null)
                return false;

            if (item.HasContextTag("fish_legendary"))
                return true;

            string itemId = item.ItemId;
            if (LegendaryFishIds.Contains(itemId))
                return true;

            // Check Data/Fish for boss fish flag
            try
            {
                var fishData = DataLoader.Fish(Game1.content);
                if (fishData != null && fishData.TryGetValue(itemId, out var rawData))
                {
                    string[] parts = rawData.Split('/');
                    if (parts.Length > 1 && parts[1].Equals("trap", StringComparison.OrdinalIgnoreCase))
                        return false;
                    // Check if is boss fish or special catch
                    if (parts.Length > 8 && parts[8].Equals("true", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            catch
            {
                // Fallback safe
            }

            return false;
        }

        public static Item? OutputSpeciesChum(StardewValley.Object machine, GameLocation location, Farmer player, Item? inputItem, bool probe)
        {
            if (inputItem == null)
                return null;

            // Must be fish
            if (!inputItem.HasContextTag("category_fish"))
                return null;

            // Check legendary restriction
            if (IsLegendaryFish(inputItem) && !ModEntry.Config.AllowLegendaryProcessing)
            {
                if (!probe && player?.IsLocalPlayer == true)
                {
                    Game1.showRedMessage(ModEntry.Instance.Helper.Translation.Get("message.chum_maker.legendary_rejected"));
                    location.playSound("cancel");
                }
                return null;
            }

            // Create 2 Species Chum
            Item output = ItemRegistry.Create(AssetService.QualifiedSpeciesChumId, 2);
            output.modData["ChumFrenzy.ChumType"] = "Species";
            output.modData["ChumFrenzy.TargetFish"] = inputItem.QualifiedItemId;

            if (output is StardewValley.Object obj)
            {
                int targetFishPrice = inputItem.salePrice(false);
                obj.Price = Math.Clamp((int)(targetFishPrice * 0.10f) + 20, 20, 250);
            }

            return output;
        }
    }
}
