using System;
using System.Linq;
using ChumFrenzy.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ChumFrenzy.Services
{
    public class SaveDataService
    {
        private const string SaveKey = "chum-hotspots";
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly HotspotManager hotspotManager;

        public SaveDataService(IModHelper helper, IMonitor monitor, HotspotManager hotspotManager)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.hotspotManager = hotspotManager;
        }

        public void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            try
            {
                var saveData = this.helper.Data.ReadSaveData<ModSaveData>(SaveKey);
                if (saveData != null && saveData.Hotspots != null)
                {
                    // Migration / validation
                    var validHotspots = saveData.Hotspots
                        .Where(h => h.RemainingMinutes > 0 && !string.IsNullOrEmpty(h.LocationName))
                        .ToList();

                    this.hotspotManager.SetAllHotspots(validHotspots);
                    this.monitor.Log($"Restored {validHotspots.Count} active chum hotspots from save data.", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                this.monitor.Log($"Failed to load chum hotspots from save data: {ex.Message}", LogLevel.Warn);
            }
        }

        public void OnSaving(object? sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            try
            {
                var saveData = new ModSaveData
                {
                    Version = 1,
                    Hotspots = this.hotspotManager.GetAllHotspots().Where(h => h.RemainingMinutes > 0).ToList()
                };

                this.helper.Data.WriteSaveData(SaveKey, saveData);
            }
            catch (Exception ex)
            {
                this.monitor.Log($"Failed to write chum hotspots to save data: {ex.Message}", LogLevel.Warn);
            }
        }

        public void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            // All active hotspots disappear overnight
            this.hotspotManager.ClearAll();

            if (Context.IsMainPlayer)
            {
                try
                {
                    this.helper.Data.WriteSaveData(SaveKey, new ModSaveData { Version = 1 });
                }
                catch
                {
                    // Safe ignore
                }
            }
        }
    }
}
