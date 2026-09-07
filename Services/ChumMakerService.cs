using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Machines;

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

            return false;
        }

        public static Item? OutputSpeciesChum(
            StardewValley.Object machine,
            Item inputItem,
            bool probe,
            MachineItemOutput outputData,
            Farmer player,
            out int? overrideMinutesUntilReady)
        {
            overrideMinutesUntilReady = null;

            if (inputItem == null)
                return null;

            // Must be fish
            if (!inputItem.HasContextTag("category_fish"))
                return null;

            // Check legendary restriction
            bool allowLegendary = ModEntry.Config?.AllowLegendaryProcessing ?? false;
            if (IsLegendaryFish(inputItem) && !allowLegendary)
            {
                if (!probe && player?.IsLocalPlayer == true)
                {
                    string msg = ModEntry.Instance?.Helper?.Translation?.Get("message.chum_maker.legendary_rejected") ?? "Legendary fish cannot be processed into chum.";
                    Game1.showRedMessage(msg);
                    (machine.Location ?? Game1.currentLocation)?.playSound("cancel");
                }
                return null;
            }

            // Create 2 Species Chum
            Item output = ItemRegistry.Create(AssetService.QualifiedSpeciesChumId, 2);
            output.modData["ChumFrenzy.ChumType"] = "Species";
            output.modData["ChumFrenzy.TargetFish"] = inputItem.QualifiedItemId;

            if (output is StardewValley.Object obj)
            {
                int targetFishPrice = (inputItem as StardewValley.Object)?.Price ?? 20;
                obj.Price = Math.Clamp((int)(targetFishPrice * 0.10f) + 20, 20, 250);
            }

            return output;
        }
    }
}
