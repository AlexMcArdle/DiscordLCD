# DiscordLCD
## This project is on hold indefinitely due to Discord breaking RPC.
Logitech LCD plugin for Discord


![DiscordLCD](https://cloud.githubusercontent.com/assets/941476/19740219/79c13fce-9b83-11e6-90c8-e3940bb48abc.png "DiscordLCD")

Currently supports the monochrome LCDs (can't test RGB)
- G510 / G510s - 160x43 Monochrome
- G13 - 160x43 Monochrome
- G15 v1 - 160x43 Monochrome
- G15 v2 - 160x43 Monochrome

Uses [Discord.Net 1.0 beta (dev branch)](https://github.com/RogueException/Discord.Net/tree/dev) & DiscordPTB (Public Test Build)

To use:
* Create a new application in the [Discord Developer portal](https://discordapp.com/developers/applications/me)
* Input your settings in the `Settings.settings` config file

Currently only supports 1 hardcoded voice channel to monitor for the LCD. Waiting on RPC improvements.
