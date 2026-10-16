# Mac handoff

The meeting changed the priority: go further on DJ night and Dandiya with energetic motion and title takeovers. Daytime may use one approved event overview still. Continue shader coding and Wire setup on the M1 Max / 64 GB Mac. The Windows GPU is occupied; do not use it until the user releases it.

## Start here

1. Done: the repository is on the Mac and media is copied into `show-media/` (see below). The earlier USB Project snapshot and meeting presentation are not needed.
2. Read [the Wire + GLSL tutorial](shaders/WIRE-TUTORIAL.md). Build RadialRave as a Source and the PrismaticTitle + TitleTakeover chain as an Effect. The two tutorial examples have not yet been rendered or compiled in Wire.
3. Create a 1248 x 540 composition in Resolume Avenue 7.28.1 (the show app; Arena is also installed but not used). Check it with `outputs/calibration/wall-check-1248x540.png`. Confirm the processor's input signal with Sam before configuring output.
4. Done except conversion: media is in ignored `show-media/`. Run the two folders in `show-media/_alley/` through Alley (DXV3 High Quality, no alpha), then move the outputs into place as `ALLEY-LIST.md` describes. Save the composition and consolidated patches in `show-media/` too.
5. Make one title clip each for DJ Edwin, DJ Mumbai Rani and DJ Dinu. Set readable size individually. Verify clip-trigger restart, readable hold, zoom-through and fade-out.
6. Rehearse three to five layers, save, and reopen with the USB disconnected. Camera work comes after playback works.

## Show media on the Mac

| Folder | Contents |
| --- | --- |
| `show-media/DJ/` | 6 Gold Sparkles playlist clips (bursts, light paths, streaks) and `LED DJ Visuals/` (tunnels, trippy, sunsets, party lights) |
| `show-media/Dandiya/` | 8 Gold Sparkles playlist clips (stars, bokeh, glitter) and `LED Dandiya Visuals/` (mandalas, kaleidoscopes, tunnels) |
| `show-media/Daytime/` | LED Durga Puja films, Sindur Khela/Bijoya, genai images. The approved overview still is not chosen yet |
| `show-media/Ads/` | All confirmed 2026 ads, including `incredible-india-promos/`. More are coming. The portrait ad needs gold side fill |
| `show-media/Titles/` | Transparent DJ title PNGs, masters and SVGs |

Playlist clip 03 has a faint stock watermark. The hour-long party clips should be trimmed before any DXV conversion. Prior-year 2024 Europe and 2025 Royal folders were skipped because this year's theme is Bling / Golden Glamour. dGold remains the untouched backup.

## Layers, bottom to top

| Layer | DJ / Dandiya | Daytime / performance |
| --- | --- | --- |
| 1 | Playlist footage or procedural source | Approved overview or quiet backing |
| 2 | Alternate pattern or kaleidoscope | Optional stable Durga portrait |
| 3 | DJ takeover, normally hidden | Very faint base/edge accent if needed |
| 4 | Crowd feed after testing | Usually off |
| 5 | Clean ad / overview override | Clean ad / overview override |

Use normal alpha for transparent DJ titles. Use Screen/Add deliberately for bright-on-black accents. Clear decorative layers for ads and announcements. Keep portraits' faces and hands stable. The downloaded footage provides base motion; shaders and Resolume transforms provide variation and musical timing.

## Existing assets and remaining decisions

[MATERIAL-NOTES.md](MATERIAL-NOTES.md) describes the copied footage and playlist sorting. [Ma/index.html](Ma/index.html) retains image choices and review notes. Old 03 is rejected; 09/12 have defects; 14 is the preferred old reference; 16 needs eye correction. Prior-year 1152 x 544 images should be fitted proportionally, not stretched to the wall.

Still to test: native Wire compilation, alpha behavior, tempo/retrigger controls, HDMI adapter and processor mapping, brightness, and sustained performance at the intended layer count. [SHOW-PLAN.md](SHOW-PLAN.md) also retains the later X4/OBS camera experiment.

Sanch manages screen/event coordination and supplied the playlist. Sam supplies the panels. Venu is the photographer. No one has been contacted on the user's behalf. The next Mac chat should begin with one running Source and one title Effect, not another AI generation batch.
