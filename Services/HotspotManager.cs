using System;
using System.Collections.Generic;
using System.Linq;
using ChumFrenzy.Models;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ChumFrenzy.Services
{
    public class HotspotManager
    {
        private readonly Dictionary<string, List<ChumHotspot>> hotspotsByLocation = new Dictionary<string, List<ChumHotspot>>(StringComparer.OrdinalIgnoreCase);

        public event Action<ChumHotspot>? HotspotAdded;
        public event Action<Guid, string>? HotspotRemoved;

        public IReadOnlyList<ChumHotspot> GetHotspots(string locationName)
        {
            if (string.IsNullOrEmpty(locationName))
                return Array.Empty<ChumHotspot>();

            if (this.hotspotsByLocation.TryGetValue(locationName, out var list))
                return list;

            return Array.Empty<ChumHotspot>();
        }

        public static string GetLocationKey(GameLocation? location)
        {
            if (location == null)
                return string.Empty;

            try
            {
                return location.NameOrUniqueName ?? location.Name ?? string.Empty;
            }
            catch
            {
                try
                {
                    return location.Name ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public List<ChumHotspot> GetHotspotsAt(GameLocation? location, Vector2 tile)
        {
            var result = new List<ChumHotspot>();
            if (location == null)
                return result;

            string locName = GetLocationKey(location);
            if (string.IsNullOrEmpty(locName))
                return result;

            if (this.hotspotsByLocation.TryGetValue(locName, out var list))
            {
                foreach (var h in list)
                {
                    if (h.Contains(tile))
                    {
                        result.Add(h);
                    }
                }
            }

            return result;
        }

        public ChumHotspot AddHotspot(ChumHotspot hotspot)
        {
            if (!this.hotspotsByLocation.TryGetValue(hotspot.LocationName, out var list))
            {
                list = new List<ChumHotspot>();
                this.hotspotsByLocation[hotspot.LocationName] = list;
            }

            list.RemoveAll(h => h.Id == hotspot.Id);

            // Enforce max hotspots per location (FIFO)
            int max = ModEntry.Config.MaxHotspotsPerLocation;
            while (list.Count >= max && list.Count > 0)
            {
                var oldest = list[0];
                list.RemoveAt(0);
                this.HotspotRemoved?.Invoke(oldest.Id, oldest.LocationName);
            }

            list.Add(hotspot);
            this.HotspotAdded?.Invoke(hotspot);

            // Sync with vanilla bubble and frenzy system
            var loc = Game1.getLocationFromName(hotspot.LocationName);
            if (loc != null)
            {
                this.SyncLocationNativeFrenzy(loc);
            }

            return hotspot;
        }

        public bool RemoveHotspot(Guid id, string locationName)
        {
            if (this.hotspotsByLocation.TryGetValue(locationName, out var list))
            {
                int index = list.FindIndex(h => h.Id == id);
                if (index >= 0)
                {
                    list.RemoveAt(index);
                    this.HotspotRemoved?.Invoke(id, locationName);

                    var loc = Game1.getLocationFromName(locationName);
                    if (loc != null)
                    {
                        this.SyncLocationNativeFrenzy(loc);
                    }

                    return true;
                }
            }
            return false;
        }

        public void TickTenMinutes()
        {
            var toRemove = new List<ChumHotspot>();

            foreach (var kvp in this.hotspotsByLocation)
            {
                foreach (var hotspot in kvp.Value)
                {
                    hotspot.RemainingMinutes -= 10;
                    if (hotspot.RemainingMinutes <= 0)
                    {
                        toRemove.Add(hotspot);
                    }
                }
            }

            foreach (var expired in toRemove)
            {
                this.RemoveHotspot(expired.Id, expired.LocationName);
            }
        }

        public void ClearAll()
        {
            foreach (var kvp in this.hotspotsByLocation)
            {
                foreach (var h in kvp.Value)
                {
                    this.HotspotRemoved?.Invoke(h.Id, h.LocationName);
                }

                var loc = Game1.getLocationFromName(kvp.Key);
                if (loc != null)
                {
                    loc.fishSplashPoint.Value = Point.Zero;
                    loc.fishFrenzyFish.Value = "";
                }
            }
            this.hotspotsByLocation.Clear();
        }

        public List<ChumHotspot> GetAllHotspots()
        {
            var all = new List<ChumHotspot>();
            foreach (var kvp in this.hotspotsByLocation)
            {
                all.AddRange(kvp.Value);
            }
            return all;
        }

        public void SetAllHotspots(IEnumerable<ChumHotspot> hotspots)
        {
            this.ClearAll();
            foreach (var h in hotspots)
            {
                if (!this.hotspotsByLocation.TryGetValue(h.LocationName, out var list))
                {
                    list = new List<ChumHotspot>();
                    this.hotspotsByLocation[h.LocationName] = list;
                }
                list.Add(h);
                this.HotspotAdded?.Invoke(h);

                var loc = Game1.getLocationFromName(h.LocationName);
                if (loc != null)
                {
                    this.SyncLocationNativeFrenzy(loc);
                }
            }
        }

        public void SyncLocationNativeFrenzy(GameLocation? location)
        {
            if (location == null)
                return;

            string locName = GetLocationKey(location);
            if (string.IsNullOrEmpty(locName))
                return;

            var hotspots = this.GetHotspots(locName);
            if (hotspots.Count == 0)
            {
                if (location.fishSplashPoint != null && !location.fishSplashPoint.Value.Equals(Point.Zero))
                {
                    location.fishSplashPoint.Value = Point.Zero;
                    if (location.fishFrenzyFish != null)
                    {
                        location.fishFrenzyFish.Value = "";
                    }
                }
                return;
            }

            // Select primary hotspot (prefer Frenzy or Species over Basic)
            var primary = hotspots
                .OrderByDescending(h => h.Type is HotspotType.DeluxeFrenzy or HotspotType.Frenzy ? 2 : (h.Type == HotspotType.Species ? 1 : 0))
                .FirstOrDefault();

            if (primary == null)
                return;

            if (location.fishSplashPoint != null)
            {
                location.fishSplashPoint.Value = new Point((int)primary.CenterTile.X, (int)primary.CenterTile.Y);
            }

            if (primary.Type == HotspotType.Species && !string.IsNullOrEmpty(primary.TargetFishQualifiedId))
            {
                if (ModEntry.FishingEffects != null && ModEntry.FishingEffects.TryResolveEligibleTargetFish(location, primary.TargetFishQualifiedId, primary.CenterTile, 3, Game1.player, out _))
                {
                    if (location.fishFrenzyFish != null)
                        location.fishFrenzyFish.Value = primary.TargetFishQualifiedId;
                }
                else
                {
                    if (location.fishFrenzyFish != null)
                        location.fishFrenzyFish.Value = "";
                }
            }
            else if (primary.Type is HotspotType.Frenzy or HotspotType.DeluxeFrenzy)
            {
                // Select eligible fish from location to frenzy
                Item? fishItem = null;
                try
                {
                    fishItem = location.getFish(0f, "", 3, Game1.player, 0.0, primary.CenterTile);
                }
                catch
                {
                    fishItem = null;
                }

                if (fishItem != null && fishItem.Category == -4 && !fishItem.HasContextTag("fish_legendary"))
                {
                    if (location.fishFrenzyFish != null)
                        location.fishFrenzyFish.Value = fishItem.QualifiedItemId;
                }
                else
                {
                    if (location.fishFrenzyFish != null)
                        location.fishFrenzyFish.Value = "";
                }
            }
            else
            {
                // Basic Chum = standard fishing bubble hotspot
                if (location.fishFrenzyFish != null)
                    location.fishFrenzyFish.Value = "";
            }
        }
    }
}
