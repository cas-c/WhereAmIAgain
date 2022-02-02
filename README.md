# Where am I again?

A Dalamud plugin to display your current location on screen at all times in plain text.

## What's it do?

As you move around in-game, will update text next to your server information to display the current location of your character.

`{Zone}, {Territory}, {Region}`

% On first load, this may only display `{Territory}, {Region}` until you enter a new zone.

## Todos

- Get current location from memory rather than by parsing and filtering toasts.
  - This mostly worked using `scanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 56");`, (ty midori) but in some cases is in another location.  Still needs some fiddling about and it's on a local branch rn since the dtr bar dalamud update came out and franz made that pr!
  - This value also doesn't update immediately on rezone, so also needs additional code for handling that.
- Make cute togglable icon, something like orchestrion's music note to also match the server location icon! waow
  