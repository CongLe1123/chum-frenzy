using System;
using StardewModdingAPI;

namespace ChumFrenzy.Compatibility
{
    public class GenericModConfigMenuIntegration
    {
        private readonly IManifest manifest;
        private readonly IModHelper helper;
        private readonly Func<ModConfig> getConfig;
        private readonly Action saveConfig;

        public GenericModConfigMenuIntegration(IManifest manifest, IModHelper helper, Func<ModConfig> getConfig, Action saveConfig)
        {
            this.manifest = manifest;
            this.helper = helper;
            this.getConfig = getConfig;
            this.saveConfig = saveConfig;
        }

        public void Register()
        {
            var api = this.helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null)
                return;

            api.Register(
                this.manifest,
                reset: () =>
                {
                    var fresh = new ModConfig();
                    var cfg = this.getConfig();
                    cfg.BasicChumDuration = fresh.BasicChumDuration;
                    cfg.BasicChumRadius = fresh.BasicChumRadius;
                    cfg.BasicChumBiteMultiplier = fresh.BasicChumBiteMultiplier;

                    cfg.FrenzyDuration = fresh.FrenzyDuration;
                    cfg.FrenzyRadius = fresh.FrenzyRadius;
                    cfg.FrenzyBiteMultiplier = fresh.FrenzyBiteMultiplier;
                    cfg.FrenzyTrashMultiplier = fresh.FrenzyTrashMultiplier;

                    cfg.SpeciesChumDuration = fresh.SpeciesChumDuration;
                    cfg.SpeciesChumRadius = fresh.SpeciesChumRadius;
                    cfg.SpeciesWeightMultiplier = fresh.SpeciesWeightMultiplier;

                    cfg.DeluxeFrenzyDuration = fresh.DeluxeFrenzyDuration;
                    cfg.DeluxeFrenzyRadius = fresh.DeluxeFrenzyRadius;
                    cfg.DeluxeFrenzyBiteMultiplier = fresh.DeluxeFrenzyBiteMultiplier;
                    cfg.DeluxeFrenzyTrashMultiplier = fresh.DeluxeFrenzyTrashMultiplier;
                    cfg.DeluxeSpeciesWeightMultiplier = fresh.DeluxeSpeciesWeightMultiplier;

                    cfg.MaxHotspotsPerLocation = fresh.MaxHotspotsPerLocation;
                    cfg.MinimumBiteDelay = fresh.MinimumBiteDelay;
                    cfg.EnableWillyShop = fresh.EnableWillyShop;
                    cfg.AllowLegendaryProcessing = fresh.AllowLegendaryProcessing;
                    cfg.EnableIntroMail = fresh.EnableIntroMail;
                },
                save: () =>
                {
                    this.getConfig().Sanitize();
                    this.saveConfig();
                }
            );

            // Basic Chum
            api.AddSectionTitle(this.manifest, () => this.helper.Translation.Get("config.section.basic_chum"));
            api.AddNumberOption(this.manifest, () => this.getConfig().BasicChumDuration, val => this.getConfig().BasicChumDuration = val, () => this.helper.Translation.Get("config.basic_chum_duration.name"), () => this.helper.Translation.Get("config.basic_chum_duration.desc"), 10, 240, 10);
            api.AddNumberOption(this.manifest, () => this.getConfig().BasicChumRadius, val => this.getConfig().BasicChumRadius = val, () => this.helper.Translation.Get("config.basic_chum_radius.name"), () => this.helper.Translation.Get("config.basic_chum_radius.desc"), 1.0f, 6.0f, 0.5f);
            api.AddNumberOption(this.manifest, () => this.getConfig().BasicChumBiteMultiplier, val => this.getConfig().BasicChumBiteMultiplier = val, () => this.helper.Translation.Get("config.basic_chum_bite_multiplier.name"), () => this.helper.Translation.Get("config.basic_chum_bite_multiplier.desc"), 0.1f, 1.0f, 0.05f);

            // Frenzy Chum
            api.AddSectionTitle(this.manifest, () => this.helper.Translation.Get("config.section.frenzy_chum"));
            api.AddNumberOption(this.manifest, () => this.getConfig().FrenzyDuration, val => this.getConfig().FrenzyDuration = val, () => this.helper.Translation.Get("config.frenzy_duration.name"), () => this.helper.Translation.Get("config.frenzy_duration.desc"), 10, 240, 10);
            api.AddNumberOption(this.manifest, () => this.getConfig().FrenzyRadius, val => this.getConfig().FrenzyRadius = val, () => this.helper.Translation.Get("config.frenzy_radius.name"), () => this.helper.Translation.Get("config.frenzy_radius.desc"), 1.0f, 8.0f, 0.5f);
            api.AddNumberOption(this.manifest, () => this.getConfig().FrenzyBiteMultiplier, val => this.getConfig().FrenzyBiteMultiplier = val, () => this.helper.Translation.Get("config.frenzy_bite_multiplier.name"), () => this.helper.Translation.Get("config.frenzy_bite_multiplier.desc"), 0.05f, 1.0f, 0.05f);
            api.AddNumberOption(this.manifest, () => this.getConfig().FrenzyTrashMultiplier, val => this.getConfig().FrenzyTrashMultiplier = val, () => this.helper.Translation.Get("config.frenzy_trash_multiplier.name"), () => this.helper.Translation.Get("config.frenzy_trash_multiplier.desc"), 0.0f, 1.0f, 0.05f);

            // Species Chum
            api.AddSectionTitle(this.manifest, () => this.helper.Translation.Get("config.section.species_chum"));
            api.AddNumberOption(this.manifest, () => this.getConfig().SpeciesChumDuration, val => this.getConfig().SpeciesChumDuration = val, () => this.helper.Translation.Get("config.species_duration.name"), () => this.helper.Translation.Get("config.species_duration.desc"), 10, 240, 10);
            api.AddNumberOption(this.manifest, () => this.getConfig().SpeciesChumRadius, val => this.getConfig().SpeciesChumRadius = val, () => this.helper.Translation.Get("config.species_radius.name"), () => this.helper.Translation.Get("config.species_radius.desc"), 1.0f, 8.0f, 0.5f);
            api.AddNumberOption(this.manifest, () => this.getConfig().SpeciesWeightMultiplier, val => this.getConfig().SpeciesWeightMultiplier = val, () => this.helper.Translation.Get("config.species_weight_multiplier.name"), () => this.helper.Translation.Get("config.species_weight_multiplier.desc"), 1.0f, 10.0f, 0.5f);

            // Deluxe Frenzy
            api.AddSectionTitle(this.manifest, () => this.helper.Translation.Get("config.section.deluxe_frenzy"));
            api.AddNumberOption(this.manifest, () => this.getConfig().DeluxeFrenzyDuration, val => this.getConfig().DeluxeFrenzyDuration = val, () => this.helper.Translation.Get("config.deluxe_duration.name"), () => this.helper.Translation.Get("config.deluxe_duration.desc"), 10, 240, 10);
            api.AddNumberOption(this.manifest, () => this.getConfig().DeluxeFrenzyRadius, val => this.getConfig().DeluxeFrenzyRadius = val, () => this.helper.Translation.Get("config.deluxe_radius.name"), () => this.helper.Translation.Get("config.deluxe_radius.desc"), 1.0f, 10.0f, 0.5f);
            api.AddNumberOption(this.manifest, () => this.getConfig().DeluxeFrenzyBiteMultiplier, val => this.getConfig().DeluxeFrenzyBiteMultiplier = val, () => this.helper.Translation.Get("config.deluxe_bite_multiplier.name"), () => this.helper.Translation.Get("config.deluxe_bite_multiplier.desc"), 0.05f, 1.0f, 0.05f);
            api.AddNumberOption(this.manifest, () => this.getConfig().DeluxeFrenzyTrashMultiplier, val => this.getConfig().DeluxeFrenzyTrashMultiplier = val, () => this.helper.Translation.Get("config.deluxe_trash_multiplier.name"), () => this.helper.Translation.Get("config.deluxe_trash_multiplier.desc"), 0.0f, 1.0f, 0.05f);
            api.AddNumberOption(this.manifest, () => this.getConfig().DeluxeSpeciesWeightMultiplier, val => this.getConfig().DeluxeSpeciesWeightMultiplier = val, () => this.helper.Translation.Get("config.deluxe_species_weight_multiplier.name"), () => this.helper.Translation.Get("config.deluxe_species_weight_multiplier.desc"), 1.0f, 15.0f, 0.5f);

            // General
            api.AddSectionTitle(this.manifest, () => this.helper.Translation.Get("config.section.general"));
            api.AddNumberOption(this.manifest, () => this.getConfig().MaxHotspotsPerLocation, val => this.getConfig().MaxHotspotsPerLocation = val, () => this.helper.Translation.Get("config.max_hotspots.name"), () => this.helper.Translation.Get("config.max_hotspots.desc"), 1, 30, 1);
            api.AddNumberOption(this.manifest, () => this.getConfig().MinimumBiteDelay, val => this.getConfig().MinimumBiteDelay = val, () => this.helper.Translation.Get("config.minimum_bite_delay.name"), () => this.helper.Translation.Get("config.minimum_bite_delay.desc"), 0.1f, 2.0f, 0.1f);
            api.AddBoolOption(this.manifest, () => this.getConfig().EnableWillyShop, val => this.getConfig().EnableWillyShop = val, () => this.helper.Translation.Get("config.enable_willy_shop.name"), () => this.helper.Translation.Get("config.enable_willy_shop.desc"));
            api.AddBoolOption(this.manifest, () => this.getConfig().AllowLegendaryProcessing, val => this.getConfig().AllowLegendaryProcessing = val, () => this.helper.Translation.Get("config.allow_legendary_processing.name"), () => this.helper.Translation.Get("config.allow_legendary_processing.desc"));
            api.AddBoolOption(this.manifest, () => this.getConfig().EnableIntroMail, val => this.getConfig().EnableIntroMail = val, () => this.helper.Translation.Get("config.enable_intro_mail.name"), () => this.helper.Translation.Get("config.enable_intro_mail.desc"));
        }
    }
}
