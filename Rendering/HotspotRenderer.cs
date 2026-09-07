using System;
using System.Collections.Generic;
using ChumFrenzy.Models;
using ChumFrenzy.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ChumFrenzy.Rendering
{
    public class HotspotRenderer
    {
        private readonly HotspotManager hotspotManager;
        private readonly Dictionary<Guid, TemporaryAnimatedSprite> secondaryBubbleSprites = new Dictionary<Guid, TemporaryAnimatedSprite>();

        public HotspotRenderer(HotspotManager hotspotManager)
        {
            this.hotspotManager = hotspotManager;
            this.hotspotManager.HotspotRemoved += this.OnHotspotRemoved;
        }

        private void OnHotspotRemoved(Guid id, string locationName)
        {
            this.secondaryBubbleSprites.Remove(id);
        }

        public void Update(GameTime time)
        {
            var loc = Game1.currentLocation;
            if (loc == null)
                return;

            var hotspots = this.hotspotManager.GetHotspots(loc.NameOrUniqueName);
            if (hotspots.Count <= 1)
            {
                this.secondaryBubbleSprites.Clear();
                return;
            }

            Point primaryPoint = loc.fishSplashPoint.Value;

            foreach (var h in hotspots)
            {
                Point hPoint = new Point((int)h.CenterTile.X, (int)h.CenterTile.Y);
                if (hPoint == primaryPoint)
                    continue;

                if (!this.secondaryBubbleSprites.TryGetValue(h.Id, out var sprite))
                {
                    // Sprite 51 is Stardew Valley's official native fishing bubble animation
                    sprite = new TemporaryAnimatedSprite(51, new Vector2(h.CenterTile.X * 64f, h.CenterTile.Y * 64f), Color.White, 10, flipped: false, 80f, 999999)
                    {
                        layerDepth = (float)(h.CenterTile.Y * 64f - 64f - 1f) / 10000f
                    };
                    this.secondaryBubbleSprites[h.Id] = sprite;
                }

                sprite.update(time);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var loc = Game1.currentLocation;
            if (loc == null)
                return;

            foreach (var kvp in this.secondaryBubbleSprites)
            {
                kvp.Value.draw(spriteBatch);
            }
        }
    }
}
