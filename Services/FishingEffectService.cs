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
using StardewValley.Locations;
using StardewValley.Tools;

namespace ChumFrenzy.Services
{
    public class FishingEffectService
    {
        private readonly HotspotManager hotspotManager;
        private static MethodInfo? getFishFromLocationDataMethod;

        public FishingEffectService(HotspotManager hotspotManager)
        {
            this.hotspotManager = hotspotManager;

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

        public static bool IsJunk(Item? item)
        {
            if (item == null)
                return true;

            if (item.Category == StardewValley.Object.junkCategory)
                return true;

            string id = item.ItemId;
            return id is "167" or "168" or "169" or "170" or "171" or "172";
        }

        public Item? ApplyCatchModifiers(GameLocation location, Vector2 bobberTile, Farmer who, Item? initialCatch, int waterDepth)
        {
            var hotspots = this.hotspotManager.GetHotspotsAt(location, bobberTile);
            if (hotspots.Count == 0)
                return initialCatch;

            var config = ModEntry.Config;
            Item? currentCatch = initialCatch;

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
                    if (currentCatch != null && currentCatch.QualifiedItemId.Equals(targetId, StringComparison.OrdinalIgnoreCase))
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

        private static Season SafeGetSeasonForLocation(GameLocation? location)
        {
            if (location == null)
                return Game1.season;
            try
            {
                return Game1.GetSeasonForLocation(location);
            }
            catch
            {
                return Game1.season;
            }
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

        public bool TryResolveEligibleTargetFish(GameLocation? location, string? targetQualifiedId, Vector2 bobberTile, int waterDepth, Farmer? player, out Item? resolvedFish)
        {
            resolvedFish = null;
            if (location == null || string.IsNullOrEmpty(targetQualifiedId))
                return false;

            LocationData? locData = location.GetData();
            Dictionary<string, string>? allFishData = DataLoader.Fish(Game1.content);
            Season seasonForLocation = SafeGetSeasonForLocation(location);

            if (!location.TryGetFishAreaForTile(bobberTile, out string? areaId, out var _))
            {
                areaId = null;
            }

            bool hasMagicBait = false;
            bool hasCuriosityLure = false;
            if (player?.CurrentTool is FishingRod { isFishing: true } rod)
            {
                hasMagicBait = rod.HasMagicBait();
                hasCuriosityLure = rod.HasCuriosityLure();
            }

            Point tilePoint = player?.TilePoint ?? new Point((int)bobberTile.X, (int)bobberTile.Y);
            int fishingLevel = player?.FishingLevel ?? 0;
            var context = new ItemQueryContext(location, player, Game1.random, $"species chum check for '{targetQualifiedId}'");

            var spawnsList = new List<SpawnFishData>();
            if (Game1.locationData != null && Game1.locationData.TryGetValue("Default", out var defaultLocData) && defaultLocData?.Fish != null)
            {
                spawnsList.AddRange(defaultLocData.Fish.Where(f => f != null));
            }

            if (locData?.Fish != null)
            {
                foreach (var spawn in locData.Fish)
                {
                    if (spawn == null)
                        continue;

                    if (!string.IsNullOrEmpty(spawn.ItemId) && spawn.ItemId.StartsWith("LOCATION_FISH ", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] parts = spawn.ItemId.Split(' ');
                        if (parts.Length > 1 && Game1.locationData != null && Game1.locationData.TryGetValue(parts[1], out var refLoc) && refLoc?.Fish != null)
                        {
                            spawnsList.AddRange(refLoc.Fish.Where(f => f != null));
                        }
                    }
                    else
                    {
                        spawnsList.Add(spawn);
                    }
                }
            }

            if (location is MineShaft mine)
            {
                if (mine.mineLevel == 100)
                {
                    spawnsList.Add(new SpawnFishData { ItemId = "(O)162" }); // Lava Eel
                }
                else if (mine.mineLevel == 60)
                {
                    spawnsList.Add(new SpawnFishData { ItemId = "(O)161" }); // Ice Pip
                }
                else if (mine.mineLevel == 20)
                {
                    spawnsList.Add(new SpawnFishData { ItemId = "(O)158" }); // Stonefish
                }
            }

            IEnumerable<SpawnFishData> spawns = spawnsList;

            foreach (var spawn in spawns)
            {
                if (spawn == null || string.IsNullOrEmpty(spawn.ItemId))
                    continue;

                // Must match target fish
                string qualifiedSpawnId = ItemRegistry.QualifyItemId(spawn.ItemId) ?? spawn.ItemId;
                if (!string.Equals(qualifiedSpawnId, targetQualifiedId, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(spawn.ItemId, targetQualifiedId, StringComparison.OrdinalIgnoreCase))
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
                if (fishingLevel < spawn.MinFishingLevel)
                    continue;

                // Check depth
                if (waterDepth < spawn.MinDistanceFromShore || (spawn.MaxDistanceFromShore > -1 && waterDepth > spawn.MaxDistanceFromShore))
                    continue;

                // Check magic bait requirement
                if (spawn.RequireMagicBait && !hasMagicBait)
                    continue;

                // Check condition GameStateQuery
                HashSet<string>? ignoreQueryKeys = hasMagicBait ? GameStateQuery.MagicBaitIgnoreQueryKeys : null;
                if (spawn.Condition != null)
                {
                    try
                    {
                        if (!GameStateQuery.CheckConditions(spawn.Condition, location, player, null, null, null, ignoreQueryKeys))
                            continue;
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Resolve item candidate
                Item? candidate = null;
                try
                {
                    candidate = ItemQueryResolver.TryResolveRandomItem(spawn, context, avoidRepeat: false, null, (string query) => query.Replace("BOBBER_X", ((int)bobberTile.X).ToString()).Replace("BOBBER_Y", ((int)bobberTile.Y).ToString()).Replace("WATER_DEPTH", waterDepth.ToString()));
                }
                catch
                {
                    candidate = null;
                }

                if (candidate == null && !string.IsNullOrEmpty(targetQualifiedId))
                {
                    try
                    {
                        candidate = ItemRegistry.Create(targetQualifiedId);
                    }
                    catch
                    {
                        candidate = null;
                    }
                }

                if (candidate == null)
                    continue;

                // Check catch limit
                if (spawn.CatchLimit > -1 && player?.fishCaught != null && player.fishCaught.TryGetValue(candidate.QualifiedItemId, out var caught) && caught[0] >= spawn.CatchLimit)
                    continue;

                // Check generic fish requirements deterministically (time, weather, season, fishing level, training rod)
                if (!CheckFishEligibility(candidate, allFishData, location, player, hasMagicBait))
                    continue;

                // Attach boss fish / pickup flags if applicable
                try
                {
                    if (!string.IsNullOrWhiteSpace(spawn.SetFlagOnCatch))
                    {
                        candidate.SetFlagOnPickup = spawn.SetFlagOnCatch;
                    }
                    if (spawn.IsBossFish)
                    {
                        candidate.SetTempData("IsBossFish", true);
                    }
                }
                catch
                {
                    // Ignore flag attach failure
                }

                resolvedFish = candidate;
                return true;
            }

            return false;
        }

        private static bool CheckFishEligibility(Item candidate, Dictionary<string, string>? allFishData, GameLocation location, Farmer? player, bool hasMagicBait)
        {
            if (candidate == null)
                return false;

            if (allFishData == null || !allFishData.TryGetValue(candidate.ItemId, out string? rawData) || string.IsNullOrEmpty(rawData))
                return true;

            string[] parts = rawData.Split('/');
            if (parts.Length > 1 && parts[1].Equals("trap", StringComparison.OrdinalIgnoreCase))
                return false;

            if (player?.CurrentTool is FishingRod rod && rod.QualifiedItemId == "(T)TrainingRod")
            {
                if (int.TryParse(parts[1], out int difficulty) && difficulty >= 50)
                    return false;
            }

            if (player != null && parts.Length > 12 && int.TryParse(parts[12], out int minLevel))
            {
                if (player.FishingLevel < minLevel)
                    return false;
            }

            if (!hasMagicBait)
            {
                // Season check: field 6
                if (parts.Length > 6)
                {
                    string seasons = parts[6];
                    if (!seasons.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        string currentSeason = SafeGetSeasonForLocation(location).ToString().ToLowerInvariant();
                        if (!seasons.Split(' ').Any(s => s.Equals(currentSeason, StringComparison.OrdinalIgnoreCase)))
                            return false;
                    }
                }

                // Weather check: field 7 (sunny, rainy, both)
                if (parts.Length > 7)
                {
                    string weather = parts[7];
                    bool isRaining = false;
                    try
                    {
                        isRaining = location != null && location.IsRainingHere();
                    }
                    catch
                    {
                        isRaining = Game1.isRaining;
                    }

                    if (weather.Equals("sunny", StringComparison.OrdinalIgnoreCase) && isRaining)
                        return false;
                    if (weather.Equals("rainy", StringComparison.OrdinalIgnoreCase) && !isRaining)
                        return false;
                }

                // Time of day check: field 5
                if (parts.Length > 5)
                {
                    string[] timeTokens = parts[5].Split(' ');
                    bool timeValid = false;
                    for (int i = 0; i < timeTokens.Length - 1; i += 2)
                    {
                        if (int.TryParse(timeTokens[i], out int startTime) && int.TryParse(timeTokens[i + 1], out int endTime))
                        {
                            if (Game1.timeOfDay >= startTime && Game1.timeOfDay < endTime)
                            {
                                timeValid = true;
                                break;
                            }
                        }
                    }
                    if (!timeValid)
                        return false;
                }
            }

            return true;
        }
    }
}
