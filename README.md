# SPA III (Single Player Apartment) – SHVDN3 + LemonUI Port, with Dynamic Unloading

## Requirements

| Dependency | Notes |
|---|---|
| **GTA V** (Legacy/Premium) | Use the build supported by your ScriptHookV and your SHVDN. |
| **ScriptHookV** | A version compatible with your game build. |
| **ScriptHookVDotNet 3.x** (compiled against 3.6.0) | Use a 3.x release that supports your build. **Remove SHVDN2**, because the two cannot coexist. |
| **LemonUI.SHVDN3** 2.2 | Included in the package (`LemonUI.SHVDN3.dll`). |
| **iFruitAddon2 v3.1.1** | Included in the package. The old (SHVDN2) version **will not work**. |
| **.NET Framework 4.8** | Already included in an up-to-date Windows 10/11. |

**No longer required:** `INMNativeUI.dll`, `Metadata.dll` and `ScriptHookVDotNet2.dll`.

## Installation

1. In your `scripts` folder, **delete** the old `SPAII.dll`, `INMNativeUI.dll`, `Metadata.dll` and the old `iFruitAddon2.dll`.
2. Copy `SPAII.dll`, `LemonUI.SHVDN3.dll` and `iFruitAddon2.dll` from the package into `scripts`.
3. Keep the `scripts\SPA II` folder. Your garage saves remain compatible because the XML format did not change. **Back it up first.**

## What was fixed

### Performance and memory
- Interiors were pinned in memory (`PIN_INTERIOR_IN_MEMORY`) and never released. They are now released when no longer needed.
- IPLs requested by the mod were never removed. They are now removed together with their interior.
- Menus, for-sale signs and doors for all ~100 buildings were created at once. They now exist only for buildings within 200 m, and are removed beyond 250 m.
- The stats scaleforms (10 movies) and `instructional_buttons` stayed loaded the whole time. They now load on demand.
- Interior logic ran once per building, every frame. It now runs once per frame.
- The INI file was read every frame, for every building. It is now read once.
- Blips were duplicated on every refresh. They are now recreated without duplicates.
- The list of vehicles outside the garage was never cleaned. It is now pruned periodically.
- When the script unloads, less is left behind (menus, props, blips, pins).
- The log is capped at 2 MB and no longer repeats the same error in a loop, which avoids disk writes every frame.

### Stability
- **LemonUI** menus replace NativeUI, to prevent crashes when opening the closet and garage menus.
- Vehicle XML files are written to a temporary file and validated before replacing the original. A `.bak` is kept, and a corrupted XML is recovered from it.
- If a DLC vehicle model goes missing, the mod logs it and carries on, **without deleting the XML**. The car comes back when the DLC does.
- Each vehicle loads with its own error handling, so one broken car does not stop the others.
- When saving a car, the old file is deleted only after the new one has been saved. If something fails midway, the camera, HUD and screen fade are restored.
- Waits that could freeze the game (model loading, IPL swapping) now have time limits.
- Removed a write to game memory (`Game.Globals`) that was only valid up to build 2060 and could corrupt memory on newer builds.
- Decorators (which required a memory patch) were replaced with an internal registry.

### Side effects
The "a building in use is never unloaded" rule is meant to keep you unaffected inside apartments and garages. The "for sale" blips remain across the whole map.

## How to test

Set `DebugMode=True` in `modconfig.ini`. About every 30 s, `SPA II.log` records a line like this:

```
Janitor: heap 87 MB | loaded 3/102 | pins 1 | menus 18 | tags 2 | outVeh 1
```

- [ ] **Distance loading:** walk up to a building. Between 200 m and the door, the purchase menu should work and the for-sale sign should appear. Move more than 250 m away and `loaded` should drop.
- [ ] **Buying and entering:** buy an apartment and enter it. Change the style (IPL) and leave. The interior should load without you falling through the floor.
- [ ] **Garages:** repeat with 2, 6 and 10-car garages. Save a car, leave, and enter again, then check that the car returns with its mods and colors.
- [ ] **Missing DLC vehicle:** uninstall the DLC for a saved car and enter the garage. A line should appear in the log, the other cars should load, and the XML should stay in the folder.
- [ ] **Menus:** open and close the closet, garage, phone (mechanic/insurance), real estate and wardrobe menus. Confirm that clothing previews work while navigating and that the Back button returns to the previous menu.
- [ ] **Long session:** play for 1 to 2 hours, entering and leaving several properties. `heap` and `pins` should level off instead of climbing indefinitely. Note your FPS too.
- [ ] **Tuning:** if you get stutter when arriving in a dense area, increase `SweepIntervalMs` or lower `MaxLoadsPerSweep`. To save more memory, lower `UnloadDistance` (always keep it above `LoadDistance`).

### What to report
Open an issue with your `SPA II.log`, your game build, your SHVDN version, and what you were doing when the problem happened. If a menu is unresponsive, say which one.

## Optional settings

In `modconfig.ini`, section `[PERFORMANCE]` (created on first run):

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

Invalid values are corrected automatically. For example, `LoadDistance` never ends up greater than or equal to `UnloadDistance`.

## Building from source

The source is VB.NET targeting .NET Framework 4.8. Reference DLLs are in `SPAII/lib`. Open `SPAII.vbproj` in Visual Studio and build.

## Credits

Original SPA II mod by its authors. Libraries: [ScriptHookVDotNet](https://github.com/scripthookvdotnet/scripthookvdotnet) (crosire and contributors), [LemonUI](https://github.com/LemonUIbyLemon/LemonUI) (LemonUIbyLemon), [iFruitAddon2](https://github.com/Bob74/iFruitAddon2) (Bob74).
