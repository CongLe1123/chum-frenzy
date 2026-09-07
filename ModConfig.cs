using System;

namespace ChumFrenzy
{
    public class ModConfig
    {
        public int BasicChumDuration { get; set; } = 60;
        public float BasicChumRadius { get; set; } = 2.0f;
        public float BasicChumBiteMultiplier { get; set; } = 0.50f;

        public int FrenzyDuration { get; set; } = 40;
        public float FrenzyRadius { get; set; } = 3.0f;
        public float FrenzyBiteMultiplier { get; set; } = 0.25f;
        public float FrenzyTrashMultiplier { get; set; } = 0.15f;

        public int SpeciesChumDuration { get; set; } = 60;
        public float SpeciesChumRadius { get; set; } = 3.0f;
        public float SpeciesWeightMultiplier { get; set; } = 4.0f;

        public int DeluxeFrenzyDuration { get; set; } = 30;
        public float DeluxeFrenzyRadius { get; set; } = 4.0f;
        public float DeluxeFrenzyBiteMultiplier { get; set; } = 0.20f;
        public float DeluxeFrenzyTrashMultiplier { get; set; } = 0.05f;
        public float DeluxeSpeciesWeightMultiplier { get; set; } = 5.0f;

        public int MaxHotspotsPerLocation { get; set; } = 10;
        public float MinimumBiteDelay { get; set; } = 0.5f;
        public bool EnableWillyShop { get; set; } = true;
        public bool AllowLegendaryProcessing { get; set; } = false;
        public bool EnableIntroMail { get; set; } = true;

        public void Sanitize()
        {
            this.BasicChumDuration = Math.Clamp(this.BasicChumDuration, 10, 360);
            this.BasicChumRadius = Math.Clamp(this.BasicChumRadius, 1.0f, 10.0f);
            this.BasicChumBiteMultiplier = Math.Clamp(this.BasicChumBiteMultiplier, 0.05f, 1.0f);

            this.FrenzyDuration = Math.Clamp(this.FrenzyDuration, 10, 360);
            this.FrenzyRadius = Math.Clamp(this.FrenzyRadius, 1.0f, 10.0f);
            this.FrenzyBiteMultiplier = Math.Clamp(this.FrenzyBiteMultiplier, 0.05f, 1.0f);
            this.FrenzyTrashMultiplier = Math.Clamp(this.FrenzyTrashMultiplier, 0.0f, 1.0f);

            this.SpeciesChumDuration = Math.Clamp(this.SpeciesChumDuration, 10, 360);
            this.SpeciesChumRadius = Math.Clamp(this.SpeciesChumRadius, 1.0f, 10.0f);
            this.SpeciesWeightMultiplier = Math.Clamp(this.SpeciesWeightMultiplier, 1.0f, 20.0f);

            this.DeluxeFrenzyDuration = Math.Clamp(this.DeluxeFrenzyDuration, 10, 360);
            this.DeluxeFrenzyRadius = Math.Clamp(this.DeluxeFrenzyRadius, 1.0f, 10.0f);
            this.DeluxeFrenzyBiteMultiplier = Math.Clamp(this.DeluxeFrenzyBiteMultiplier, 0.05f, 1.0f);
            this.DeluxeFrenzyTrashMultiplier = Math.Clamp(this.DeluxeFrenzyTrashMultiplier, 0.0f, 1.0f);
            this.DeluxeSpeciesWeightMultiplier = Math.Clamp(this.DeluxeSpeciesWeightMultiplier, 1.0f, 20.0f);

            this.MaxHotspotsPerLocation = Math.Clamp(this.MaxHotspotsPerLocation, 1, 50);
            this.MinimumBiteDelay = Math.Clamp(this.MinimumBiteDelay, 0.1f, 3.0f);
        }
    }
}
