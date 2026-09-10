# Chum & Fish Frenzy

[![Stardew Valley 1.6](https://img.shields.io/badge/Stardew%20Valley-1.6+-brightgreen.svg)](https://stardewvalleywiki.com/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0+-blue.svg)](https://smapi.io/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Chum & Fish Frenzy** brings a complete chum and feeding frenzy system to *Stardew Valley 1.6+*. Craft various grades of chum or process fish through the **Chum Maker** to create artificial fishing hotspots, trigger feeding frenzies, and target elusive fish species without breaking vanilla progression or seasonal rules.

---

## 🎣 Features

* **Artificial Bubble Hotspots:** Throw chum into fishable water to create your own fishing spots on demand.
* **Fish Frenzies:** Summon native Stardew 1.6 fish frenzies with jumping fish, high bite rates, and heavily reduced trash.
* **Species-Targeted Chum:** Turn caught fish into specialized chum to heavily increase the chance of catching that specific species.
* **The Chum Maker Machine:** A craftable machine to convert fish into species-targeted chum.
* **Vanilla Balance Friendly:** Species chum respects vanilla seasons, weather, time, and location restrictions—it optimizes your time, not cheats the game rules.
* **Full Multiplayer Synchronization:** Hotspots, projectiles, and frenzy visual effects sync cleanly across all players and farmhands.
* **Mod Compatibility:** Works out-of-the-box with **Automate**, **Better Crafting**, and **Generic Mod Config Menu (GMCM)**.

---

## 🐟 The Chum System

| Item | Unlock | Crafting Recipe / Source | Description & Effects |
| :--- | :---: | :--- | :--- |
| **Basic Chum** | Fishing Lv 2 | 2 Bug Meat, 2 Fiber *(Yields 2)*<br>*(Also sold by Willy)* | Creates a temporary 3×3 bubble hotspot. Bites are **50% faster**. Lasts 60 in-game minutes. |
| **Chum Maker** | Fishing Lv 5 | 20 Wood, 10 Stone, 5 Hardwood, 1 Copper Bar | BigCraftable machine. Place any standard fish inside to produce 2 **Species Chum**. |
| **Species Chum** | Fishing Lv 5 | Produced via Chum Maker using any fish | Increases target fish selection chance by **4×** within a 3×3 area. The sprite dynamically displays the fish inside! |
| **Frenzy Chum** | Fishing Lv 7 | 5 Bug Meat, 5 Bait, 1 Seaweed, 1 Green Algae *(Yields 1)*<br>*(Also sold by Willy)* | Triggers an active feeding frenzy with jumping fish. Bites are **75% faster**, trash is **85% rarer**, 4-tile radius. Lasts 90 in-game minutes. |
| **Deluxe Frenzy Chum** | Fishing Lv 10 | 1 Frenzy Chum, 10 Bait, 1 Squid Ink, 1 Roe *(Yields 1)* | Endgame feeding frenzy. 5-tile radius, **80% faster bites**, **95% less trash**, and stacks with Species Chum for a **5×** catch bonus. Lasts 120 minutes. |

---

## ⚙️ How to Use

1. **Craft or Buy Chum:** Craft chum in your crafting menu or purchase basic/frenzy chum from Willy's Fish Shop.
2. **Aim and Throw:** Hold any chum in your active inventory slot and right-click (or action button) on fishable water. Your character will toss the chum into the water.
3. **Cast Your Line:** Cast into the bubbling or splashing area. You do not need to hit the exact center tile—the benefits apply across the entire hotspot radius!

---

## 🤝 Mod Compatibility

* **Generic Mod Config Menu (GMCM):** In-game configuration UI to adjust durations, bite multipliers, trash reduction rates, hotspot radiuses, and shop prices.
* **Automate:** Connect chests to the **Chum Maker** to automatically process input fish and extract Species Chum.
* **Better Crafting:** All recipes are registered to `Data/CraftingRecipes` and appear natively with categories and chest crafting.
* **Lookup Anything & UI Info Suite 2:** Properly displays item values, recipes, and descriptions.

---

## 📥 Installation

1. Install the latest version of [SMAPI](https://smapi.io/) (4.0.0 or higher).
2. Download the latest release: `ChumAndFrenzy 1.0.2.zip`.
3. Unzip the folder and place `ChumAndFrenzy` into your `Stardew Valley/Mods` folder.
4. Launch the game using SMAPI!

---

## 📜 License & Permissions

Created by **CongLe1123**.

This project is licensed under the **[MIT License](LICENSE)**. 
You are completely free to:
* Fork, modify, fix, or improve this codebase.
* Include it in modpacks.
* Create translation packs or patches.
* Redistribute or build upon it however you want.

All that is required is keeping the original author credit (**CongLe1123**) in copies or substantial portions of the project.
