# KKManager
Mod, plugin and card manager for games by Illusion that use BepInEx. It can:
- Browse installed zipmods and plugins, and view information about them.
- Automatically find and install mod updates from the internet.
- Browse cards and scenes (supports drag and drop so you can drag cards into the game to load them).
- Install plugins and mods for you.
- Fix some common issues with the game.

You can see a list of supported games [here](https://github.com/IllusionMods/KKManager/blob/master/src/KKManager.Core/Functions/GameType.cs).

While the app is already useful, it's still in beta. Expect some bugs and lacking features. You can report issues on [GitHub Issues page](https://github.com/bbepis/KKManager/issues), or on the [Koikatu! discord server](https://discord.gg/urDt8CK).

## How to use
If you want to update your existing KKManager you can watch [this video guide](https://www.youtube.com/watch?v=ceg2XXGNwcU&feature=youtu.be).

1. Download the latest release from the [releases](https://github.com/IllusionMods/KKManager/releases) page.
2. Extract to a new folder anywhere. If you want to update your current KK Manager install then it's recommended to remove all old files except for the .settings file.
3. Start KKManager.exe, create a shortcut to it if you want.
4. If the game directory was not automatically detected, select where you installed KK.

You can turn on automatic checking for updates in the Tools menu. If there are any mod updates available, the update menu button on top will turn green after a while. You can observe update process through the log window.

## Screenshots
![kkmanager cards preview](https://user-images.githubusercontent.com/39247311/70395199-ae99f600-19fc-11ea-99b2-ee31a9081468.PNG)
![kkmanager plugins preview](https://user-images.githubusercontent.com/39247311/70395200-af328c80-19fc-11ea-90b8-b2baed0b3521.PNG)
![kkmanager updater preview](https://user-images.githubusercontent.com/39247311/70381094-dded2c00-1944-11ea-9502-db5ced9dd3e0.PNG)

You can support development of this tool through the [patreon page](https://www.patreon.com/ManlyMarco).

# Contributing
If you would like to make changes to KKManager or build the latest version with unreleased features, you can build this project with Visual Studio Community (2022 recommended). Simply clone or download code zip, open the .sln, and hit `Build > Build Solution`.
You can submit code fixes and improvements with a PR, they are being actively merged.

If you would like to translate KKManager to your language, you can use https://github.com/HakanL/resxtranslator. First clone this repository, then open it in the translator app, select your language and go ham. You can build and run KKManager to test your changes.
You can submit new translations with a PR, or if you don't have a GitHub account you can zip it up and send it to ManlyMarco on Koikatsu or IllusionSoft Discord server.

If you've found a bug and can't fix it yourself, feel free to post it in Issues. Make sure to include as many details as possible - how to reproduce the issue, exactly what the effects are, and what should happen instead. Please make sure that there is no open issue for your bug before posting.
Please don't post issues related to the Sideloader Modpacks and the update servers being slow (but do post issues related to the update process failing).
