# Project handoff

Read `AGENTS.md`. Theme: Bling / Golden Glamour. Current direction after the organizer meeting: energetic DJ/Dandiya visuals and DJ-name takeovers; daytime may use one approved event overview still (not chosen yet).

Show machine: M1 Max Mac running Resolume **Avenue 7.28.1** (not Arena) and Wire 7.28.1. Start with `shaders/WIRE-TUTORIAL.md`, then `MAC-HANDOFF.md`. Eight shaders, including the new `GoldTitleTakeover.fs` DJ-title effect, have browser validation. The two files in `shaders/examples/` have static checks only and still need native compilation/rendering. No compiled Wire patches exist.

Show media is already on the Mac in ignored `show-media/` (`DJ/` with `Titles/`, `YouTubeGold/`, `DJ Mumbai Rani/`; `Dandiya/`; `Daytime/`; `Ads/`; `Shubho Bijoya/`), copied from USB dGold with SHA-256 verification. dGold stays untouched as the backup. `show-media/_alley/ALLEY-LIST.md` lists clips staged for DXV3 HQ conversion in Alley; after conversion, move the outputs into their prefixed folders (`DJ__`, `Dandiya__`, `Daytime__`) and remove the staging copies. Wall calibration: `outputs/calibration/wall-check-1248x540.png`.

Division of work: Claude Code writes and tests GLSL/ISF shaders and manages files (show-media, ffmpeg, Alley staging, Git). A Claude Desktop Chat project, which has the Resolume Avenue/Wire MCP extensions, builds compositions and operates the software. Claude Code does not control Avenue.

The Windows GPU is occupied. Do not generate, render shader previews, or use that GPU until explicitly released. Preserve existing stills, loops, workflows and metadata. The old meeting presentation was removed; use `outputs/index.html` and `Ma/index.html` only as asset archives. Downloaded footage and ads stay in `show-media/`, never in Git. Keep this project simple.
