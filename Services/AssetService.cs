using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;

namespace ChumFrenzy.Services
{
    public class AssetService
    {
        private readonly IModHelper helper;
        private readonly Func<ModConfig> getConfig;

        public const string ChumId = "ChumFrenzy_Chum";
        public const string FrenzyChumId = "ChumFrenzy_FrenzyChum";
        public const string DeluxeFrenzyChumId = "ChumFrenzy_DeluxeFrenzyChum";
        public const string SpeciesChumId = "ChumFrenzy_SpeciesChum";
        public const string ChumMakerId = "ChumFrenzy_ChumMaker";

        public const string QualifiedChumId = "(O)" + ChumId;
        public const string QualifiedFrenzyChumId = "(O)" + FrenzyChumId;
        public const string QualifiedDeluxeFrenzyChumId = "(O)" + DeluxeFrenzyChumId;
        public const string QualifiedSpeciesChumId = "(O)" + SpeciesChumId;
        public const string QualifiedChumMakerId = "(BC)" + ChumMakerId;

        public const string TextureChum = "Mods/ChumTeam.ChumFrenzy/Chum";
        public const string TextureFrenzyChum = "Mods/ChumTeam.ChumFrenzy/FrenzyChum";
        public const string TextureDeluxeFrenzyChum = "Mods/ChumTeam.ChumFrenzy/DeluxeFrenzyChum";
        public const string TextureSpeciesChum = "Mods/ChumTeam.ChumFrenzy/SpeciesChum";
        public const string TextureChumMaker = "Mods/ChumTeam.ChumFrenzy/ChumMaker";
        public const string TextureProjectile = "Mods/ChumTeam.ChumFrenzy/Projectile";

        public AssetService(IModHelper helper, Func<ModConfig> getConfig)
        {
            this.helper = helper;
            this.getConfig = getConfig;
        }

        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            // Custom textures
            if (e.NameWithoutLocale.IsEquivalentTo(TextureChum))
            {
                e.LoadFromModFile<Texture2D>("assets/chum.png", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TextureFrenzyChum))
            {
                e.LoadFromModFile<Texture2D>("assets/frenzy_chum.png", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TextureDeluxeFrenzyChum))
            {
                e.LoadFromModFile<Texture2D>("assets/deluxe_frenzy_chum.png", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TextureSpeciesChum))
            {
                e.LoadFromModFile<Texture2D>("assets/species_chum_base.png", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TextureChumMaker))
            {
                e.LoadFromModFile<Texture2D>("assets/chum_maker.png", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TextureProjectile))
            {
                e.LoadFromModFile<Texture2D>("assets/chum_projectile.png", AssetLoadPriority.Medium);
            }

            // Data/Objects
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;

                    data[ChumId] = new ObjectData
                    {
                        Name = "Chum",
                        DisplayName = this.helper.Translation.Get("item.chum.name"),
                        Description = this.helper.Translation.Get("item.chum.description"),
                        Type = "Basic",
                        Category = -999,
                        Price = 20,
                        Texture = TextureChum,
                        SpriteIndex = 0
                    };

                    data[FrenzyChumId] = new ObjectData
                    {
                        Name = "Frenzy Chum",
                        DisplayName = this.helper.Translation.Get("item.frenzy_chum.name"),
                        Description = this.helper.Translation.Get("item.frenzy_chum.description"),
                        Type = "Basic",
                        Category = -999,
                        Price = 80,
                        Texture = TextureFrenzyChum,
                        SpriteIndex = 0
                    };

                    data[DeluxeFrenzyChumId] = new ObjectData
                    {
                        Name = "Deluxe Frenzy Chum",
                        DisplayName = this.helper.Translation.Get("item.deluxe_frenzy_chum.name"),
                        Description = this.helper.Translation.Get("item.deluxe_frenzy_chum.description"),
                        Type = "Basic",
                        Category = -999,
                        Price = 150,
                        Texture = TextureDeluxeFrenzyChum,
                        SpriteIndex = 0
                    };

                    data[SpeciesChumId] = new ObjectData
                    {
                        Name = "Species Chum",
                        DisplayName = this.helper.Translation.Get("item.species_chum.unknown_name"),
                        Description = this.helper.Translation.Get("item.species_chum.unknown_description"),
                        Type = "Basic",
                        Category = -999,
                        Price = 20,
                        Texture = TextureSpeciesChum,
                        SpriteIndex = 0
                    };
                });
            }

            // Data/BigCraftables
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>().Data;

                    data[ChumMakerId] = new BigCraftableData
                    {
                        Name = "Chum Maker",
                        DisplayName = this.helper.Translation.Get("item.chum_maker.name"),
                        Description = this.helper.Translation.Get("item.chum_maker.description"),
                        Price = 100,
                        Texture = TextureChumMaker,
                        SpriteIndex = 0
                    };
                });
            }

            // Data/CraftingRecipes
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    // Fishing Lv 2: 2 Bug Meat (684), 2 Fiber (771) -> 2 Chum
                    data[ChumId] = $"684 2 771 2/Home/{ChumId} 2/false/s Fishing 2/{this.helper.Translation.Get("recipe.chum.name")}";

                    // Fishing Lv 5: 20 Wood (388), 10 Stone (390), 5 Hardwood (709), 1 Copper Bar (334) -> Chum Maker (BigCraftable)
                    data[ChumMakerId] = $"388 20 390 10 709 5 334 1/Home/{ChumMakerId}/true/s Fishing 5/{this.helper.Translation.Get("recipe.chum_maker.name")}";

                    // Fishing Lv 7: 5 Bug Meat (684), 5 Bait (685), 1 Seaweed (152), 1 Green Algae (153) -> 1 Frenzy Chum
                    data[FrenzyChumId] = $"684 5 685 5 152 1 153 1/Home/{FrenzyChumId} 1/false/s Fishing 7/{this.helper.Translation.Get("recipe.frenzy_chum.name")}";

                    // Fishing Lv 10: 1 Frenzy Chum, 10 Bait (685), 1 Squid Ink (814), 1 Roe (347) -> 1 Deluxe Frenzy Chum
                    data[DeluxeFrenzyChumId] = $"{FrenzyChumId} 1 685 10 814 1 347 1/Home/{DeluxeFrenzyChumId} 1/false/s Fishing 10/{this.helper.Translation.Get("recipe.deluxe_frenzy_chum.name")}";
                });
            }

            // Data/Machines
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, MachineData>().Data;

                    data[QualifiedChumMakerId] = new MachineData
                    {
                        HasInput = true,
                        HasOutput = true,
                        WobbleWhileWorking = true,
                        ShowNextIndexWhileWorking = true,
                        ShowNextIndexWhenReady = false,
                        InvalidItemMessage = this.helper.Translation.Get("message.chum_maker.only_fish"),
                        OutputRules = new List<MachineOutputRule>
                        {
                            new MachineOutputRule
                            {
                                Id = "ChumFrenzy.ChumMaker.ProcessFish",
                                Triggers = new List<MachineOutputTriggerRule>
                                {
                                    new MachineOutputTriggerRule
                                    {
                                        Id = "ItemPlaced",
                                        Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                                        RequiredTags = new List<string> { "category_fish" },
                                        RequiredCount = 1
                                    }
                                },
                                MinutesUntilReady = 30,
                                OutputItem = new List<MachineItemOutput>
                                {
                                    new MachineItemOutput
                                    {
                                        OutputMethod = "ChumFrenzy.Services.ChumMakerService, ChumFrenzy:OutputSpeciesChum"
                                    }
                                }
                            }
                        }
                    };
                });
            }

            // Data/Shops (Willy's FishShop)
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(asset =>
                {
                    if (!this.getConfig().EnableWillyShop)
                        return;

                    var data = asset.AsDictionary<string, ShopData>().Data;
                    if (data.TryGetValue("FishShop", out var fishShop))
                    {
                        fishShop.Items.Add(new ShopItemData
                        {
                            Id = "ChumFrenzy_Willy_Chum",
                            ItemId = QualifiedChumId,
                            Price = 80,
                            Condition = "PLAYER_FISHING_LEVEL Current 2"
                        });

                        fishShop.Items.Add(new ShopItemData
                        {
                            Id = "ChumFrenzy_Willy_FrenzyChum",
                            ItemId = QualifiedFrenzyChumId,
                            Price = 350,
                            Condition = "PLAYER_FISHING_LEVEL Current 7"
                        });
                    }
                });
            }

            // Data/Mail
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["ChumFrenzy_WillyIntroMail"] = this.helper.Translation.Get("mail.willy_intro.body");
                });
            }
        }
    }
}
