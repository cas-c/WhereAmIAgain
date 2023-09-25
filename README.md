# Where am I again?
[![Download count](https://img.shields.io/endpoint?url=https://qzysathwfhebdai6xgauhz4q7m0mzmrf.lambda-url.us-east-1.on.aws/WhereAmIAgain)](https://github.com/cas-c/WhereAmIAgain)

A Dalamud plugin to display your current location on screen at all times in plain text.

## What's it do?

As you move around in-game, your current location will be displayed in the top right corner of your screen, next to your server information / clocks.

### Features

- Display as much / as little location information as you'd like, separated in whatever way you'd like, with support for preceding and trailing characters.
  - Default Format: `{0}, {1}, {2}, {3}`)
  - Default Display: `Old Sharlayan Aetheryte Plaza, Archon's Design, Old Sharlayan, The Northern Empty`
  - Example Custom Format: `{1} | {2}`
  - Example Custom Display: `Archon's Design | Old Sharlayan`
  - Example Custom Format 2: `<<{0}>> |`
  - Example Custom Display 2: `<<Old Sharlayan Aetheryte Plaza>> |`
    - Thank you to [@mustafakalash](https://github.com/mustafakalash) for the update to allow additional decorative text outside of the actual location string.
- Display the current instance number you are in if you are in a zone with multiple instances.  
  - Default: On
  - Note to any other plugin devs: Don't use Toasts to find your current location.  The current known signature for this is included in this repository, feel free to make use of it.  Thank you [@NadyaNayme](https://github.com/NadyaNayme) and [@MidoriKami](https://github.com/MidoriKami).

## How do I use it?

You can use the chat command `/waia` to open up the configuration screen.

From there, you can turn off the instance number display, as well as change the separators between each location and the order to your liking by simply adjusting the format string in the text box.

By default, the location is printed in order of most specificity to least, followed by the instance number.
