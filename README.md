# TerrariaAutoBackupper

## For what
This program is designed to automatically backup data for the game "Terraria", such as files of game characters and worlds.

## Configuration
Example configuration file
```json
{
    "GameDataDirectory": "C:\\Users\\UserName\\Documents\\My Games\\Terraria",
    "BackupTargetDirectory": "K:\\Folder\\Other\\TerrariaBackup",
    "Players": [
        "Player1"
    ],
    "Worlds": [
        "World1"
    ],
    "LaunchSystemStartup": false
}
```

## Version capabilities

### v1
There is currently no interactive interaction with the program. You must manually make changes to the configuration file.
The program is also designed for Windows OS.