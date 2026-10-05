# SPA III – Unofficial Community Update of SPA II (Single Player Apartment)

**A performance and stability update of SPA II for GTA V Legacy (Premium Edition), ported to ScriptHookVDotNet 3 and LemonUI.**

> **Not affiliated with the original author.** SPA II was created by **Zettabyte Technology** (© 2015–2021). This is an unofficial, community-maintained update based on the original source. See [Credits and legal](#credits-and-legal).

---

## Status (please read)

| | |
|---|---|
| ✅ **Confirmed** | The mod loads and runs in-game on the author's GTA V **Legacy** install. |
| ⚠️ **Not guaranteed** | It has **not** been tested on every game build, every property, every garage or every menu. |
| ❌ **Not supported** | GTA Online, and the GTA V *Enhanced* edition (untested). |

**Tested by the author on:**

| Component | Version |
|---|---|
| GTA V (Legacy) | `[FILL IN: right-click GTA5.exe → Properties → Details → Product version]` |
| ScriptHookV | `[FILL IN: version of the ScriptHookV.dll you used]` |
| ScriptHookVDotNet | v3.7.0-nightly.191 |

Whether it works for you depends mostly on **ScriptHookV and SHVDN supporting your exact game build**. If they don't, no script mod will load, including this one. Please report your results (see [Reporting problems](#reporting-problems)) so the compatibility list can grow.

---

## What was updated (everything that differs from SPA II 2.0.5)

### Compatibility
- Ported from **ScriptHookVDotNet 2 to ScriptHookVDotNet 3** (renamed natives, vehicle-mod API, tasks, props, cameras, settings).
- Replaced **INMNativeUI** with **LemonUI 2.2** for all menus and the purchase banner, to avoid crashes when opening the closet and garage menus.
- Replaced **Metadata.dll** (it patched game memory to unlock decorators, which is fragile across builds) with a managed registry for the garage vehicle IDs.
- Updated to **iFruitAddon2 v3.1.1** (the SHVDN3 build) for the phone contacts.
- Removed a write to game memory (`Game.Globals(...).SetInt(1)`). Its index was only valid up to build 2060 and could corrupt memory on newer builds.

### Performance and memory leak
- **Distance-based loading.** A property only has its menus, for-sale sign and doors while you are near it. It loads at **200 m or less** and unloads beyond **250 m**. The gap between the two values prevents load/unload flickering. Both are configurable.
- **A property you are inside is never unloaded**, even though its interior is far from its door. Nothing unloads while a menu is open.
- **Interiors are released.** The original pinned every visited interior in memory (`PIN_INTERIOR_IN_MEMORY`) and never released it, and never removed the IPLs it requested. They are now tracked and released after they stop being needed. On re-entry the mod waits (with a timeout) for the interior to be ready, so you don't fall through the floor.
- **Scaleforms load on demand.** The original loaded 11 scaleform movies at startup and never freed them.
- **Much less work per frame.** Interior logic ran once per building, every frame. It now runs once per frame, only over nearby properties. The config file is no longer read from disk every frame.
- **Blips are no longer duplicated** on each refresh.
- **Periodic cleanup** removes dead vehicles from internal lists and purges stale IDs. The managed garbage collector runs only when the heap passes a threshold (or every N minutes), and never during menus.
- **Safer log.** Capped at 2 MB, and repeated identical errors are collapsed instead of being written every frame.
- **Clean shutdown.** Menus, props, blips, interior pins and scaleforms are released when scripts are unloaded.

### Stability and save safety
- **Vehicle XML is written safely.** Written to a temporary file, validated, then swapped in, with a `.bak` copy kept. A corrupted XML is recovered from the backup, and a corrupt file no longer hides your other cars.
- **Missing DLC vehicles no longer break the garage.** If a car's model is not available, the mod logs it and carries on, and **keeps your XML** so the car returns when the DLC does.
- **Per-vehicle error handling.** One broken car no longer stops the rest from loading.
- **Saving a car is safer.** The old file is deleted only after the new one is saved, and the camera, HUD and screen fade are restored if something fails midway.
- **No more indefinite waits.** Model loading, IPL swapping and interior loading now have time limits. The for-sale sign spawner no longer recurses without limit.
- **Fixed enter/exit cameras.** The apartment and garage camera transitions passed a Boolean where SHVDN3 expects a number, which turns `True` into `-1`. They now pass `1`/`0`.
- **Fault-tolerant startup.** Each startup step is isolated. A failure in one part (phone, wardrobe, missing folder) is logged and does not stop the properties from loading. Missing `scripts\SPA II`, `Garages` and `Sounds` folders are created automatically.
- **Startup is logged**, so you can confirm the mod is running (see [Checking that it loaded](#checking-that-it-loaded)).

### Unchanged on purpose
Garage save files keep the same XML format, so **existing SPA II saves remain compatible**. Back up `scripts\SPA II` before the first run anyway. The DLL, folder and log names still say "SPA II" for the same reason.

---

## Requirements

| Dependency | Notes |
|---|---|
| **GTA V Legacy** | Use a build that ScriptHookV and SHVDN support. |
| **ScriptHookV** (`ScriptHookV.dll` + `dinput8.dll`) | Download from the official site: <https://dev-c.com/GTAV/scripthookv>. It must support **your exact** game build. At the time of writing the page lists v3889.0 for Legacy 1.0.3889.0. |
| **ScriptHookVDotNet 3.x** | Tested with **v3.7.0-nightly.191**: <https://github.com/scripthookvdotnet/scripthookvdotnet-nightly/releases>. The `.asi` and both `.dll` files must be from the **same** version. |
| **LemonUI.SHVDN3** 2.2 | Included in the release package. |
| **iFruitAddon2 v3.1.1** | Included in the release package. The old SHVDN2 version will not work. |
| **.NET Framework 4.8** | Included in an up-to-date Windows 10/11. |
| **Visual C++ Redistributable 2019 x64** | Required by ScriptHookV / SHVDN. |

**No longer needed:** `INMNativeUI.dll`, `Metadata.dll`, `ScriptHookVDotNet2.dll` as a dependency of this mod.

> ScriptHookV is © Alexander Blade and is **not included** in this repository or release. Download it from the official site.

## Installation

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
3. Copy the **contents** of the release folder `To your scripts folder!!!` into `scripts` (not the folder itself, so the result is `scripts\SPAII.dll`).
4. Delete every old `ScriptHookVDotNet*` file from the game root and copy the SHVDN files fresh, so all of them are the same version.
5. Start the game, load a **story mode** save, and wait until you are in control of your character.

`modconfig.ini` is created on the first run in `scripts\SPA II`.

### Important notes
- **Do not enter GTA Online** with ScriptHookV installed. ScriptHookV closes the game when you go online. Remove `dinput8.dll` to play online.
- **F4 opens the SHVDN console. It does not load or reload mods.** Mods load automatically when the game starts. Pressing F4 closed the game on a setup with mismatched files, so avoid it. To test changes, close and reopen the game instead of reloading scripts.
- Don't run two ASI loaders at once (for example `dinput8.dll` together with another loader renamed `xinput1_4.dll`).

---

## Checking that it loaded

1. Open the map. Property icons should be scattered across it.
2. Walk up to a property for sale. A help prompt to open the purchase menu should appear.
3. Open `SPA II.log` in the game root folder. It should contain:
   ```
   [STARTUP] SPA II iniciado | jogo <version> | predios N | blips N
   [STARTUP] primeiro tick executado: o script esta rodando
   ```
   (The log messages are in Portuguese.) If there is no `[STARTUP]` line, SHVDN did not load the mod. Lines saying `etapa '...' falhou` describe what failed and why.

## Configuration

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

Invalid values are corrected automatically (for example, `LoadDistance` is always kept below `UnloadDistance`). To save more memory, lower `UnloadDistance`. If you get stutter when arriving in a dense area, raise `SweepIntervalMs` or lower `MaxLoadsPerSweep`.

Set `DebugMode=True` in the `[SETTING]` section to log a status line about every 30 seconds:

```
Janitor: heap 87 MB | loaded 3/102 | pins 1 | menus 18 | tags 2 | outVeh 1
```

During a long session `heap` and `pins` should level off instead of climbing indefinitely.

## Help us test

Please try the following and report what you find:

- [ ] Walk up to a property and open the purchase menu. Walk away and confirm it unloads (`loaded` drops in the debug log).
- [ ] Buy an apartment, enter it, change its style (IPL), and leave.
- [ ] Use 2, 6 and 10-car garages: store a car, leave, come back, and check mods and colors.
- [ ] Uninstall the DLC of a saved car and enter the garage (other cars should still load, and the XML should stay).
- [ ] Open the closet/wardrobe, garage, phone (mechanic/insurance) and real estate menus. Check that the Back button works and clothing previews update while browsing.
- [ ] Play 1–2 hours entering and leaving several properties and watch `heap` and `pins`.

## Reporting problems

Open an issue and include:
- Your **game build** (GTA5.exe → Properties → Details → Product version).
- Your **ScriptHookV** and **SHVDN** versions.
- `SPA II.log`, `ScriptHookVDotNet.log` and `ScriptHookV.log` (all in the game root).
- What you were doing when it happened, and which menu if one was unresponsive.

## Known limitations

- Property blips stay on the map for all properties (they are the "for sale" icons). What is unloaded is menus, signs, props, interior pins and IPLs.
- The menu layer maps the old menu events onto LemonUI. Two behaviours differ on purpose: the selection-changed event does not fire when a menu opens, and the menu-closed event fires only when the player backs out (not when the code switches menus).
- Some original game logic was left as it was, even where it looks odd.
- Not tested on the Enhanced edition.

## Building from source

The source is **VB.NET** targeting .NET Framework 4.8. Reference DLLs are in `SPAII/lib` (ScriptHookVDotNet3, LemonUI.SHVDN3, iFruitAddon2). Open `SPAII/SPAII.vbproj` in Visual Studio and build. New files are in `SPAII/Core`:

| File | Purpose |
|---|---|
| `ApartmentManager.vb` | Distance checks and per-property load/unload |
| `Settings.vb` | Performance settings (`[PERFORMANCE]`) |
| `InteriorService.vb` | Interior pin/unpin and IPL tracking |
| `Janitor.vb` | Periodic cleanup and garbage collection |
| `LemonCompat.vb` | LemonUI bridge for the original menu code |
| `VehicleCompat.vb`, `MetadataCompat.vb` | SHVDN3 vehicle API and the `Metadata.dll` replacement |

## Credits and legal

- **SPA II** (Single Player Apartment Remastered): original work by **Zettabyte Technology**, © 2015–2021. All credit for the mod itself goes to the original author.
- Libraries: [ScriptHookVDotNet](https://github.com/scripthookvdotnet/scripthookvdotnet) (crosire, Kagikn and contributors), [LemonUI](https://github.com/LemonUIbyLemon/LemonUI) (LemonUIbyLemon), [iFruitAddon2](https://github.com/Bob74/iFruitAddon2) (Bob74), ScriptHookV (Alexander Blade).
- Update and port: `[FILL IN: your name / handle]`.
- **License note:** the original source repository did not include a license file, so the original author's rights apply by default. This project is shared as a community update with full credit. If the original author asks for changes or removal, they will be honored.

*This is a fan-made update. It is not an official release of SPA II, and "SPA III" is a community name, not an announcement by the original author.*

