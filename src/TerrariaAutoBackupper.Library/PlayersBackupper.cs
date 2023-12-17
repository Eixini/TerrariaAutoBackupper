namespace TerrariaAutoBackupper.Library;

public class PlayersBackupper
{
    BackupContext backupContext;

    public PlayersBackupper()
    {
        backupContext = new BackupContext();
        backupContext.Initialization();
    }

    public void PlayersAutoBackup()
    {
        // Tracking changes to game character files
        using var playerFilesWatcher = new FileSystemWatcher(backupContext.ConfigData.GameDataDirectory +
                                                             Path.DirectorySeparatorChar +
                                                             "Players");

        playerFilesWatcher.NotifyFilter = NotifyFilters.LastWrite;

        playerFilesWatcher.Changed += PlayerDataOnChanged;

        playerFilesWatcher.Filter = "*.plr";
        playerFilesWatcher.EnableRaisingEvents = true;

        Console.ReadLine();
    }

    /// <summary>
    /// Handling changes to character files.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void PlayerDataOnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        Console.WriteLine("Changed");

        if (backupContext.SourceDataDirectoryExist())
        {
            if (PresencePlayersInConfiguration())
                PlayersBackupProcess();
        }

    }

    /// <summary>
    /// Checks if a specific player's destination directory exists.
    /// If it does not exist, it will be created.
    /// </summary>
    /// <param name="playerName">Game character name.</param>
    public void CurrentPlayerDirectory(string playerName)
    {
        var playerDirectory = backupContext.ConfigData.BackupTargetDirectory +
                      Path.DirectorySeparatorChar +
                      "Players" +
                      Path.DirectorySeparatorChar +
                      playerName;

        if (!Directory.Exists(playerDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"The \"{playerName}\" player destination directory does not exist.");
            Console.WriteLine("It will be created.");
            Console.ResetColor();

            Directory.CreateDirectory(playerDirectory);
        }

    }

    /// <summary>
    /// Checking the existence of the original game character data file, in ".plr" format.
    /// </summary>
    /// <param name="playerName">Game character name, without file extension.</param>
    /// <returns>true - the game character file exists, false - otherwise.</returns>
    public bool PlayerIsExists(string playerName)
    {
        var pathToPlayer = backupContext.ConfigData.GameDataDirectory +
                           Path.DirectorySeparatorChar +
                           "Players" +
                           Path.DirectorySeparatorChar +
                           playerName + ".plr";

        if (File.Exists(pathToPlayer))
            return true;
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No character game \"{playerName}\" files found.");
            Console.ResetColor();

            return false;
        }
    }

    /// <summary>
    /// Checks if there are game characters in the configuration list.
    /// </summary>
    /// <returns>true - if present, otherwise - false</returns>
    public bool PresencePlayersInConfiguration()
    {
        if (backupContext.ConfigData.BackupPlayersTargets.Count() == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There are no game characters in the configuration to backup their data files.");
            Console.ResetColor();

            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Checks if the destination directory with players exists.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void PlayersDestinationDataDirectory()
    {
        var playersBackupTargetDirectory = backupContext.ConfigData.BackupTargetDirectory +
                                           Path.DirectorySeparatorChar +
                                           "Players";

        if (!Directory.Exists(playersBackupTargetDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Destination directory containing player data was not found.");
            Console.WriteLine("A new one will be created.");
            Console.ResetColor();

            Directory.CreateDirectory(playersBackupTargetDirectory);
        }
    }

    /// <summary>
    /// Preparation before the backup process.
    /// The necessary directories are checked and created.
    /// </summary>
    /// <param name="playerName">Game character name.</param>
    public void PreparingPlayersBackupProcess(string playerName)
    {
        backupContext.GeneralDestinationDataDirectory();
        PlayersDestinationDataDirectory();
        CurrentPlayerDirectory(playerName);
    }

    /// <summary>
    /// The process of backing up game character files.
    /// </summary>
    public void PlayersBackupProcess()
    {
        foreach (string player in backupContext.ConfigData.BackupPlayersTargets)
        {
            if (PlayerIsExists(player))
            {
                PreparingPlayersBackupProcess(player);

                // Create a directory PlayerName1_DD_MM_YYYY_HH_MM_SS.
                var currentDateTime = DateTime.Now;
                var currentPlayerBackupFileName = player + "_" +
                                                  currentDateTime.Day.ToString() + "-" +
                                                  currentDateTime.Month.ToString() + "-" +
                                                  currentDateTime.Year.ToString() + "_" +
                                                  currentDateTime.Hour.ToString() + "-" +
                                                  currentDateTime.Minute.ToString() + "-" +
                                                  currentDateTime.Second.ToString();

                var actualDataPlayerDirectory = backupContext.ConfigData.BackupTargetDirectory +
                                                Path.DirectorySeparatorChar +
                                                "Players" +
                                                Path.DirectorySeparatorChar +
                                                player +
                                                Path.DirectorySeparatorChar +
                                                currentPlayerBackupFileName;

                Directory.CreateDirectory(actualDataPlayerDirectory);

                CopyPlayerDirectory(backupContext.ConfigData.GameDataDirectory +
                                    Path.DirectorySeparatorChar +
                                    "Players", actualDataPlayerDirectory, player);
            }

        }
    }

    /// <summary>
    /// Copies a directory and its contents.
    /// </summary>
    /// <param name="sourceDirectory">Directory to copy.</param>
    /// <param name="destinationDirectory">Directory to copy to.</param>
    /// <param name="player">Player name.</param>
    public void CopyPlayerDirectory(string sourceDirectory, string destinationDirectory, string player)
    {
        var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);

        // Copy files from source to destination
        string[] playerFiles = Directory.GetFiles(sourceDirectory, $"{player}.plr*");
        foreach (string file in playerFiles)
        {
            if (!File.Exists(Path.Combine(destinationDirectory, Path.GetFileName(file))))
                File.Copy(Path.Combine(sourceDirectory, Path.GetFileName(file)),
                          Path.Combine(destinationDirectory, Path.GetFileName(file)));

            Console.WriteLine("copy: " + Path.Combine(destinationDirectory, Path.GetFileName(file)));
        }

        // The method returns a list of directories, but in the current case only one is needed.
        DirectoryInfo subDirectory = sourceDirectoryInfo.GetDirectories(player, SearchOption.TopDirectoryOnly)
                                                        .FirstOrDefault();

        var playerSubDirectory = destinationDirectory +
                                 Path.DirectorySeparatorChar +
                                 player;

        Directory.CreateDirectory(playerSubDirectory);

        // Copy files from a subdirectory to the destination subdirectory
        foreach (FileInfo file in subDirectory.GetFiles())
        {
            if (!File.Exists(file.ToString()))
            {
                Console.WriteLine("copy: " + Path.Combine(destinationDirectory, file.Name));
                file.CopyTo(Path.Combine(playerSubDirectory, file.Name));
            }
        }

    }

}
