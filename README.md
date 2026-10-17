<div align="center">

# The SPA III

### The updated **SPA II – Single Player Apartment** for GTA V Legacy
**Smooth performance · No memory leak · Reliable garages · Modern menus**

![GTA V](https://img.shields.io/badge/GTA%20V-Legacy-green)
![SHVDN](https://img.shields.io/badge/ScriptHookVDotNet-3.x-blue)
![UI](https://img.shields.io/badge/UI-LemonUI%202.2-orange)
![Status](https://img.shields.io/badge/status-tested-brightgreen)
![Online](https://img.shields.io/badge/GTA%20Online-not%20supported-red)

</div>

SPA III keeps everything you liked about SPA II (buy apartments and garages in story mode, store your cars, change clothes, use the phone mechanic) and fixes what made it hard to live with: FPS drops and memory growth in long sessions, glitchy garage animations, cars that wouldn't stay where you put them, and menu crashes.

Tested in-game on **GTA V Legacy**: buying properties, entering and leaving garages, storing, moving and swapping cars, apartment styles, wardrobe, phone menus, and long play sessions.

---

## 📑 Contents

[What's new](#-whats-new-compared-to-spa-ii) · [Requirements](#-requirements) · [Installation](#-installation) · [How to use](#-how-to-use) · [Troubleshooting](#-troubleshooting) · [Configuration](#️-configuration) · [Reporting problems](#-reporting-problems) · [Building](#️-building-from-source) · [Credits](#-credits)

---

## ✨ What's new compared to SPA II

| Area | SPA II 2.0.5 | SPA III |
|---|---|---|
| Scripting API | ScriptHookVDotNet 2 | **ScriptHookVDotNet 3** |
| Menus | INMNativeUI | **LemonUI 2.2** (no more crashes opening the closet and garage menus) |
| Interiors | Stayed loaded in memory forever | **Released when you're done with them** |
| Menus, signs and doors | Created for every property at startup | **Only for properties near you** |
| Entering / leaving a garage | Camera cutscene | **Loading screen, no cutscene** |
| Car after leaving a garage | Could drive off by itself | **Placed outside, standing still** |
| Moving and swapping cars | Buggy | **Reliable, with rollback if something fails** |
| Garage save files | Written directly; a crash could corrupt them | **Written safely, validated, with automatic backup** |
| A saved car whose DLC is missing | Could break the whole garage | **Skipped, and the car comes back when the DLC does** |

### 🚀 Performance and memory
- **Distance-based loading.** A property only has its menus, for-sale sign and doors while you're near it. It loads at **200 m or less** and unloads beyond **250 m**. Both distances are configurable. A property you're inside is never unloaded.
- **Interiors are released.** SPA II pinned every interior you visited in memory and never freed it, and never removed the IPLs it requested. This was the main cause of the slowdown over time.
- **Scaleform movies load on demand** instead of 11 of them being kept in memory from the start.
- **Far less work per frame.** Interior logic used to run once per property, every frame, and the config file was read from disk constantly. Both are gone.
- **No duplicated map blips**, periodic cleanup of dead vehicles, and a smart garbage collection that never runs during menus.
- **Safe log file**: capped at 2 MB, and repeated errors are collapsed.
- **Clean unload**: menus, props, blips, interior pins and scaleforms are released when scripts reload.

### 🚗 Garages and vehicles
- **Loading screen instead of cutscenes.** Entering or leaving a garage fades to black with a "Loading..." indicator, waits until the garage, the interior and your cars are ready, and only then shows the game again. It has a time limit, so it can never hang.
- **No more car driving off by itself.** The old exit cutscene set an automatic driving task and never cancelled it. That code is gone: your car appears outside the garage, stopped, under your control.
- **Moving and swapping cars works.** Moving or swapping validates every save and rolls back if one fails, so two cars can never end up in the same slot. Picking an empty slot no longer causes an error, and the camera follows the selected car.
- **Fixed the slot bug of the original.** SPA II calculated one slot too many, so a **full** garage could assign a slot that doesn't exist or one that was already taken. Parking a car back in its own garage now keeps its original slot, and a full garage correctly says it's full.
- **Old garages repair themselves.** Cars saved in an invalid or duplicated slot are moved to a free one the next time you enter the garage.
- **The management menu always shows** when you press the action key on the blue circle in a garage.
- **Safer vehicle saving.** Files are written to a temporary file, validated, and swapped in with a `.bak` copy. A corrupted file is recovered from the backup, and one bad file never hides your other cars.
- **Missing DLC cars don't break anything.** The mod skips them, writes a note to the log, and **keeps your save file**, so the car returns when the DLC is installed again.

### 🛡️ Stability
- **Fault-tolerant startup.** If one part fails (phone, wardrobe, a missing folder), the rest still loads. Missing folders are created automatically.
- **The screen can't get stuck on black**, even if something fails during a transition or scripts are reloaded.
- **Errors in one part of the game loop can't freeze the menus.** They're logged instead.
- **No more endless waits.** Model loading, IPL swapping and interior loading all have time limits.
- Removed the game-memory hacks of the old version (`Metadata.dll` and a write to a game global that could corrupt memory on newer builds).

### 💾 Your saves
Garage saves use the same format as SPA II, so **your existing cars and properties carry over**. Back up `scripts\SPA II` before the first run anyway.

---

## 📦 Requirements

| Dependency | Notes |
|---|---|
| **GTA V Legacy** | Story mode. |
| **ScriptHookV** (`ScriptHookV.dll` + `dinput8.dll`) | Official site: <https://dev-c.com/GTAV/scripthookv>. It must support **your exact** game build. At the time of writing the page lists v3889.0 for Legacy 1.0.3889.0. |
| **ScriptHookVDotNet 3.x** | Tested with **v3.7.0-nightly.191**: <https://github.com/scripthookvdotnet/scripthookvdotnet-nightly/releases>. The `.asi` and both `.dll` files must come from the **same** version. |
| **LemonUI.SHVDN3** 2.2 | Included in the release package. |
| **iFruitAddon2 v3.1.1** | Included in the release package. The old SHVDN2 version won't work. |
| **.NET Framework 4.8** | Already included in an up-to-date Windows 10/11. |
| **Visual C++ Redistributable 2019 x64** | Needed by ScriptHookV and SHVDN. |

No longer needed: `INMNativeUI.dll` and `Metadata.dll`.

---

## 🔧 Installation

**1. Game root folder** (next to `GTA5.exe`)

```
ScriptHookV.dll              ← ScriptHookV
dinput8.dll                  ← ScriptHookV (ASI loader)
ScriptHookVDotNet.asi        ← SHVDN
ScriptHookVDotNet.ini        ← SHVDN
ScriptHookVDotNet2.dll       ← SHVDN (comes in the same zip)
ScriptHookVDotNet3.dll       ← SHVDN
```

**2. `scripts` folder** (inside the game root)

```
scripts\
├── SPAII.dll
├── LemonUI.SHVDN3.dll
├── iFruitAddon2.dll
└── SPA II\
    ├── Garages\
    ├── Sounds\
    └── SPAII Setting.exe
```

**Steps**

1. **Back up** `scripts\SPA II` and your GTA V save games.
2. Delete any old `SPAII.dll`, `INMNativeUI.dll`, `Metadata.dll` and the old `iFruitAddon2.dll` from `scripts`.
3. Copy the **contents** of the release folder `To your scripts folder!!!` into `scripts`. Don't copy the folder itself: the result must be `scripts\SPAII.dll`.
4. Delete every old `ScriptHookVDotNet*` file from the game root and copy the SHVDN files fresh, so they're all the same version.
5. Start the game, load a **story mode** save, and wait until you control your character. The mods load by themselves.

`modconfig.ini` is created on the first run in `scripts\SPA II`.

### ⚠️ Important
- **Don't enter GTA Online** with ScriptHookV installed. Remove `dinput8.dll` if you want to play online.
- **F4 opens the SHVDN console. It does not load mods**, and pressing it can close the game on mismatched files. Mods load automatically at startup. To test changes, close and reopen the game.
- Don't run two ASI loaders at once (for example `dinput8.dll` together with another loader renamed `xinput1_4.dll`).

---

## 🎮 How to use

- **Buying a property:** walk (on foot) to the **"For Sale" sign in front of the building**, stand next to it, and press **E**. The door is only for properties you already own.
- **Entering a garage:** go to the garage entrance on foot or in a car. The screen shows "Loading..." and you appear in the garage.
- **Managing cars:** stand on the **blue circle** inside the garage and press **E**. Pick a car, then choose **Move** (to an empty slot, or swap with another car), **Remove**, or **Plate**.
- **Taking a car out:** drive towards the exit. The car appears outside, ready to drive.
- **Checking the mod loaded:** the map shows property icons, and `SPA II.log` (game root folder) contains `[STARTUP] SPA II iniciado ... blips N` and `[STARTUP] primeiro tick executado`.

---

## 🩺 Troubleshooting

| Symptom | Likely cause and fix |
|---|---|
| **Nothing loads, no icons on the map** | ScriptHookV or SHVDN doesn't support your game build, or the files are mixed versions. Update both and recopy **all** `ScriptHookVDotNet*` files from the same package. |
| **No `[STARTUP]` lines in `SPA II.log`** | SHVDN didn't load the mod. Check `ScriptHookVDotNet.log` and `ScriptHookV.log` in the game root. |
| **Game closes when pressing F4** | F4 is the SHVDN console and it crashes on mismatched or unsupported files. Don't press it and fix the versions. You can set `ConsoleKeyBinding=None` in `ScriptHookVDotNet.ini`. |
| **A message asks you to check the script and updates** | Usually a version mismatch. Update ScriptHookV and SHVDN to versions that support your game build. |
| **Can't buy a property** | You must be at the **for-sale sign**, on foot, within about 1.5 m. |
| **Icons show but the purchase prompt never appears** | Look for a line starting with `LoadRuntime` in `SPA II.log` and report it. |
| **Lines saying `etapa '...' falhou` in the log** | One startup part failed. The message below it says which and why. |
| **Lines starting with `[TICK` in the log** | An error in one part of the game loop was caught. Please report it. |
| **Game closes when going online** | Expected with ScriptHookV. Remove `dinput8.dll` to play online. |

---

## ⚙️ Configuration

`scripts\SPA II\modconfig.ini`, section `[PERFORMANCE]` (created on first run):

```ini
[PERFORMANCE]
UnloadDistance=250
LoadDistance=200
SweepIntervalMs=500
MaxLoadsPerSweep=2
InteriorReleaseGraceSec=20
JanitorIntervalSec=30
GcThresholdMB=150
ForceGcMinutes=10
ModelLoadTimeoutMs=3000
InteriorReadyTimeoutMs=4000
IplTimeoutMs=5000
```

| Setting | What it does |
|---|---|
| `UnloadDistance` / `LoadDistance` | Metres at which a property unloads / loads. `LoadDistance` is always kept below `UnloadDistance`. |
| `SweepIntervalMs` | How often distances are checked. |
| `MaxLoadsPerSweep` | Max properties loaded per check (smooths spikes in dense areas). |
| `InteriorReleaseGraceSec` | Seconds an unused interior stays loaded before it's released. |
| `JanitorIntervalSec` | How often the cleanup runs. |
| `GcThresholdMB` / `ForceGcMinutes` | When to force a garbage collection. |
| `*TimeoutMs` | Time limits for model, interior and IPL loading. |

Invalid values are corrected automatically. To use even less memory, lower `UnloadDistance`. If you get stutter arriving in a dense area, raise `SweepIntervalMs` or lower `MaxLoadsPerSweep`.

**Measuring memory:** set `DebugMode=True` in the `[SETTING]` section. About every 30 seconds the log records a line like:

```
Janitor: heap 87 MB | loaded 3/102 | pins 1 | menus 18 | tags 2 | outVeh 1
```

During a long session `heap` and `pins` level off instead of climbing.

---

## 🐛 Reporting problems

Open an issue and include:
- Your **game build** (GTA5.exe → Properties → Details → Product version).
- Your **ScriptHookV** and **SHVDN** versions.
- `SPA II.log`, `ScriptHookVDotNet.log` and `ScriptHookV.log` (all in the game root).
- What you were doing when it happened.

---

## 🛠️ Building from source

The source is **VB.NET** targeting .NET Framework 4.8. Reference DLLs are in `SPAII/lib`. Open `SPAII/SPAII.vbproj` in Visual Studio and build.

New code lives in `SPAII/Core`:

| File | Purpose |
|---|---|
| `ApartmentManager.vb` | Distance checks and per-property load/unload |
| `Settings.vb` | Performance settings |
| `InteriorService.vb` | Interior pin/unpin and IPL tracking |
| `Janitor.vb` | Periodic cleanup and garbage collection |
| `GarageSlots.vb` | Safe slot assignment, moving and self-repair |
| `GarageTransition.vb` | Loading screen for entering and leaving garages |
| `LemonCompat.vb` | LemonUI bridge for the original menu code |
| `VehicleCompat.vb`, `MetadataCompat.vb` | SHVDN3 vehicle API and the `Metadata.dll` replacement |

---

## 🙏 Credits

Based on **SPA II – Single Player Apartment Remastered** by Zettabyte Technology.

Built on [ScriptHookV](https://dev-c.com/GTAV/scripthookv) (Alexander Blade), [ScriptHookVDotNet](https://github.com/scripthookvdotnet/scripthookvdotnet) (crosire, Kagikn and contributors), [LemonUI](https://github.com/LemonUIbyLemon/LemonUI) (LemonUIbyLemon) and [iFruitAddon2](https://github.com/Bob74/iFruitAddon2) (Bob74).
