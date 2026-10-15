# Mac handoff

The meeting changed the priority: go further on DJ night and Dandiya with energetic motion and title takeovers. Daytime may use one approved event overview still. Continue shader coding and Wire setup on the M1 Max / 64 GB Mac. The Windows GPU is occupied; do not use it until the user releases it.

## Start here

1. Clone or update the Git repository on the Mac. The earlier USB Project snapshot predates this direction change. Use the USB for prior-year media and ads; the retired meeting presentation is not needed.
2. Read [the Wire + GLSL tutorial](shaders/WIRE-TUTORIAL.md). Build RadialRave as a Source and the PrismaticTitle + TitleTakeover chain as an Effect. The two tutorial examples have not yet been rendered or compiled in Wire.
3. Create a 1248 x 540 Resolume composition. Confirm the installed edition/version and the processor's input signal with Sam before configuring output.
4. Copy selected playlist footage, prior-year media and ads to the internal SSD. Keep these, the composition and consolidated patches in `show-media/` or another folder outside Git. The existing transparent title PNGs are in `outputs/dj-titles/`.
5. Make one title clip each for DJ Edwin, DJ Mumbai Rani and DJ Dinu. Set readable size individually. Verify clip-trigger restart, readable hold, zoom-through and fade-out.
6. Rehearse three to five layers, save, and reopen with the USB disconnected. Camera work comes after playback works.

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

[MATERIAL-NOTES.md](MATERIAL-NOTES.md) points to reusable USB footage. [Ma/index.html](Ma/index.html) retains image choices and review notes. Old 03 is rejected; 09/12 have defects; 14 is the preferred old reference; 16 needs eye correction. Prior-year 1152 x 544 images should be fitted proportionally, not stretched to the wall.

Still to test: native Wire compilation, alpha behavior, tempo/retrigger controls, HDMI adapter and processor mapping, brightness, and sustained performance at the intended layer count. [SHOW-PLAN.md](SHOW-PLAN.md) also retains the later X4/OBS camera experiment.

Sanch manages screen/event coordination and supplied the playlist. Sam supplies the panels. Venu is the photographer. No one has been contacted on the user's behalf. The next Mac chat should begin with one running Source and one title Effect, not another AI generation batch.
