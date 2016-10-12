# DiscordLCD
Logitech LCD plugin for Discord

Currently supports the monochrome LCDs (can't test RGB)
- G510 / G510s - 160x43 Monochrome
- G13 - 160x43 Monochrome
- G15 v1 - 160x43 Monochrome
- G15 v2 - 160x43 Monochrome

Uses [Discord.Net 1.0 beta (concrete2 branch)](https://github.com/RogueException/Discord.Net) & DiscordPTB (Public Test Build)

To use:
* Create a new application in the [Discord Developer portal](https://discordapp.com/developers/applications/me)
* Input your settings in the `Settings.settings` config file

Currently only supports 1 hardcoded voice channel to monitor for the LCD. Waiting on RPC improvements.
