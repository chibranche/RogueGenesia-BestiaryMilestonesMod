# Extended Bestiary Milestones

A mod for [Rogue Genesia](https://store.steampowered.com/app/2089120/Rogue_Genesia/)
that adds 45 "Special Milestones" on top of the vanilla 5 per monster, each granting
+5% bestiary XP. Special milestones are display-only and **do not** count toward
existing bestiary completion achievements.

## Special milestone thresholds

| Tier | Vanilla M5 | Special M1 | Step | Special M45 |
|------|-----------:|-----------:|-----:|------------:|
| Normal   | 10 000 | 20 000 | 10 000 | 460 000 |
| Elite    | 625    | 1 000  | 500    | 23 000  |
| Boss     | 16     | 32     | 16     | 736     |
| MiniBoss | 81     | 160    | 80     | 3 680   |

XP bonus = vanilla milestone bonus + 0.05 × special milestone count.

## Building

Requires:
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- A Rogue Genesia install on the **Modded** branch (the `Managed` folder must exist
  at `<game install>/Modded/Rogue Genesia_Data/Managed/`)
- Visual Studio 2022 (or `dotnet build` from the command line)

### Configure your game install path

The csproj defaults to `E:\SteamLibrary\steamapps\common\Rogue Genesia\Modded\...`.
If your install is elsewhere, set the `RogueGenesiaManaged` environment variable
before building. Persist it once with PowerShell (run once per machine):

```powershell
[Environment]::SetEnvironmentVariable(
  'RogueGenesiaManaged',
  'C:\Path\To\SteamLibrary\steamapps\common\Rogue Genesia\Modded\Rogue Genesia_Data\Managed',
  'User')
```

Then restart Visual Studio / your terminal so it picks up the variable.

Or pass it on the command line for a one-off build:

```powershell
dotnet build -c Release -p:RogueGenesiaManaged="C:\Path\To\Managed"
```

You can also override the install destination with `ModInstallFolder` if you don't
want the build to copy into the game's `Mods` folder.

### Build

In Visual Studio: **Build → Build Solution** (Ctrl+Shift+B).
From the command line: `dotnet build -c Release` from the repo root.

After a successful build, the mod files are copied automatically to
`<game install>/Modded/Mods/BestiaryMilestones/`. Restart the game and enable
the mod from the in-game mod manager.

## Project layout

```
BestiaryMilestonesMod.sln
BestiaryMilestonesMod/
├── BestiaryMilestonesMod.csproj    Project + auto-install build target
├── Plugin.cs                        Mod entry, threshold math
├── Patches.cs                       Harmony patches (XP bonus + UI)
├── ModInfo.rgmod                    Mod metadata read by the game
├── ModPreview.png                   (optional) 800×400 Workshop preview
└── ModIcon.png                      (optional) 32×32 in-game list icon
```

## Steam Workshop

Once built and tested locally, upload from the in-game mod manager:
*Mods → select this mod → Upload to Workshop*. The first upload reserves
a Workshop ID and writes it back into `ModInfo.rgmod` — commit the
updated file so future uploads target the same Workshop entry.

## License

MIT — see [LICENSE](LICENSE).
