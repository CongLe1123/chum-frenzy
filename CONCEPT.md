Chum & Fish Frenzy — Complete Design
Core concept

The mod adds a full chum system for fishing.

Players can craft and throw different types of chum into fishable water to temporarily alter fishing conditions in a specific area.

The system has three purposes:

create artificial fishing hotspots
create intense temporary fish frenzies
create species-targeted fishing opportunities

The player still fishes normally with a rod.

Chum never gives fish directly.

1. Chum categories

There are three functional categories.

Basic Chum

Purpose:

Create a temporary fishing bubble anywhere valid.

This is the general utility chum.

Effect:

creates a fishing hotspot
speeds up bites
no species bias
no quality bonus
no treasure bonus
Frenzy Chum

Purpose:

Turn an area into a temporary feeding frenzy.

Effect:

much faster bites
heavily reduced trash chance
stronger visual activity
more fish-oriented catches
still obeys the normal fish availability rules
Species Chum

Purpose:

Attract a specific fish species.

Made using a fish as an ingredient.

Example:

Use Sardine to create:

Sardine Chum

When thrown into water where Sardine can normally appear:

Sardine becomes much more likely to be selected
other fish can still appear
normal requirements still apply

If Sardine cannot normally appear in that water at that time:

the chum does not bypass those restrictions
the species attraction simply has no effect on Sardine eligibility

This prevents targeted chum from becoming a way to cheat seasonal, weather, or location restrictions.

2. Unlock progression

Use fishing progression naturally.

Fishing Level	Unlock
2	Basic Chum
5	Chum Maker
7	Frenzy Chum
8	Species Chum
10	Advanced chum recipes

This gives the mod an actual progression curve rather than dumping everything on the player immediately.

3. Basic Chum
Crafting

Example recipe:

2 Bug Meat
2 Fiber

Produces:

2 Basic Chum

Effect

When thrown:

radius: 2 tiles
duration: 60 in-game minutes
bite delay reduction: 50%
normal fish tables
normal trash rates
normal quality
normal treasure chance

Visually:

bubbles
occasional ripples
occasional tiny splash

This functions approximately like a player-created fishing bubble.

4. Frenzy Chum
Recipe

Example:

5 Bug Meat
5 Bait
1 Seaweed
1 Green Algae

Produces:

1 Frenzy Chum

Effect

When thrown:

radius: 3 tiles
duration: 40 in-game minutes
bite delay reduction: 75%
trash chance reduced by 85%
no direct quality bonus
no easier fishing minigame

The player still has to actually catch every fish.

5. Species Chum

Species Chum is produced through the Chum Maker.

The player inserts one fish.

Example:

Largemouth Bass

becomes:

Largemouth Bass Chum

The resulting item retains the fish species information.

6. Chum Maker

Add a new processing machine:

Chum Maker

Purpose:

Convert fish into species-specific chum.

Suggested recipe:

20 Wood
10 Stone
5 Hardwood
1 Copper Bar

Unlock:

Fishing Level 5.

Processing behavior

Insert:

1 fish

After processing:

Receive:

2 Species Chum

The chum remembers the exact fish species used.

Fish quality does not matter.

A gold-quality fish and normal fish make the same chum.

That prevents wasting high-quality fish purely for stronger chum.

7. Species Chum effect

When thrown:

radius: 3 tiles
duration: 60 in-game minutes

Within that area:

If the targeted fish is eligible to be caught:

its selection weight is multiplied.

Recommended starting multiplier:

×4

Example normal fish table:

Carp: 40
Largemouth Bass: 20
Bullhead: 20
Chub: 20

Using Largemouth Bass Chum:

Carp: 40
Largemouth Bass: 80
Bullhead: 20
Chub: 20

The fish is significantly more common but not guaranteed.

This is better than forcing 100% catches because the fishing location still feels alive.

8. Invalid Species Chum

Suppose the player throws Tuna Chum into the mountain lake.

Tuna cannot normally appear there.

Then:

the chum spot still exists
normal fishing continues
no Tuna is added to the catch table
the item is not refunded

This keeps the system consistent.

The chum attracts that species.

It does not magically teleport fish into impossible habitats.

9. Legendary fish

Legendary fish require special rules.

Species Chum made from legendary fish should either be impossible or heavily restricted.

I recommend:

Legendary and unique one-time fish cannot be processed in the Chum Maker.

This includes fish that the game treats as unique or special.

That avoids duplication exploits and strange save behavior.

Legendary fish can still be caught while normal chum is active, but chum does not increase their appearance rate.

10. Special fish

Fish with unusual rules should continue following the original game.

Examples:

Night Market fish
submarine fish
Ginger Island fish
mine fish
sewer fish
special quest fish

Species Chum only modifies selection after eligibility has already been determined.

That architecture is important.

The logic should conceptually be:

Determine all fish normally available here → apply chum weighting to eligible fish.

Not:

Add chum fish to the available list.

11. Throwing system

Chum is a throwable consumable.

The player:

selects chum
faces water
presses use
character performs a throwing animation
chum travels to the selected water tile
splash occurs
hotspot starts

Maximum throwing distance:

5 tiles

12. Target selection

The targeting system should prefer:

cursor-selected water tile on mouse
controller aim direction
nearest valid water tile in the player's facing direction

This makes it work properly with:

mouse
keyboard
controller
Steam Deck
split-screen
13. Invalid throw

If no valid fishable water exists within range:

do not consume chum
do not play full throw animation
play an error sound
optionally display:

“There aren't any fish here to attract.”

14. Valid water

The mod should not merely check whether a tile looks like water.

It must check whether the location supports normal rod fishing.

Valid examples:

ocean
rivers
lakes
forest ponds
farm fishing water
mine fishing floors
Ginger Island
special fishing maps

Invalid examples:

fountains
decorative water
inaccessible water
bathhouse
map water that cannot normally produce fishing catches
15. Hotspot positioning

Each chum hotspot stores:

location
center tile
radius
chum type
start time
remaining duration
optional target fish ID
owner player ID if needed for statistics
unique hotspot ID

Gameplay effects are not owner-exclusive.

16. Hotspot overlap

Multiple hotspots can overlap.

Bonuses should not blindly multiply.

Use explicit combination rules.

Bite speed

Use the strongest bite-speed modifier.

Example:

Basic Chum + Frenzy

Result:

Frenzy bite speed.

Trash modifier

Use the strongest trash reduction.

Species modifiers

Species attraction can coexist with a general hotspot.

Example:

Frenzy Chum + Catfish Chum

Result:

Frenzy bite speed
Frenzy trash reduction
Catfish weighting

This creates useful combinations without becoming mathematically insane.

17. Same species stacking

Two Catfish Chum spots overlapping should not produce:

×4 ×4 = ×16.

Instead:

Use the strongest individual species multiplier.

So:

Catfish Chum + Catfish Chum

still equals:

×4 Catfish weight

18. Different species overlap

If two different species chums overlap:

Example:

Catfish Chum

plus

Shad Chum

Both eligible fish receive their own weight multiplier.

That creates mixed targeted fishing areas.

19. Maximum active hotspots

The game should technically support many hotspots, but there should be a practical limit to prevent players from covering an entire map.

Recommended:

Maximum 10 active chum hotspots per location.

If the player attempts an 11th:

Remove the oldest hotspot.

Alternatively, allow unlimited spots if performance testing proves it safe.

20. Hotspot lifetime

Use in-game time.

Recommended:

Basic Chum:

60 minutes

Species Chum:

60 minutes

Frenzy Chum:

40 minutes

21. Time pausing

Hotspot timers follow Stardew's world clock.

If game time does not advance:

The hotspot does not expire.

This avoids real-time inconsistencies.

22. Leaving the map

Hotspots continue existing when players leave.

Example:

Player creates hotspot at Beach at 2:00 PM.

Leaves at 2:10.

Returns at 2:40.

If the duration has not expired:

the hotspot still exists.

23. Saving

Hotspots should be saved.

Since you want the system fully functioning, there is no reason to arbitrarily throw state away.

Each active hotspot should serialize:

location
center
type
target species
remaining duration

On load:

restore all unexpired hotspots.

24. Overnight behavior

All hotspots automatically disappear overnight.

They are temporary feeding effects and should not persist into the next day.

When the player sleeps:

clear all active hotspots.

This also simplifies long-term state management.

25. Natural fishing bubbles

Natural bubbles and chum can overlap.

Do not remove or replace natural bubbles.

They remain separate systems.

Effects should combine according to explicit rules.

For bite time:

Use whichever reduction is strongest.

Example:

Natural bubble = 50%

Frenzy = 75%

Result:

75%.

26. Bait interaction

Normal bait still works.

Chum is an environmental effect.

Bait is attached to the fishing rod.

They should therefore be compatible.

However, bite-speed calculations should have a floor.

Recommended minimum bite delay:

0.5 seconds

No combination can reduce the delay below that.

27. Tackle interaction

Tackle works normally.

Examples:

Trap Bobber:

still reduces fish escape.

Cork Bobber:

still increases fishing bar size.

Quality Bobber:

still changes fish quality.

Treasure Hunter:

still changes treasure behavior.

Spinner:

still improves bite rate, subject to the minimum bite-delay floor.

28. Challenge Bait

Challenge Bait should work normally.

Chum affects the environment.

Challenge Bait affects the catch.

No special exception needed.

29. Fish quality

Chum itself does not directly modify fish quality.

Quality continues to depend on normal Stardew factors.

This is important because otherwise Species Chum plus Frenzy would become both quantity and quality optimization simultaneously.

30. Perfect catches

Perfect catches work normally.

No chum-specific bonus is needed.

31. Treasure chests

Basic Chum:

no effect.

Species Chum:

no effect.

Frenzy Chum:

no direct treasure bonus.

This avoids turning Frenzy into a treasure farming mechanic.

32. Trash

Basic Chum:

normal trash chance.

Species Chum:

normal trash chance.

Frenzy Chum:

trash probability ×0.15.

That means roughly an 85% reduction.

33. Fish ponds

Do not affect fish ponds.

A fish pond is not normal wild fishing water.

Chum thrown near a fish pond should not alter its production.

34. Crab pots

No effect.

Crab pots operate on their own daily system.

35. Panning

No effect.

36. Multiplayer

Hotspots are shared world objects.

Any player can use a hotspot regardless of who created it.

37. Multiplayer synchronization

The host should be authoritative for:

spawning hotspots
deleting hotspots
hotspot expiration
restoring hotspots after load
validating throws

Clients should receive synchronized data for:

hotspot creation
hotspot removal
target fish
visual state
remaining lifetime
38. Simultaneous fishing

Several players can fish inside the same hotspot.

Each fishing attempt independently applies the hotspot modifiers.

Nothing is “consumed” by catching a fish.

39. Split-screen support

The system must support split-screen.

Each local player:

can equip chum independently
can throw independently
can see shared hotspots
receives bonuses according to their own bobber position

Do not rely on a single global Game1.player assumption.

40. Controller support

All core interactions must be controller compatible.

The player must be able to:

select chum
aim
throw
operate Chum Maker
read item tooltips

without requiring mouse interaction.

41. Visuals — Basic Chum

Basic Chum should appear modest.

Use:

small bubble clusters
gentle circular ripples
occasional water splash

It should visually resemble fish gathering, but not a dramatic event.

42. Visuals — Frenzy Chum

Frenzy should look noticeably different.

Use:

frequent bubbles
fish shadows
splashes
occasional fish jumping
fast-moving ripples
multiple active water disturbances

The effect should immediately tell the player:

A lot of fish are feeding here.

43. Fish shadows

Fish shadows should be decorative sprites only.

They do not need:

collision
pathfinding
individual fish AI
persistence
actual catchability

Each shadow simply:

spawns near hotspot
chooses a small movement direction
swims
fades or turns
respawns when needed
44. Visual density

Basic:

0–3 small fish effects.

Species:

2–5 fish effects.

Frenzy:

6–12 fish effects.

Do not spawn dozens of persistent objects.

Use pooled or lightweight temporary sprites.

45. Species Chum visual identity

Species Chum should have a small unique cue.

For example:

When thrown, briefly show the targeted fish icon above the splash.

Not permanently.

Just something like:

Catfish icon → splash → hotspot

That clearly communicates what the spot attracts.

46. Item names

Suggested naming:

Chum

Frenzy Chum

Species:

Catfish Chum

Tuna Chum

Sturgeon Chum

etc.

47. Tooltips
Chum

Throw into fishable water to create a temporary fishing hotspot.

Frenzy Chum

Throw into fishable water to cause an intense feeding frenzy. Fish bite much faster and trash becomes rare.

Catfish Chum

Throw into fishable water to attract Catfish. Only works where Catfish can normally be caught.

This last sentence is important.

48. Chum Maker UI

The machine should behave like standard Stardew processing equipment.

Player holds a fish.

Interact with Chum Maker.

Fish is inserted.

Machine begins processing.

After completion:

interact to retrieve Species Chum.

No custom UI is necessary.

49. Chum Maker processing time

Recommended:

30 in-game minutes

This is long enough to feel like processing without being annoying.

50. Output amount

One normal fish produces:

2 Species Chum

Special balance rule:

Extremely valuable fish still produce the same amount.

Quality does not increase output.

51. Fish eligibility pipeline

This should be a strict implementation rule.

When fishing:

Step 1

Let Stardew determine the fish that are normally eligible.

Step 2

Apply all normal game restrictions.

Step 3

Check active chum around bobber.

Step 4

Modify selection weights.

Step 5

Choose catch.

This dramatically reduces incompatibility.

52. Mod compatibility

Avoid completely replacing Stardew's fishing selection code.

Patch or modify the smallest necessary section.

Ideally:

inspect active bobber location
inspect eligible fish
modify catch probability/weight
preserve original result pipeline

This gives better compatibility with:

custom fish mods
location mods
expansion mods
Content Patcher fish
Stardew Valley Expanded
Ridgeside Village
East Scarp
53. Modded fish

Species Chum should support modded fish when technically possible.

The Chum Maker should store:

qualified item ID

rather than hard-coded vanilla numeric IDs.

That way:

Any compatible fish item can potentially become Species Chum.

54. Dynamic Species Chum

Do not create hundreds of individual hard-coded item definitions.

Instead Species Chum should be one logical item type with metadata:

TargetFishId = "(O)128"
TargetFishName = "Pufferfish"

or equivalent mod data.

Then:

display name becomes dynamic.

This architecture lets the mod support arbitrary fish.

55. Inventory stacking

Species Chum can stack only when its target fish is identical.

Example:

Catfish Chum + Catfish Chum

can stack.

Catfish Chum + Tuna Chum

cannot stack.

This may require separate item metadata handling.

56. Selling

All chum can be sold.

Suggested values:

Chum:

20g

Frenzy Chum:

80g

Species Chum:

based on target fish value.

Potential formula:

10% of base fish sell value + 20g

But apply a minimum and maximum.

For example:

minimum 20g

maximum 250g

This avoids absurd prices from rare fish.

57. Chum Maker exploit prevention

The Species Chum output should always be worth less than the input fish plus the gameplay utility.

Do not make processing chum a pure profit machine.

Its purpose is fishing optimization.

58. Recipe discovery

Recipes should appear naturally when fishing skill increases.

Use normal level-up recipe unlock behavior.

No mail required unless you want flavor text.

59. Optional mail introduction

A simple Willy letter could introduce the system.

For example:

Fish are easier to find when they're feeding. Toss some chum into the water and you'll see what I mean.

Then give:

3 Chum

This is a good tutorial mechanism.

60. Willy integration

Willy can sell some items.

Suggested:

Chum:

available after Level 2.

Frenzy Chum:

available after Level 7.

The player can craft them cheaper, but buying provides convenience.

Species Chum should generally require the Chum Maker.

61. Pricing

Suggested:

Chum:

80g

Frenzy Chum:

350g

This prevents buying infinite cheap frenzy.

62. Animation

When using chum:

Player performs a short underhand throw.

Projectile arc travels toward target.

On impact:

splash
small sound
ripple
hotspot starts

The item should not simply vanish from the toolbar.

Physical feedback matters a lot here.

63. Throw speed

The throw should be quick.

Approximately:

0.4–0.6 seconds

Do not make the player stand through a long animation.

64. Throw interruption

Do not allow throws while:

passing out
using another tool
during cutscenes
sleeping
riding transitions
fishing
menus are blocking gameplay
65. Casting into the hotspot

The bonus is determined by:

bobber location

not:

player location
casting origin
player's facing direction

This is mandatory.

66. Radius geometry

Use circular distance rather than square tile bounds.

For center (x, y):

distance(center, bobber) <= radius

This makes the visual and gameplay area match naturally.

67. Hotspot visual boundary

Do not draw a giant visible circle.

The boundary should be communicated organically using water activity.

However, optional config could enable a debug outline.

68. Configuration

Add a config.json.

Expose:

BasicChumDuration
BasicChumRadius
BasicChumBiteMultiplier
FrenzyDuration
FrenzyRadius
FrenzyBiteMultiplier
FrenzyTrashMultiplier
SpeciesChumDuration
SpeciesChumRadius
SpeciesWeightMultiplier
MaxHotspotsPerLocation
MinimumBiteDelay
EnableFishShadows
FishShadowDensity
EnableWillyShop
AllowLegendaryProcessing

Default legendary processing:

false

69. GMCM support

If Generic Mod Config Menu is installed:

Expose those options through GMCM.

But GMCM must remain optional.

The mod must work normally without it.

70. Error handling

Never consume an item if:

target location invalid
target tile invalid
throw cannot be completed
synchronization failed before host confirmation

Only consume once hotspot creation succeeds.

71. Desync recovery

If a multiplayer client misses a hotspot event:

The host should periodically or on warp synchronize the authoritative hotspot list.

That prevents invisible gameplay bonuses or phantom visuals.

72. Warp synchronization

When a player enters a location:

send that client all currently active hotspots in that location.

73. Joining mid-day

A multiplayer player joining an existing save should see all active hotspots after synchronization.

74. Save data

Store mod data under a unique key.

Example conceptual structure:

Hotspots:
  - Location
  - X
  - Y
  - Radius
  - Type
  - TargetFish
  - RemainingMinutes

Do not store decorative sprites.

Recreate visuals from hotspot state.

75. Item metadata

Species Chum needs:

ChumType = Species
TargetFishQualifiedId

Potentially:

TargetFishDisplayName

But preferably derive the name from the item registry so localization remains correct.

76. Localization

All visible strings should go through i18n.

At minimum:

item names
descriptions
invalid throw message
recipe names
config labels
Willy mail
machine description

Do not hard-code English inside gameplay code.

77. Custom fish compatibility

If the original fish item cannot be resolved later:

Species Chum should fail gracefully.

Display:

Unknown Fish Chum

and treat it as inactive species attraction.

Do not crash the save.

78. Fish removed by another mod

Same rule.

Existing chum referencing a removed fish must remain safe.

The item may still exist.

It simply cannot apply its targeted bonus.

79. Festival handling

Do not interfere with fishing minigame festivals unless explicitly compatible.

For:

Ice Fishing Contest
Festival of Ice
Trout Derby
SquidFest

Use normal location and fishing checks.

If the festival uses standard fishing:

chum can work.

If it uses custom scoring logic:

do not override it.

80. Competitions

Chum catches should still count normally when the base game counts them.

Do not manually award competition points.

Let the normal catch event do that.

81. Experience

All catches grant normal fishing XP.

Chum itself grants no XP.

82. Mastery

Fishing mastery mechanics continue normally.

Do not duplicate or override mastery bonuses.

83. Sonar Bobber

If the player uses Sonar Bobber:

normal functionality remains.

Species Chum should influence which fish is selected before Sonar displays it.

84. Advanced Iridium Rod

All rod slot behavior remains untouched.

85. Deluxe Bait Maker integration

Since the original Reddit idea specifically mentioned the special bait maker, I would integrate it rather than ignore it.

Add one additional interaction:

When the player puts a fish into a Deluxe Bait Maker, normally it makes targeted bait.

The mod can leave that behavior unchanged.

The Chum Maker is separate because the effects are fundamentally different:

Targeted Bait:

personal rod effect.

Species Chum:

shared environmental effect.

That distinction makes both systems useful.

86. Why not reuse Targeted Bait directly?

Because it would create awkward behavior.

The same item would then serve both:

rod attachment
thrown world consumable

That creates input conflicts and unclear inventory behavior.

Separate item types are cleaner.

87. Advanced Chum

You also mentioned wanting the system completely fleshed out.

A fourth type fits naturally:

Deluxe Frenzy Chum

Unlock:

Fishing Level 10 or Mastery.

Effect:

radius 4
duration 30 minutes
Frenzy bite speed
strong trash reduction
increases fish selection toward uncommon fish

But this requires defining what “uncommon” means across modded fish.

I would therefore use a safer rule:

Deluxe Frenzy Chum

radius 4
bite delay reduction 80%
trash reduction 95%
species chum effects within it are ×5 instead of ×4
stronger visuals

This is powerful without inventing rarity logic.

88. Deluxe Frenzy recipe

Example:

1 Frenzy Chum
10 Bait
1 Squid Ink
1 Roe

Produces:

1 Deluxe Frenzy Chum

89. Complete chum hierarchy

So the finished mod contains:

Chum

General hotspot.

Frenzy Chum

High-speed mass fishing.

Species Chum

Target one fish.

Deluxe Frenzy Chum

Endgame large frenzy.

That is enough variety without turning every fish into a separate hard-coded recipe.

90. Combination example

Imagine:

6:00 PM

Rain

River

Player wants Catfish.

They throw:

Catfish Chum

Then:

Frenzy Chum

The areas overlap.

Now inside the overlap:

Catfish receives ×4 selection weight
bite times are 75% shorter
trash chance drops 85%
Catfish still only appears because rain/time/location conditions already make it eligible

This is exactly the sort of emergent combination that makes the system interesting.

91. Deluxe combination

Later:

Catfish Chum + Deluxe Frenzy

Results:

Catfish target multiplier upgraded to ×5
bite delay reduced 80%
trash reduced 95%
radius determined independently by each hotspot

Again, no bonus stacking beyond defined rules.

92. Water animation lifecycle

Each hotspot should have:

Activation

Splash and expanding ring.

Stable phase

Normal visual density.

Near expiration

Visual density gradually declines.

Expiration

A final small ripple.

Then disappear.

93. Audio

Reuse Stardew-style water sounds where appropriate.

Avoid loud repetitive sounds because Frenzy has many visual events.

Fish jumps should not each trigger a loud sound.

Randomize and throttle audio.

94. Performance rule

Never base visual particle count directly on fishing attempts.

Keep a fixed maximum number of cosmetic actors per hotspot.

For example:

Basic:

3

Species:

5

Frenzy:

12

Deluxe:

16

This gives predictable performance.

95. Performance with many hotspots

If visual objects become expensive:

prioritize rendering hotspots near the local player's camera.

Gameplay calculations still work everywhere.

This is particularly useful for split-screen.

96. Offscreen visuals

Do not simulate decorative fish individually when the hotspot is offscreen.

Only hotspot gameplay state needs to remain active.

Recreate cosmetic movement when it enters the viewport.

97. Statistics

Optional but fitting for a full mod:

Track:

chum thrown
frenzy catches
targeted catches
most-used species chum

Do not expose this unless you add a stats page.

It is not required for core gameplay.

98. Achievements

I wouldn't add custom achievements unless you already have infrastructure for them.

They add another compatibility and localization layer without improving the main mechanic much.

99. Final system rule

The entire mod should revolve around one consistent principle:

Chum modifies the fishing environment. It does not replace Stardew's fishing system.

Everything else follows from that.

It means:

normal rods still matter
bait still matters
tackle still matters
fishing skill still matters
season matters
weather matters
location matters
fish difficulty matters
the fishing minigame still matters

Chum simply gives the player more control over where, how fast, and what they are trying to attract.

That would make this feel like a natural Stardew fishing expansion rather than a cheat mechanic.