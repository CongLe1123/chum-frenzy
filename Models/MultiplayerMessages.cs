using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChumFrenzy.Models
{
    public static class MessageTypes
    {
        public const string ThrowChumRequest = "ThrowChumRequest";
        public const string ThrowChumConfirmed = "ThrowChumConfirmed";
        public const string ThrowChumRejected = "ThrowChumRejected";
        public const string HotspotCreated = "HotspotCreated";
        public const string HotspotRemoved = "HotspotRemoved";
        public const string SyncHotspots = "SyncHotspots";
    }

    public class ThrowChumRequestMessage
    {
        public string LocationName { get; set; } = string.Empty;
        public Vector2 TargetTile { get; set; }
        public HotspotType Type { get; set; }
        public string? TargetFishQualifiedId { get; set; }
        public long PlayerId { get; set; }
    }

    public class ThrowChumConfirmedMessage
    {
        public Guid HotspotId { get; set; }
        public long PlayerId { get; set; }
    }

    public class ThrowChumRejectedMessage
    {
        public long PlayerId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class HotspotCreatedMessage
    {
        public ChumHotspot Hotspot { get; set; } = null!;
    }

    public class HotspotRemovedMessage
    {
        public Guid HotspotId { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }

    public class SyncHotspotsMessage
    {
        public List<ChumHotspot> Hotspots { get; set; } = new List<ChumHotspot>();
    }
}
