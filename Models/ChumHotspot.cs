using System;
using Microsoft.Xna.Framework;

namespace ChumFrenzy.Models
{
    public class ChumHotspot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LocationName { get; set; } = string.Empty;
        public Vector2 CenterTile { get; set; }
        public float Radius { get; set; }
        public HotspotType Type { get; set; }
        public string? TargetFishQualifiedId { get; set; }
        public int RemainingMinutes { get; set; }
        public long CreatorPlayerId { get; set; }

        public ChumHotspot()
        {
        }

        public ChumHotspot(Guid id, string locationName, Vector2 centerTile, float radius, HotspotType type, int durationMinutes, string? targetFishQualifiedId = null, long creatorPlayerId = 0)
        {
            this.Id = id;
            this.LocationName = locationName;
            this.CenterTile = centerTile;
            this.Radius = radius;
            this.Type = type;
            this.RemainingMinutes = durationMinutes;
            this.TargetFishQualifiedId = targetFishQualifiedId;
            this.CreatorPlayerId = creatorPlayerId;
        }

        public bool Contains(Vector2 tile)
        {
            return Vector2.Distance(this.CenterTile, tile) <= this.Radius;
        }
    }
}
