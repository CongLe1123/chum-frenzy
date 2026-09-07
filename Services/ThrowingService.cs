using System;
using System.Collections.Generic;
using ChumFrenzy.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace ChumFrenzy.Services
{
    public class ThrownChumProjectile
    {
        public Vector2 StartPos { get; set; }
        public Vector2 TargetPos { get; set; }
        public float ElapsedTime { get; set; }
        public float TotalDuration { get; set; } = 0.45f;
        public float ArcHeight { get; set; } = 48f;
        public HotspotType Type { get; set; }
        public string? TargetFishQualifiedId { get; set; }
        public Farmer Thrower { get; set; }
        public GameLocation Location { get; set; }
        public Vector2 TargetTile { get; set; }
        public bool Finished { get; set; }

        public ThrownChumProjectile(Farmer thrower, GameLocation location, Vector2 startPos, Vector2 targetPos, Vector2 targetTile, HotspotType type, string? targetFishQualifiedId)
        {
            this.Thrower = thrower;
            this.Location = location;
            this.StartPos = startPos;
            this.TargetPos = targetPos;
            this.TargetTile = targetTile;
            this.Type = type;
            this.TargetFishQualifiedId = targetFishQualifiedId;
        }

        public void Update(float deltaSeconds, Action<ThrownChumProjectile> onImpact)
        {
            this.ElapsedTime += deltaSeconds;
            if (this.ElapsedTime >= this.TotalDuration && !this.Finished)
            {
                this.Finished = true;
                onImpact(this);
            }
        }

        public Vector2 GetCurrentPosition()
        {
            float t = Math.Clamp(this.ElapsedTime / this.TotalDuration, 0f, 1f);
            Vector2 linear = Vector2.Lerp(this.StartPos, this.TargetPos, t);
            float height = 4f * this.ArcHeight * t * (1f - t);
            return new Vector2(linear.X, linear.Y - height);
        }
    }

    public class FloatingFishSplashIcon
    {
        public Vector2 Position { get; set; }
        public float ElapsedSeconds { get; set; }
        public float TotalDuration { get; set; } = 1.5f;
        public string FishQualifiedId { get; set; }
        public GameLocation Location { get; set; }

        public FloatingFishSplashIcon(Vector2 position, string fishQualifiedId, GameLocation location)
        {
            this.Position = position;
            this.FishQualifiedId = fishQualifiedId;
            this.Location = location;
        }

        public bool Update(float deltaSeconds)
        {
            this.ElapsedSeconds += deltaSeconds;
            this.Position = new Vector2(this.Position.X, this.Position.Y - 20f * deltaSeconds);
            return this.ElapsedSeconds < this.TotalDuration;
        }
    }

    public class ThrowingService
    {
        private readonly IModHelper helper;
        private readonly HotspotManager hotspotManager;
        private readonly MultiplayerSyncService multiplayerSync;
        private readonly List<ThrownChumProjectile> activeProjectiles = new List<ThrownChumProjectile>();
        private readonly List<FloatingFishSplashIcon> activeFloatingIcons = new List<FloatingFishSplashIcon>();
        private Texture2D? projectileTexture;

        public ThrowingService(IModHelper helper, HotspotManager hotspotManager, MultiplayerSyncService multiplayerSync)
        {
            this.helper = helper;
            this.hotspotManager = hotspotManager;
            this.multiplayerSync = multiplayerSync;
        }

        public void SetProjectileTexture(Texture2D texture)
        {
            this.projectileTexture = texture;
        }

        public static bool IsValidChumTile(GameLocation location, Vector2 tile)
        {
            if (location == null)
                return false;

            if (!location.canFishHere())
                return false;

            if (location is BathHousePool)
                return false;

            int tileX = (int)tile.X;
            int tileY = (int)tile.Y;

            // Reject fish ponds
            if (location.isTileBuildingFishable(tileX, tileY))
                return false;

            // Must be fishable water tile
            if (!location.isTileFishable(tileX, tileY))
                return false;

            // Check NoFishing property
            if (location.doesTileHaveProperty(tileX, tileY, "NoFishing", "Back") != null)
                return false;

            return true;
        }

        public Vector2? FindTargetWaterTile(Farmer player, ICursorPosition cursor)
        {
            var location = player.currentLocation;
            if (location == null)
                return null;

            // 1. Mouse aiming: Check cursor tile if within range
            Vector2 cursorTile = cursor.GrabTile;
            if (Vector2.Distance(player.Tile, cursorTile) <= 5.5f && IsValidChumTile(location, cursorTile))
            {
                return cursorTile;
            }

            // 2. Controller / Facing direction raycast & cone sweep
            Vector2 forward = player.FacingDirection switch
            {
                0 => new Vector2(0, -1),
                1 => new Vector2(1, 0),
                2 => new Vector2(0, 1),
                3 => new Vector2(-1, 0),
                _ => Vector2.Zero
            };

            Vector2 right = new Vector2(-forward.Y, forward.X);

            // Search straight forward first, up to 5 tiles
            for (int d = 1; d <= 5; d++)
            {
                Vector2 target = player.Tile + forward * d;
                if (IsValidChumTile(location, target))
                {
                    return target;
                }
            }

            // Search slight angle (cone) for controller comfort
            for (int d = 1; d <= 5; d++)
            {
                Vector2 leftTile = player.Tile + forward * d - right;
                if (IsValidChumTile(location, leftTile))
                    return leftTile;

                Vector2 rightTile = player.Tile + forward * d + right;
                if (IsValidChumTile(location, rightTile))
                    return rightTile;
            }

            return null;
        }

        public bool TryThrowChum(Farmer player, ICursorPosition cursor)
        {
            Item? heldItem = player.ActiveItem;
            if (heldItem == null || !heldItem.QualifiedItemId.StartsWith("(O)ChumFrenzy_"))
                return false;

            var location = player.currentLocation;
            if (location == null)
                return false;

            HotspotType type;
            string? targetFishId = null;

            switch (heldItem.QualifiedItemId)
            {
                case AssetService.QualifiedChumId:
                    type = HotspotType.Basic;
                    break;
                case AssetService.QualifiedFrenzyChumId:
                    type = HotspotType.Frenzy;
                    break;
                case AssetService.QualifiedDeluxeFrenzyChumId:
                    type = HotspotType.DeluxeFrenzy;
                    break;
                case AssetService.QualifiedSpeciesChumId:
                    type = HotspotType.Species;
                    heldItem.modData.TryGetValue("ChumFrenzy.TargetFish", out targetFishId);
                    break;
                default:
                    return false;
            }

            Vector2? targetTile = this.FindTargetWaterTile(player, cursor);
            if (!targetTile.HasValue)
            {
                // Invalid water feedback
                location.playSound("cancel");
                Game1.showRedMessage(this.helper.Translation.Get("message.no_fish_here"));
                return false;
            }

            // Target is valid! Start throwing sequence
            Vector2 targetWorld = new Vector2(targetTile.Value.X * 64f + 32f, targetTile.Value.Y * 64f + 32f);
            Vector2 startWorld = player.Position + new Vector2(32f, -16f);

            // Animate thrower
            player.faceGeneralDirection(targetWorld);
            player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
            {
                new FarmerSprite.AnimationFrame(54, 80),
                new FarmerSprite.AnimationFrame(55, 80),
                new FarmerSprite.AnimationFrame(56, 120)
            });

            location.playSound("throw");

            var projectile = new ThrownChumProjectile(player, location, startWorld, targetWorld, targetTile.Value, type, targetFishId);
            this.activeProjectiles.Add(projectile);

            return true;
        }

        public void Update(float deltaSeconds)
        {
            for (int i = this.activeProjectiles.Count - 1; i >= 0; i--)
            {
                var p = this.activeProjectiles[i];
                p.Update(deltaSeconds, this.OnProjectileImpact);
                if (p.Finished)
                {
                    this.activeProjectiles.RemoveAt(i);
                }
            }

            for (int i = this.activeFloatingIcons.Count - 1; i >= 0; i--)
            {
                var icon = this.activeFloatingIcons[i];
                if (!icon.Update(deltaSeconds))
                {
                    this.activeFloatingIcons.RemoveAt(i);
                }
            }
        }

        private void OnProjectileImpact(ThrownChumProjectile projectile)
        {
            var location = projectile.Location;
            Vector2 targetWorld = projectile.TargetPos;

            // Play water splash sound
            location.playSound("waterSlosh");

            // Vanilla splash particle effect
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(28, targetWorld + new Vector2(-16f, -32f), Color.White, 8, false, 80f)
            {
                scale = 1.25f,
                layerDepth = 0.999f
            });

            // If Species Chum: float target fish icon
            if (projectile.Type == HotspotType.Species && !string.IsNullOrEmpty(projectile.TargetFishQualifiedId))
            {
                this.activeFloatingIcons.Add(new FloatingFishSplashIcon(targetWorld + new Vector2(-16f, -48f), projectile.TargetFishQualifiedId, location));
            }

            // Create hotspot or request host
            if (Context.IsMainPlayer)
            {
                // Host / Single Player
                var config = ModEntry.Config;
                int duration;
                float radius;

                switch (projectile.Type)
                {
                    case HotspotType.Basic:
                        duration = config.BasicChumDuration;
                        radius = config.BasicChumRadius;
                        break;
                    case HotspotType.Frenzy:
                        duration = config.FrenzyDuration;
                        radius = config.FrenzyRadius;
                        break;
                    case HotspotType.Species:
                        duration = config.SpeciesChumDuration;
                        radius = config.SpeciesChumRadius;
                        break;
                    case HotspotType.DeluxeFrenzy:
                        duration = config.DeluxeFrenzyDuration;
                        radius = config.DeluxeFrenzyRadius;
                        break;
                    default:
                        duration = 60;
                        radius = 2f;
                        break;
                }

                var hotspot = new ChumHotspot(
                    Guid.NewGuid(),
                    location.NameOrUniqueName,
                    projectile.TargetTile,
                    radius,
                    projectile.Type,
                    duration,
                    projectile.TargetFishQualifiedId,
                    projectile.Thrower.UniqueMultiplayerID
                );

                this.hotspotManager.AddHotspot(hotspot);
                this.multiplayerSync.BroadcastHotspotCreated(hotspot);

                // Consume 1 item from thrower's inventory
                ConsumeOneActiveItem(projectile.Thrower);
            }
            else
            {
                // Client in multiplayer: Request host to create
                this.multiplayerSync.SendThrowRequest(
                    location.NameOrUniqueName,
                    projectile.TargetTile,
                    projectile.Type,
                    projectile.TargetFishQualifiedId,
                    projectile.Thrower.UniqueMultiplayerID
                );
            }
        }

        public static void ConsumeOneActiveItem(Farmer farmer)
        {
            if (farmer.ActiveItem != null && farmer.ActiveItem.QualifiedItemId.StartsWith("(O)ChumFrenzy_"))
            {
                farmer.reduceActiveItemByOne();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw projectiles
            if (this.projectileTexture != null)
            {
                foreach (var p in this.activeProjectiles)
                {
                    if (p.Location == Game1.currentLocation)
                    {
                        Vector2 screenPos = Game1.GlobalToLocal(Game1.viewport, p.GetCurrentPosition());
                        spriteBatch.Draw(this.projectileTexture, screenPos, new Rectangle(0, 0, 16, 16), Color.White, p.ElapsedTime * 10f, new Vector2(8f, 8f), 3f, SpriteEffects.None, 0.95f);
                    }
                }
            }

            // Draw floating fish splash icons
            foreach (var icon in this.activeFloatingIcons)
            {
                if (icon.Location == Game1.currentLocation)
                {
                    var data = ItemRegistry.GetDataOrErrorItem(icon.FishQualifiedId);
                    Texture2D texture = data.GetTexture();
                    Rectangle rect = data.GetSourceRect();
                    float alpha = Math.Clamp(1f - (icon.ElapsedSeconds / icon.TotalDuration), 0f, 1f);
                    Vector2 screenPos = Game1.GlobalToLocal(Game1.viewport, icon.Position);

                    spriteBatch.Draw(texture, screenPos, rect, Color.White * alpha, 0f, new Vector2(rect.Width / 2f, rect.Height / 2f), 2.5f, SpriteEffects.None, 0.9999f);
                }
            }
        }
    }
}
