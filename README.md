# Where am I again?

A Dalamud plugin to display your current location on screen at all times in plain text.

## What's it do?

As you move around in-game, your current location will be displayed in the top right corner of your screen, next to your server information / clocks.

### Features

- Display as much / as little location information as you'd like, separated in whatever way you'd like, with support for preceding and trailing characters.
  - Default Format: `{0}, {1}, {2}, {3}`)
  - Default Display: `Old Sharlayan Aetheryte Plaza, Archon's Design, Old Sharlayan, The Northern Empty`
  - Example Custom Format: `{1} | {2}`
  - Example Custom Display: `Archon's Design | Old Sharlayan`
- Display the current instance number you are in if you are in a zone with multiple instances.  
  - Default: On
  - Note to any other plugin devs: Don't use Toasts to find your current location.  The current known signature for this is included in this repository, feel free to make use of it.  Thank you @NadyaNayme and @MidoriKami.

## How do I use it?

You can use the chat command `/waia` to open up the configuration screen.

From there, you can turn off the instance number display, as well as change the separators between each location and the order to your liking by simply adjusting the format string in the text box.

By default, the location is printed in order of most specificity to least, followed by the instance number.

## How was this made?

Originally, this plugin was created simply to display the current area info.  ("Highbridge, Wellwick Wood" while on the Highbridge in Eastern Thanalan.)  This wasn't available in existing plugins that @cassandra308 could find, so it was filled in by parsing the text that appears on your screen when you move from one area to another.

Then, the found location name was inserted manually into the area next to the server information in the top right corner of the screen.  When it was showed to some friends after this initial version, custom text display and various other features were requested. In order to do all this initially most of the plugin was quickly frankensteined together from some various existing plugins:

- A clumsily cloned + modified version of [one of the sample plugins](https://github.com/goatcorp/SamplePlugin)
- @Imcintyre's fork of the [Orchestrion Plugin](https://github.com/lmcintyre/OrchestrionPlugin) for inserting directly into the server info bar, which made this plugin incompatible with Orchestrion :(
- A rather crude imitation of some of @lokinmodar's plugin [Echoglossian](https://github.com/lokinmodar/Echoglossian)'s Toast handling

This version had several limitations and improvements that were needed, which kept the plugin in test mode for a year or so:

- As already mentioned, clashed with Orchestrion, a very loved plugin
  - This was resolved when Dalamud introduced the DTR bar to accomadate the new plugins being created which all wished to inject text into the same place (the server information area).
- Very brittle handling of edge cases due to use of regex on toast messages.
  - For example, the dungeon Amaurot's names were all tested against hardcoded strings due to their oddness compared to the rest of the location names, which made it not work in any language except English
  - Non-English language punctuation was missed occasionally
  - Other plugins that create Toast messages, if not using the same text format as FFXIV's built in location strings, could randomly appear in the server info bar.
  - This was resolved by @MidoriKami's rewrite to bring the plugin more up to snuff with Dalamud standards, which involves grabbing the location name directly, which means the plugin also works now in a region-agnostic way.  The only real downside is the possibility of updates/maintenance being needed when XIV itself updates, but the tradeoff is very worth it.
- Did not display instance numbers, which was a much asked for feature in various communities (S/SS Rank spawners/hunters as well as Gatherers mostly)
  - This was also added by @MidoriKami during the rewrite! :)
- Very inefficient.
  - Previously, the parsing code would check if the location had changed both on every frame update as well as toast updates.  This was because when you teleport into a new location or start the plugin, the toast isn't popped up -- which meant to display when you are immediately when teleport somewhere, we were checking on every update.  This wasn't a game breaking deal but did get quite inefficient to the point that some other plugin devs noticed, and @MidoriKami rewrote in order to resolve this as well.  Thanks should go to @Jessidhia, @NadyaNayme and of course @MidoriKami for this.

All of these existing issues culminated in needing a fairly extensive rewrite from the ground up, which was done by @MidoriKami, and this current rewritten version is what exists in the Dalamud plugins today.

### Afterword

A very big thank you to @reiichi001 and @MidoriKami for helping to maintain and keep this alive even after it was dropped into Dalamud testing by a C#/RE noob and then neglected for a bit, as well as all the other plugin devs who helped/sig hunted/heckled this plugin into the much better state it now is in today.  Big learning project and @cassandra308 is very, very grateful.  Also, thank you to the various people who tested and gave lots of feedback and requested useful features!

Sorry for writing a novel in the README.
