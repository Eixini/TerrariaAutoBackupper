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