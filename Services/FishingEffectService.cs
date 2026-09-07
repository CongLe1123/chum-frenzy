using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ChumFrenzy.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Tools;

namespace ChumFrenzy.Services
{
    public class FishingEffectService
    {
        private readonly HotspotManager hotspotManager;
        private static MethodInfo? checkGenericFishRequirementsMethod;
        private static MethodInfo? getFishFromLocationDataMethod;

        public FishingEffectService(HotspotManager hotspotManager)
        {
            this.hotspotManager = hotspotManager;

            checkGenericFishRequirementsMethod = typeof(GameLocation).GetMethod(
                "CheckGenericFishRequirements",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            getFishFromLocationDataMethod = typeof(GameLocation).GetMethod(
                "GetFishFromLocationData",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[] { typeof(string), typeof(Vector2), typeof(int), typeof(Farmer), typeof(bool), typeof(bool), typeof(GameLocation), typeof(ItemQueryContext) },
                null
            );
        }

        public float ApplyBiteDelayReduction(Vector2 bobberTile, Farmer who, float originalDelay)
        {
            var hotspots = this.hotspotManager.GetHotspotsAt(who.currentLocation, bobberTile);
            if (hotspots.Count == 0)
                return originalDelay;

            var config = ModEntry.Config;
            float strongestMultiplier = 1.0f;

            foreach (var h in hotspots)
            {
                float multiplier = h.Type switch
                {
                    HotspotType.Basic => config.BasicChumBiteMultiplier,
                    HotspotType.Frenzy => config.FrenzyBiteMultiplier,
                    HotspotType.DeluxeFrenzy => config.DeluxeFrenzyBiteMultiplier,
                    _ => 1.0f
                };

                if (multiplier < strongestMultiplier)
                {
                    strongestMultiplier = multiplier;
                }
            }

            float reducedDelay = originalDelay * strongestMultiplier;
            float minDelayMs = config.MinimumBiteDelay * 1000f;

            return Math.Max(reducedDelay, minDelayMs);
        }

        public static bool IsJunk(Item item)
        {
            if (item == null)
                return true;

            if (item.Category == StardewValley.Object.junkCategory)
                return true;

            string id = item.ItemId;
            return id is "167" or "168" or "169" or "170" or "171" or "172";
        }

        public Item ApplyCatchModifiers(GameLocation location, Vector2 bobberTile, Farmer who, Item initialCatch, int waterDepth)
        {
            var hotspots = this.hotspotManager.GetHotspotsAt(location, bobberTile);
            if (hotspots.Count == 0)
                return initialCatch;

            var config = ModEntry.Config;
            Item currentCatch = initialCatch;

            // 1. Trash Reduction (Frenzy & Deluxe Frenzy)
            if (IsJunk(currentCatch))
            {
                float strongestTrashMultiplier = 1.0f;
                foreach (var h in hotspots)
                {
                    float mult = h.Type switch
                    {
                        HotspotType.Frenzy => config.FrenzyTrashMultiplier,
                        HotspotType.DeluxeFrenzy => config.DeluxeFrenzyTrashMultiplier,
                        _ => 1.0f
                    };
                    if (mult < strongestTrashMultiplier)
                    {
                        strongestTrashMultiplier = mult;
                    }
                }

                if (strongestTrashMultiplier < 1.0f)
                {
                    // If random roll fails the trash multiplier, we avoid trash and re-roll for an eligible fish!
                    if (Game1.random.NextDouble() >= strongestTrashMultiplier)
                    {
                        Item? rerolledFish = this.RollFishFromLocation(location, bobberTile, waterDepth, who);
                        if (rerolledFish != null && !IsJunk(rerolledFish))
                        {
                            currentCatch = rerolledFish;
                        }
                    }
                }
            }

            // 2. Species Chum Weighting
            bool hasDeluxeFrenzy = hotspots.Any(h => h.Type == HotspotType.DeluxeFrenzy);
            var speciesTargets = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

            foreach (var h in hotspots)
            {
                if (h.Type == HotspotType.Species && !string.IsNullOrEmpty(h.TargetFishQualifiedId))
                {
                    float targetMultiplier = hasDeluxeFrenzy ? config.DeluxeSpeciesWeightMultiplier : config.SpeciesWeightMultiplier;

                    // Non-stacking for same species: keep highest multiplier
                    if (!speciesTargets.TryGetValue(h.TargetFishQualifiedId, out float existing) || targetMultiplier > existing)
                    {
                        speciesTargets[h.TargetFishQualifiedId] = targetMultiplier;
                    }
                }
            }

            if (speciesTargets.Count > 0)
            {
                foreach (var kvp in speciesTargets)
                {
                    string targetId = kvp.Key;
                    float weightMultiplier = kvp.Value;

                    // Already caught this target
                    if (currentCatch.QualifiedItemId.Equals(targetId, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Strictly check if the target fish is normally eligible right now!
                    if (this.TryResolveEligibleTargetFish(location, targetId, bobberTile, waterDepth, who, out Item? targetFishItem))
                    {
                        // Target fish IS eligible!
                        // Probability to select based on weight multiplier boost
                        double replaceChance = 1.0 - (1.0 / Math.Max(1.0, weightMultiplier));
                        if (Game1.random.NextDouble() < replaceChance && targetFishItem != null)
                        {
                            currentCatch = targetFishItem;
                            break;
                        }
                    }
                }
            }

            return currentCatch;
        }

        private Item? RollFishFromLocation(GameLocation location, Vector2 bobberTile, int waterDepth, Farmer who)
        {
            try
            {
                if (getFishFromLocationDataMethod != null)
                {
                    return (Item?)getFishFromLocationDataMethod.Invoke(null, new object?[]
                    {
                        location.Name,
                        bobberTile,
                        waterDepth,
                        who,
                        false,
                        false,
                        location,
                        null
                    });
                }
            }
            catch
            {
                // Fallback
            }

            return null;
        }

        public bool TryResolveEligibleTargetFish(GameLocation location, string targetQualifiedId, Vector2 bobberTile, int waterDepth, Farmer player, out Item? resolvedFish)
        {
            resolvedFish = null;

            LocationData? locData = location.GetData();
            Dictionary<string, string> allFishData = DataLoader.Fish(Game1.content);
            Season seasonForLocation = Game1.GetSeasonForLocation(location);

            if (!location.TryGetFishAreaForTile(bobberTile, out string? areaId, out var _))
            {
                areaId = null;
            }

            bool hasMagicBait = false;
            bool hasCuriosityLure = false;
            if (player.CurrentTool is FishingRod { isFishing: true } rod)
            {
                hasMagicBait = rod.HasMagicBait();
                hasCuriosityLure = rod.HasCuriosityLure();
            }

            Point tilePoint = player.TilePoint;
            var context = new ItemQueryContext(location, null, Game1.random, $"species chum check for '{targetQualifiedId}'");

            IEnumerable<SpawnFishData> spawns = Game1.locationData["Default"].Fish;
            if (locData?.Fish?.Count > 0)
            {
                spawns = spawns.Concat(locData.Fish);
            }

            foreach (var spawn in spawns)
            {
                // Must match target fish
                if (!spawn.ItemId.Equals(targetQualifiedId, StringComparison.OrdinalIgnoreCase) &&
                    !$"(O){spawn.ItemId}".Equals(targetQualifiedId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Check area
                if (spawn.FishAreaId != null && areaId != spawn.FishAreaId)
                    continue;

                // Check season
                if (spawn.Season.HasValue && !hasMagicBait && spawn.Season != seasonForLocation)
                    continue;

                // Check player position
                if (spawn.PlayerPosition.HasValue && !spawn.PlayerPosition.Value.Contains(tilePoint.X, tilePoint.Y))
                    continue;

                // Check bobber position
                if (spawn.BobberPosition.HasValue && !spawn.BobberPosition.Value.Contains((int)bobberTile.X, (int)bobberTile.Y))
                    continue;

                // Check fishing level
                if (player.FishingLevel < spawn.MinFishingLevel)
                    continue;

                // Check depth
                if (waterDepth < spawn.MinDistanceFromShore || (spawn.MaxDistanceFromShore > -1 && waterDepth > spawn.MaxDistanceFromShore))
                    continue;

                // Check magic bait requirement
                if (spawn.RequireMagicBait && !hasMagicBait)
                    continue;

                // Check condition GameStateQuery
                HashSet<string>? ignoreQueryKeys = hasMagicBait ? GameStateQuery.MagicBaitIgnoreQueryKeys : null;
                if (spawn.Condition != null && !GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, null, ignoreQueryKeys))
                    continue;

                // Resolve item candidate
                Item? candidate = ItemQueryResolver.TryResolveRandomItem(spawn, context, avoidRepeat: false, null, (string query) => query.Replace("BOBBER_X", ((int)bobberTile.X).ToString()).Replace("BOBBER_Y", ((int)bobberTile.Y).ToString()).Replace("WATER_DEPTH", waterDepth.ToString()));
                if (candidate == null)
                    continue;

                // Check catch limit
                if (spawn.CatchLimit > -1 && player.fishCaught.TryGetValue(candidate.QualifiedItemId, out var caught) && caught[0] >= spawn.CatchLimit)
                    continue;

                // Check generic fish requirements (time of day, weather, skill, chance)
                if (checkGenericFishRequirementsMethod != null)
                {
                    try
                    {
                        bool pass = (bool)checkGenericFishRequirementsMethod.Invoke(null, new object?[]
                        {
                            candidate,
                            allFishData,
                            location,
                            player,
                            spawn,
                            waterDepth,
                            hasMagicBait,
                            hasCuriosityLure,
                            false, // usingTargetBait
                            false  // isTutorialCatch
                        })!;

                        if (!pass)
                            continue;
                    }
                    catch
                    {
                        // If reflection check fails, do not allow bypass
                        continue;
                    }
                }

                // Attach boss fish / pickup flags if applicable
                if (!string.IsNullOrWhiteSpace(spawn.SetFlagOnCatch))
                {
                    candidate.SetFlagOnPickup = spawn.SetFlagOnCatch;
                }
                if (spawn.IsBossFish)
                {
                    candidate.SetTempData("IsBossFish", true);
                }

                resolvedFish = candidate;
                return true;
            }

            return false;
        }
    }
}
