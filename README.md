# TerrariaAutoBackupper

## For what
This program is designed to automatically backup data for the game "Terraria", such as files of game characters and worlds.

## Configuration
Example configuration file
```json
{
  "GameDataDirectory": "C:\\Users\\UserName\\Documents\\My Games\\Terraria",
  "BackupTargetDirectory": "K:\\Folder\\Other\\TerrariaBackup",
  "BackupPlayersTargets": [
    "TestPlayer"
  ],
  "BackupWorldsTargets": [
    "TestWorld"
  ],
  "LaunchSystemStartup": false,
  "AvailableLanguages": [
    "en",
    "ru"
  ],
  "SelectedLanguage": "en"
}
```

## Version capabilities

### v1
There is currently no interactive interaction with the program. You must manually make changes to the configuration file.
The program is also designed for Windows OS.
### v2
Implemented CLI.
   At the moment, automatic backup is available, but not active.
Backup must be done manually. To do this, in the main menu there are items for manual backup of players and manual backup of worlds. After selecting the appropriate item, the program will check for the presence of player/world files, check the paths and copy the files.

