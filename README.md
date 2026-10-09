<div align="center">

# 🏙️ SPA III

### Unofficial community update of **SPA II – Single Player Apartment**
**Smoother performance · No more memory leak · Modern menus · Safer garage saves**

![GTA V](https://img.shields.io/badge/GTA%20V-Legacy-green)
![SHVDN](https://img.shields.io/badge/ScriptHookVDotNet-3.x-blue)
![UI](https://img.shields.io/badge/UI-LemonUI%202.2-orange)
![Status](https://img.shields.io/badge/status-community%20testing-yellow)
![Online](https://img.shields.io/badge/GTA%20Online-not%20supported-red)

</div>

> **Not affiliated with the original author.** SPA II was created by **Zettabyte Technology** (© 2015–2021). This is an unofficial, fan-made update based on the original source, shared with full credit. See [Credits and legal](#-credits-and-legal).

<!-- Add screenshots/GIFs here: purchase at the sign, apartment menu, garage with cars. -->

---

## 📑 Contents

[Quick start](#-quick-start) · [Status](#-status) · [What's new](#-whats-new) · [Requirements](#-requirements) · [Installation](#-installation) · [Checking it loaded](#-checking-that-it-loaded) · [Troubleshooting](#-troubleshooting) · [Configuration](#%EF%B8%8F-configuration) · [Help us test](#-help-us-test) · [Reporting problems](#-reporting-problems) · [Known limitations](#-known-limitations) · [Building](#-building-from-source) · [Credits and legal](#-credits-and-legal)

---

## ⚡ Quick start

1. Install **ScriptHookV** (official site) and **ScriptHookVDotNet 3.x nightly** in the game root, **matching your game build**.
2. Copy the contents of the release folder into `scripts\` (see [Installation](#-installation)).
3. Start the game in **story mode**. Don't press F4. Mods load by themselves.
4. To **buy a property, go on foot to the "For Sale" sign in front of the building** and press **E**.

That's it. If something doesn't work, jump to [Troubleshooting](#-troubleshooting).

---

## 📊 Status

| | |
|---|---|
| ✅ **Confirmed by the author** | Loads and runs on a GTA V **Legacy** install. Buying a property at its for-sale sign works. Several cars were stored in a garage and stayed saved. A 1+ hour session ran with no crashes and no FPS problems. |
| ⚠️ **Not guaranteed** | Not tested on every game build, property, garage or menu. See [Help us test](#-help-us-test). |
| ❌ **Not supported** | GTA Online, and the GTA V *Enhanced* edition (untested). |

**Tested by the author on:**

| Component | Version |
|---|---|
| Release | rev3 (based on SPA II 2.0.5) |
| GTA V (Legacy) | `[FILL IN: right-click GTA5.exe → Properties → Details → Product version]` |
| ScriptHookV | `[FILL IN: version of the ScriptHookV.dll you used]` |
| ScriptHookVDotNet | v3.7.0-nightly.191 |

Whether it works for you depends mostly on **ScriptHookV and SHVDN supporting your exact game build**. If they don't, no script mod loads, including this one.

> There are no published benchmark numbers yet. The author reports smooth gameplay, and the `DebugMode` log (see [Configuration](#%EF%B8%8F-configuration)) lets anyone measure memory over time. Community results are welcome.

---

## ✨ What's new

### At a glance

| Area | SPA II 2.0.5 | This update |
|---|---|---|
| Game scripting API | ScriptHookVDotNet 2 | **ScriptHookVDotNet 3** |
| Menus | INMNativeUI | **LemonUI 2.2** |
| Interiors | Pinned in memory forever | **Released when no longer needed** |
| IPLs | Requested, never removed | **Tracked and removed with their interior** |
| Menus, signs and doors | Created for every property at startup | **Only for properties within 200 m** |
| Scaleform movies | 11 loaded at startup, never freed | **Loaded on demand, freed afterwards** |
| Work per frame | Repeated for every property | **Once per frame, nearby properties only** |
| Garage save files | Written directly to the target | **Written safely, validated, `.bak` kept** |
| A saved car whose DLC is missing | Could break loading | **Skipped and logged, XML kept** |
| Startup failure in one part | Could stop the whole mod | **Isolated and logged** |

### 🚀 Performance and memory leak
- **Distance-based loading.** A property has its menus, for-sale sign and doors only while you are near it. It loads at **200 m or less** and unloads beyond **250 m**. The gap prevents load/unload flickering. Both values are configurable.
- **A property you are inside is never unloaded**, even though its interior is far from its door. Nothing unloads while a menu is open.
- **Interiors are released.** The original pinned every visited interior (`PIN_INTERIOR_IN_MEMORY`) and never unpinned it. They are now tracked and released after they stop being needed. On re-entry the mod waits (with a timeout) for the interior to be ready, so you don't fall through the floor.
- **Scaleforms load on demand** instead of all at startup.
- **Much less per-frame work.** Interior logic ran once per property, every frame. It now runs once per frame. The config file is no longer read from disk every frame.
- **No duplicated blips** on refresh.
- **Periodic cleanup** removes dead vehicles from internal lists and purges stale IDs. The garbage collector runs only past a heap threshold (or every N minutes) and never during menus.
- **Safer log.** Capped at 2 MB, with repeated identical errors collapsed.
- **Clean shutdown.** Menus, props, blips, interior pins and scaleforms are released when scripts unload.

### 🛡️ Stability and save safety
- **Vehicle XML is written safely**: temporary file, validation, then swap, with a `.bak` copy. A corrupted file is recovered from the backup, and one bad file no longer hides your other cars.
- **Missing DLC vehicles don't break the garage.** The mod logs it and carries on, **keeping your XML** so the car comes back when the DLC does.
- **Per-vehicle error handling.** One broken car doesn't stop the rest from loading.
- **Safer saving.** The old file is deleted only after the new one is saved. If something fails midway, the camera, HUD and screen fade are restored.
- **No more indefinite waits.** Model loading, IPL swapping and interior loading have time limits.
- **Fixed enter/exit cameras.** The apartment and garage camera transitions passed a Boolean where SHVDN3 expects a number (`True` became `-1`). They now pass `1`/`0`.
- **Fault-tolerant startup.** Each startup step is isolated. A failure in one part is logged and doesn't stop the properties from loading. Missing `scripts\SPA II`, `Garages` and `Sounds` folders are created automatically.
- **Startup is logged**, so you can confirm the mod is running.

### 🔧 Compatibility
- Ported to **ScriptHookVDotNet 3** (natives, vehicle-mod API, tasks, props, cameras, settings).
- **LemonUI** replaces INMNativeUI, avoiding crashes when opening the closet and garage menus.
- **Metadata.dll** (which patched game memory to unlock decorators and was fragile across builds) is replaced by a managed registry for garage vehicle IDs.
- **iFruitAddon2 v3.1.1** (the SHVDN3 build) for the phone contacts.
- Removed a write to game memory (`Game.Globals`). Its index was only valid up to build 2060 and could corrupt memory on newer builds.

### 💾 Unchanged on purpose
Garage saves keep the same XML format, so **existing SPA II saves remain compatible**. Back up `scripts\SPA II` before the first run anyway. DLL, folder and log names still say "SPA II" for that reason.

---

## 📦 Requirements

| Dependency | Notes |
|---|---|
| **GTA V Legacy** | A build that ScriptHookV and SHVDN support. |
| **ScriptHookV** (`ScriptHookV.dll` + `dinput8.dll`) | Official site: <https://dev-c.com/GTAV/scripthookv>. It must support **your exact** game build. At the time of writing the page lists v3889.0 for Legacy 1.0.3889.0. |
| **ScriptHookVDotNet 3.x** | Tested with **v3.7.0-nightly.191**: <https://github.com/scripthookvdotnet/scripthookvdotnet-nightly/releases>. The `.asi` and both `.dll` files must come from the **same** version. |
| **LemonUI.SHVDN3** 2.2 | Included in the release package. |
| **iFruitAddon2 v3.1.1** | Included in the release package. The old SHVDN2 version will not work. |
| **.NET Framework 4.8** | Included in an up-to-date Windows 10/11. |
| **Visual C++ Redistributable 2019 x64** | Needed by ScriptHookV / SHVDN. |

**No longer needed:** `INMNativeUI.dll` and `Metadata.dll`.

> ScriptHookV is © Alexander Blade and is **not included** in this repository or release. Download it from the official site.

---

## 🔧 Installation

**Game root folder (next to `GTA5.exe`)**

```
ScriptHookV.dll              ← ScriptHookV
dinput8.dll                  ← ScriptHookV (ASI loader)
ScriptHookVDotNet.asi        ← SHVDN
ScriptHookVDotNet.ini        ← SHVDN
ScriptHookVDotNet2.dll       ← SHVDN (ships in the same zip)
ScriptHookVDotNet3.dll       ← SHVDN
```

**`scripts` folder (inside the game root)**

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

1. **Back up** `scripts\SPA II` and your GTA V save games.
2. In `scripts`, **delete** any old `SPAII.dll`, `INMNativeUI.dll`, `Metadata.dll` and the old `iFruitAddon2.dll`.
3. Copy the **contents** of the release folder `To your scripts folder!!!` into `scripts`. Don't copy the folder itself: the result must be `scripts\SPAII.dll`.
4. Delete every old `ScriptHookVDotNet*` file from the game root and copy the SHVDN files fresh, so they are all the same version.
5. Start the game, load a **story mode** save, and wait until you control your character.

`modconfig.ini` is created on the first run in `scripts\SPA II`.

### ⚠️ Important
- **Never enter GTA Online** with ScriptHookV installed. Remove `dinput8.dll` to play online.
- **F4 opens the SHVDN console. It does not load or reload mods.** Mods load automatically at game start. To test changes, close and reopen the game rather than reloading scripts.
- Don't run **two ASI loaders** at once (for example `dinput8.dll` together with another loader renamed `xinput1_4.dll`). Don't delete the `xinput1_4.dll` in `C:\Windows\System32`; that's a normal system file.

---

## ✅ Checking that it loaded

1. Open the map. Property icons should be scattered across it.
2. **To buy, go to the "For Sale" sign in front of the building**, on foot (not in a vehicle), and stand right next to it (within about 1.5 m). A help prompt appears; press the action key (**E** on keyboard). The building's door is only for entering properties you already own.
3. Open `SPA II.log` in the game root folder. It should contain:
   ```
   [STARTUP] SPA II iniciado | jogo <version> | predios N | blips N
   [STARTUP] primeiro tick executado: o script esta rodando
   ```
   (The log messages are in Portuguese.)

---

## 🩺 Troubleshooting

| Symptom | Likely cause and fix |
|---|---|
| **Nothing loads, no icons on the map** | ScriptHookV or SHVDN doesn't support your game build, or the files are mixed versions. Check your game version, update both, and recopy **all** `ScriptHookVDotNet*` files from the same package. |
| **No `[STARTUP]` lines in `SPA II.log`** | SHVDN didn't load the mod. Check `ScriptHookVDotNet.log` and `ScriptHookV.log` in the game root. |
| **Lines saying `etapa '...' falhou`** | One startup part failed. The message below it says which and why. Please report it. |
| **Game closes when pressing F4** | F4 is the SHVDN console, and it crashes on mismatched or unsupported SHVDN files. Don't press it, and fix the versions. You can set `ConsoleKeyBinding=None` in `ScriptHookVDotNet.ini`. |
| **A message asks you to check the script and for updates** | Usually a version mismatch. Update ScriptHookV and SHVDN to versions that support your game build. |
| **Can't buy a property** | You must be at the **for-sale sign**, on foot, within ~1.5 m. The door doesn't sell. |
| **Icons show but the purchase prompt never appears** | Look for a line starting with `LoadRuntime` in `SPA II.log` and report it. |
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
| `InteriorReleaseGraceSec` | Seconds an unused interior stays pinned before release. |
| `JanitorIntervalSec` | How often the cleanup runs. |
| `GcThresholdMB` / `ForceGcMinutes` | When to force a garbage collection. |
| `*TimeoutMs` | Time limits for model, interior and IPL loading. |

Invalid values are corrected automatically. To save more memory, lower `UnloadDistance`. If you get stutter arriving in a dense area, raise `SweepIntervalMs` or lower `MaxLoadsPerSweep`.

**Measuring memory:** set `DebugMode=True` in the `[SETTING]` section. About every 30 seconds the log records:

```
Janitor: heap 87 MB | loaded 3/102 | pins 1 | menus 18 | tags 2 | outVeh 1
```

During a long session `heap` and `pins` should level off instead of climbing.

---

## 🧪 Help us test

✅ = confirmed by the author. Everything else is still open: please try it and report what you find.

- [x] ✅ Buy a property at its for-sale sign through the purchase menu.
- [x] ✅ Store several cars in a garage, leave, and confirm they are saved and present.
- [x] ✅ Play 3+ hour with no crashes and no FPS problems.
- [x] ✅ Check that stored cars keep their **mods and colors** after leaving and re-entering the garage.
- [x] ✅ Walk away from a property and confirm it unloads (`loaded` drops in the `DebugMode` log).
- [x] ✅ Enter a property you own, **change its style (IPL)**, and leave.
- [x] ✅ Use 2, 6 and 10-car garages.



---

## 📌 Known limitations

- Property blips stay on the map for all properties (they are the "for sale" icons). What is unloaded is menus, signs, props, interior pins and IPLs.
- The menu layer maps the old menu events onto LemonUI. Two behaviours differ on purpose: the selection-changed event doesn't fire when a menu opens, and the menu-closed event fires only when the player backs out (not when code switches menus).
- Some original game logic was left as it was, even where it looks odd.
- Not tested on the Enhanced edition.

---

## 🛠️ Building from source

The source is **VB.NET** targeting .NET Framework 4.8. Reference DLLs are in `SPAII/lib` (ScriptHookVDotNet3, LemonUI.SHVDN3, iFruitAddon2). Open `SPAII/SPAII.vbproj` in Visual Studio and build.

New code lives in `SPAII/Core`:

| File | Purpose |
|---|---|
| `ApartmentManager.vb` | Distance checks and per-property load/unload |
| `Settings.vb` | Performance settings (`[PERFORMANCE]`) |
| `InteriorService.vb` | Interior pin/unpin and IPL tracking |
| `Janitor.vb` | Periodic cleanup and garbage collection |
| `LemonCompat.vb` | LemonUI bridge for the original menu code |
| `VehicleCompat.vb`, `MetadataCompat.vb` | SHVDN3 vehicle API and the `Metadata.dll` replacement |

---

## 🙏 Credits and legal

- **SPA II** (Single Player Apartment Remastered): original work by **Zettabyte Technology**, © 2015–2021. All credit for the mod itself goes to the original author.
- Libraries: [ScriptHookVDotNet](https://github.com/scripthookvdotnet/scripthookvdotnet) (crosire, Kagikn and contributors), [LemonUI](https://github.com/LemonUIbyLemon/LemonUI) (LemonUIbyLemon), [iFruitAddon2](https://github.com/Bob74/iFruitAddon2) (Bob74), ScriptHookV (Alexander Blade).
- Update and port: `[FILL IN: your name / handle]`.
- **License note:** the original source repository did not include a license file, so the original author's rights apply by default. This project is shared as a community update with full credit. If the original author asks for changes or removal, they will be honored.

*This is a fan-made update. It is not an official release of SPA II, and "SPA III" is a community name, not an announcement by the original author.*

