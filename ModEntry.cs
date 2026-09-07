using System;
using ChumFrenzy.Compatibility;
using ChumFrenzy.Services;
using ChumFrenzy.Rendering;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ChumFrenzy
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; } = null!;
        public static ModConfig Config { get; private set; } = null!;
        public static HotspotManager Hotspots { get; private set; } = null!;
        public static FishingEffectService FishingEffects { get; private set; } = null!;
        public static ThrowingService Throwing { get; private set; } = null!;
        public static MultiplayerSyncService MultiplayerSync { get; private set; } = null!;
        public static SaveDataService SaveData { get; private set; } = null!;
        public static AssetService Assets { get; private set; } = null!;
        public static HotspotRenderer Renderer { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();
            Config.Sanitize();

            Hotspots = new HotspotManager();
            FishingEffects = new FishingEffectService(Hotspots);
            MultiplayerSync = new MultiplayerSyncService(helper, this.Monitor, Hotspots);
            Throwing = new ThrowingService(helper, Hotspots, MultiplayerSync);
            SaveData = new SaveDataService(helper, this.Monitor, Hotspots);
            Assets = new AssetService(helper, () => Config);
            Renderer = new HotspotRenderer(Hotspots);

            // Apply Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);
                harmony.PatchAll();
                this.Monitor.Log("Harmony patches successfully registered.", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to apply Harmony patches: {ex}", LogLevel.Error);
            }

            // Register event handlers
            helper.Events.Content.AssetRequested += Assets.OnAssetRequested;
            helper.Events.Multiplayer.ModMessageReceived += MultiplayerSync.OnModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += this.OnPeerContextReceived;
            helper.Events.GameLoop.SaveLoaded += SaveData.OnSaveLoaded;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += SaveData.OnSaving;
            helper.Events.GameLoop.DayEnding += SaveData.OnDayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Player.Warped += this.OnPlayerWarped;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            this.Monitor.Log("Chum & Fish Frenzy mod loaded successfully.", LogLevel.Info);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            try
            {
                var projectile = this.Helper.ModContent.Load<Texture2D>("assets/chum_projectile.png");
                Throwing.SetProjectileTexture(projectile);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Error pre-loading projectile texture: {ex}", LogLevel.Warn);
            }

            // Register GMCM if available
            try
            {
                var gmcm = new GenericModConfigMenuIntegration(this.ModManifest, this.Helper, () => Config, () => this.Helper.WriteConfig(Config));
                gmcm.Register();
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Error registering Generic Mod Config Menu: {ex}", LogLevel.Trace);
            }
        }

        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (e.NewLocation != null)
            {
                Hotspots.SyncLocationNativeFrenzy(e.NewLocation);
            }
        }

        private void OnPeerContextReceived(object? sender, PeerContextReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                MultiplayerSync.SendSyncToPlayer(e.Peer.PlayerID);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            this.CheckRetroactiveRecipes();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            this.CheckRetroactiveRecipes();
            this.CheckWillyIntroMail();
        }

        private void CheckRetroactiveRecipes()
        {
            var player = Game1.player;
            if (player == null)
                return;

            if (player.FishingLevel >= 2 && !player.craftingRecipes.ContainsKey(AssetService.ChumId))
            {
                player.craftingRecipes.Add(AssetService.ChumId, 0);
                this.Monitor.Log($"Unlocked recipe: {AssetService.ChumId}", LogLevel.Trace);
            }

            if (player.FishingLevel >= 5 && !player.craftingRecipes.ContainsKey(AssetService.ChumMakerId))
            {
                player.craftingRecipes.Add(AssetService.ChumMakerId, 0);
                this.Monitor.Log($"Unlocked recipe: {AssetService.ChumMakerId}", LogLevel.Trace);
            }

            if (player.FishingLevel >= 7 && !player.craftingRecipes.ContainsKey(AssetService.FrenzyChumId))
            {
                player.craftingRecipes.Add(AssetService.FrenzyChumId, 0);
                this.Monitor.Log($"Unlocked recipe: {AssetService.FrenzyChumId}", LogLevel.Trace);
            }

            if (player.FishingLevel >= 10 && !player.craftingRecipes.ContainsKey(AssetService.DeluxeFrenzyChumId))
            {
                player.craftingRecipes.Add(AssetService.DeluxeFrenzyChumId, 0);
                this.Monitor.Log($"Unlocked recipe: {AssetService.DeluxeFrenzyChumId}", LogLevel.Trace);
            }
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Hotspots.TickTenMinutes();
            }

            this.CheckWillyIntroMail();
        }

        private void CheckWillyIntroMail()
        {
            if (!Config.EnableIntroMail)
                return;

            const string mailKey = "ChumFrenzy_WillyIntroMail";
            var player = Game1.player;

            if (player != null && player.FishingLevel >= 2)
            {
                if (!player.mailReceived.Contains(mailKey) && !player.mailbox.Contains(mailKey))
                {
                    player.mailbox.Add(mailKey);
                    this.Monitor.Log("Sent Willy's Chum introductory mail to player mailbox.", LogLevel.Trace);
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button.IsActionButton() || e.Button.IsUseToolButton())
            {
                var heldItem = Game1.player.ActiveItem;
                if (heldItem != null && heldItem.QualifiedItemId.StartsWith("(O)ChumFrenzy_"))
                {
                    this.Helper.Input.Suppress(e.Button);
                    Throwing.TryThrowChum(Game1.player, e.Cursor);
                }
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            float deltaSeconds = 1f / 60f;
            Throwing.Update(deltaSeconds);
            Renderer.Update(Game1.currentGameTime);
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            Renderer.Draw(e.SpriteBatch);
            Throwing.Draw(e.SpriteBatch);
        }
    }
}
