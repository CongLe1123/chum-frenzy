using System;
using ChumFrenzy.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ChumFrenzy.Services
{
    public class MultiplayerSyncService
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly HotspotManager hotspotManager;

        public MultiplayerSyncService(IModHelper helper, IMonitor monitor, HotspotManager hotspotManager)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.hotspotManager = hotspotManager;
        }

        public void SendThrowRequest(string locationName, Vector2 targetTile, HotspotType type, string? targetFishId, long playerId)
        {
            var msg = new ThrowChumRequestMessage
            {
                LocationName = locationName,
                TargetTile = targetTile,
                Type = type,
                TargetFishQualifiedId = targetFishId,
                PlayerId = playerId
            };

            this.helper.Multiplayer.SendMessage(msg, MessageTypes.ThrowChumRequest, new[] { this.helper.ModRegistry.ModID });
        }

        public void BroadcastHotspotCreated(ChumHotspot hotspot)
        {
            if (!Context.IsMainPlayer)
                return;

            var msg = new HotspotCreatedMessage { Hotspot = hotspot };
            this.helper.Multiplayer.SendMessage(msg, MessageTypes.HotspotCreated, new[] { this.helper.ModRegistry.ModID });
        }

        public void BroadcastHotspotRemoved(Guid id, string locationName)
        {
            if (!Context.IsMainPlayer)
                return;

            var msg = new HotspotRemovedMessage { HotspotId = id, LocationName = locationName };
            this.helper.Multiplayer.SendMessage(msg, MessageTypes.HotspotRemoved, new[] { this.helper.ModRegistry.ModID });
        }

        public void SendSyncToPlayer(long targetPlayerId)
        {
            if (!Context.IsMainPlayer)
                return;

            var msg = new SyncHotspotsMessage
            {
                Hotspots = this.hotspotManager.GetAllHotspots()
            };

            this.helper.Multiplayer.SendMessage(msg, MessageTypes.SyncHotspots, new[] { this.helper.ModRegistry.ModID }, new[] { targetPlayerId });
        }

        public void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.helper.ModRegistry.ModID)
                return;

            switch (e.Type)
            {
                case MessageTypes.ThrowChumRequest:
                    if (Context.IsMainPlayer)
                    {
                        var req = e.ReadAs<ThrowChumRequestMessage>();
                        var loc = Game1.getLocationFromName(req.LocationName);
                        if (loc != null && ThrowingService.IsValidChumTile(loc, req.TargetTile))
                        {
                            var config = ModEntry.Config;
                            int duration = req.Type switch
                            {
                                HotspotType.Basic => config.BasicChumDuration,
                                HotspotType.Frenzy => config.FrenzyDuration,
                                HotspotType.Species => config.SpeciesChumDuration,
                                HotspotType.DeluxeFrenzy => config.DeluxeFrenzyDuration,
                                _ => 60
                            };
                            float radius = req.Type switch
                            {
                                HotspotType.Basic => config.BasicChumRadius,
                                HotspotType.Frenzy => config.FrenzyRadius,
                                HotspotType.Species => config.SpeciesChumRadius,
                                HotspotType.DeluxeFrenzy => config.DeluxeFrenzyRadius,
                                _ => 2f
                            };

                            var hotspot = new ChumHotspot(Guid.NewGuid(), req.LocationName, req.TargetTile, radius, req.Type, duration, req.TargetFishQualifiedId, req.PlayerId);
                            this.hotspotManager.AddHotspot(hotspot);
                            this.BroadcastHotspotCreated(hotspot);

                            // Confirm to client
                            var confirm = new ThrowChumConfirmedMessage { HotspotId = hotspot.Id, PlayerId = req.PlayerId };
                            this.helper.Multiplayer.SendMessage(confirm, MessageTypes.ThrowChumConfirmed, new[] { this.helper.ModRegistry.ModID }, new[] { req.PlayerId });
                        }
                        else
                        {
                            var reject = new ThrowChumRejectedMessage { PlayerId = req.PlayerId, Reason = "Invalid location or tile" };
                            this.helper.Multiplayer.SendMessage(reject, MessageTypes.ThrowChumRejected, new[] { this.helper.ModRegistry.ModID }, new[] { req.PlayerId });
                        }
                    }
                    break;

                case MessageTypes.ThrowChumConfirmed:
                    {
                        var confirm = e.ReadAs<ThrowChumConfirmedMessage>();
                        if (confirm.PlayerId == Game1.player.UniqueMultiplayerID)
                        {
                            ThrowingService.ConsumeOneActiveItem(Game1.player);
                        }
                    }
                    break;

                case MessageTypes.ThrowChumRejected:
                    {
                        var reject = e.ReadAs<ThrowChumRejectedMessage>();
                        if (reject.PlayerId == Game1.player.UniqueMultiplayerID)
                        {
                            Game1.playSound("cancel");
                            Game1.showRedMessage(this.helper.Translation.Get("message.no_fish_here"));
                        }
                    }
                    break;

                case MessageTypes.HotspotCreated:
                    if (!Context.IsMainPlayer)
                    {
                        var created = e.ReadAs<HotspotCreatedMessage>();
                        this.hotspotManager.AddHotspot(created.Hotspot);
                    }
                    break;

                case MessageTypes.HotspotRemoved:
                    if (!Context.IsMainPlayer)
                    {
                        var removed = e.ReadAs<HotspotRemovedMessage>();
                        this.hotspotManager.RemoveHotspot(removed.HotspotId, removed.LocationName);
                    }
                    break;

                case MessageTypes.SyncHotspots:
                    if (!Context.IsMainPlayer)
                    {
                        var sync = e.ReadAs<SyncHotspotsMessage>();
                        this.hotspotManager.SetAllHotspots(sync.Hotspots);
                    }
                    break;
            }
        }
    }
}
