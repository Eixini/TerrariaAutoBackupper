using System.Numerics;
using System.Text.Json;

namespace TerrariaAutoBackupper;

internal class Backupper
{
    private ConfigContent _configData = new ConfigContent();

    public Backupper()
    {
        ApplicationDirectory();
        ApplicationConfiguration();
        ReadDataFromConfigFile();

        if (SourceDataDirectoryExist())
        {
            if (PresencePlayersInConfiguration())
                PlayersBackupProcess();
            if (PresenceWorldInConfiguration())
                WorldsBackupProcess();
        }
    }

    /// <summary>
    /// Method for checking the existence of the application directory.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void ApplicationDirectory()
    {
        // C:\Users\<UserName>\Documents
        //Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

        string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                         Path.DirectorySeparatorChar +
                         "TerrariaAutoBackupper";

        if (!Directory.Exists(AppPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Application Directory does not exist.");

            Directory.CreateDirectory(AppPath);

            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Application Directory exist.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Method for checking for the existence of a configuration file.
    /// </summary>
    public void ApplicationConfiguration()
    {
        string ConfigFileName = "tab_config.json";
        string ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                 Path.DirectorySeparatorChar +
                 "TerrariaAutoBackupper" +
                 Path.DirectorySeparatorChar +
                 ConfigFileName;

        if (!File.Exists(ConfigFilePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Application configuration file does not exist.");
            Console.ResetColor();

            using (StreamWriter streamWriter = File.CreateText(ConfigFilePath))
            {
                streamWriter.WriteLine(DefaultConfigContents());
            }

            //File.WriteAllText(ConfigFilePath, DefaultConfigContents());
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Application configuration file exist.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Method for filling the application configuration file.
    /// This method is necessary for the case when the application did not find the configuration file,
    /// and you need to create a new file and fill it with default content.
    /// </summary>
    /// <returns> Serialized object </returns>
    public string DefaultConfigContents()
    {
        var defaultConfigContent = new ConfigContent
        {
            GameDataDirectory = "Path to game data",
            BackupTargetDirectory = "Path to target backup directory",
            BackupPlayersTargets = new List<string>(),
            BackupWorldsTargets = new List<string>(),
            LaunchSystemStartup = false
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(defaultConfigContent, options);

        return jsonString;
    }

    /// <summary>
    /// Method for reading data from a configuration file.
    /// </summary>
    public void ReadDataFromConfigFile()
    {
        string ConfigFileName = "tab_config.json";
        string ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                 Path.DirectorySeparatorChar +
                 "TerrariaAutoBackupper" +
                 Path.DirectorySeparatorChar +
                 ConfigFileName;

        using (StreamReader streamReader = new StreamReader(ConfigFilePath))
        {
            var configFileContent = streamReader.ReadToEnd();
            _configData = JsonSerializer.Deserialize<ConfigContent>(configFileContent)!;
        }

        // For Windows
        _configData.GameDataDirectory.Replace(@"\\", @"\");

        _configData.BackupTargetDirectory.Replace(@"\\", @"\");

        // DEBUG
        //Console.WriteLine();
        //Console.WriteLine(_configData.GameDataDirectory);
        //Console.WriteLine(_configData.BackupTargetDirectory);
        //Console.WriteLine(_configData?.BackupPlayersTargets.FirstOrDefault());
        //Console.WriteLine(_configData?.BackupWorldsTargets.FirstOrDefault());
        //Console.WriteLine(_configData.LaunchSystemStartup);
    }

    /// <summary>
    /// Checking for the existence of a shared directory with data sources.
    /// </summary>
    /// <returns>true - the source directory exists, false - otherwise</returns>
    public bool SourceDataDirectoryExist()
    {
        if (Directory.Exists(_configData.GameDataDirectory))
            return true;
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The shared directory with the source data does not exist or the path is incorrect.");
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
        if(_configData.BackupPlayersTargets.Count() == 0)
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("There are no game characters in the configuration to backup their data files.");
            Console.ResetColor();

            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Checks if there are worlds in the configuration list.
    /// </summary>
    /// <returns>true - if present, otherwise - false</returns>
    public bool PresenceWorldInConfiguration()
    {
        if(_configData.BackupWorldsTargets.Count() == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There are no world in the configuration to backup their data files.");
            Console.ResetColor();

            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Checking the existence of the original game character data file, in ".plr" format.
    /// </summary>
    /// <param name="playerName">Game character name, without file extension.</param>
    /// <returns>true - the game character file exists, false - otherwise.</returns>
    public bool PlayerIsExists(string playerName)
    {
        var pathToPlayer = _configData.GameDataDirectory +
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
    /// Checking the existence of the original world data file, in ".wld" format.
    /// </summary>
    /// <param name="worldName">World name, without file extension.</param>
    /// <returns>true - the world file exists, false - otherwise.</returns>
    public bool WorldIsExists(string worldName)
    {
        var pathToWorld = _configData.GameDataDirectory +
                           Path.DirectorySeparatorChar +
                           "Worlds" +
                           Path.DirectorySeparatorChar +
                           worldName + ".wld";

        if (File.Exists(pathToWorld))
            return true;
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No world \"{worldName}\" files found.");
            Console.ResetColor();

            return false;
        }
    }

    /// <summary>
    /// Checks if the shared destination directory exists.
    /// If there is none, it will be created.
    /// </summary>
    public void GeneralDestinationDataDirectory()
    {
        if(!Directory.Exists(_configData.BackupTargetDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The shared destination directory was not found.");
            Console.WriteLine("The directory will be created.");
            Console.ResetColor();

            Directory.CreateDirectory( _configData.BackupTargetDirectory);
        }
    }

    /// <summary>
    /// Checks if the destination directory with players exists.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void PlayersDestinationDataDirectory()
    {
        var playersBackupTargetDirectory = _configData.BackupTargetDirectory +
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
    /// Checks if the destination directory with worlds exists.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void WorldDestinationDataDirectory()
    {
        var worldsBackupTargetDirectory = _configData.BackupTargetDirectory +
                                          Path.DirectorySeparatorChar +
                                          "Worlds";

        if (!Directory.Exists(worldsBackupTargetDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Destination directory containing world data was not found.");
            Console.WriteLine("A new one will be created.");
            Console.ResetColor();

            Directory.CreateDirectory(worldsBackupTargetDirectory);
        }
    }

    /// <summary>
    /// Checks if a specific player's destination directory exists.
    /// If it does not exist, it will be created.
    /// </summary>
    /// <param name="playerName">Game character name.</param>
    public void CurrentPlayerDirectory(string playerName)
    {
        var playerDirectory = _configData.BackupTargetDirectory +
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
    /// Checks if a specific world's destination directory exists.
    /// If it does not exist, it will be created.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void CurrentWorldDirectory(string worldName)
    {
        var worldDirectory = _configData.BackupTargetDirectory +
                             Path.DirectorySeparatorChar +
                             "Worlds" +
                             Path.DirectorySeparatorChar +
                             worldName;

        if (!Directory.Exists(worldDirectory))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"The \"{worldName}\" world destination directory does not exist.");
            Console.WriteLine("It will be created.");
            Console.ResetColor();

            Directory.CreateDirectory(worldDirectory);
        }

    }

    /// <summary>
    /// Preparation before the backup process.
    /// The necessary directories are checked and created.
    /// </summary>
    /// <param name="playerName">Game character name.</param>
    public void PreparingPlayersBackupProcess(string playerName)
    {
        GeneralDestinationDataDirectory();
        PlayersDestinationDataDirectory();
        CurrentPlayerDirectory(playerName);
    }

    /// <summary>
    /// Preparation before the backup process.
    /// The necessary directories are checked and created.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void PreparingWorldsBackupProcess(string worldName)
    {
        GeneralDestinationDataDirectory();
        WorldDestinationDataDirectory();
        CurrentWorldDirectory(worldName);
    }

    /// <summary>
    /// The process of backing up game character files.
    /// </summary>
    public void PlayersBackupProcess()
    {
        foreach (string player in _configData.BackupPlayersTargets)
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

                var actualDataPlayerDirectory = _configData.BackupTargetDirectory +
                                                Path.DirectorySeparatorChar +
                                                "Players" +
                                                Path.DirectorySeparatorChar +
                                                player +
                                                Path.DirectorySeparatorChar +
                                                currentPlayerBackupFileName;

                Directory.CreateDirectory(actualDataPlayerDirectory);

                CopyPlayerDirectory(_configData.GameDataDirectory +
                                    Path.DirectorySeparatorChar +
                                    "Players", actualDataPlayerDirectory, player);
            }

        }
    }

    /// <summary>
    /// The process of backing up world files.
    /// </summary>
    public void WorldsBackupProcess()
    {
        foreach(string world in _configData.BackupWorldsTargets) {
            
            if(WorldIsExists(world))
            {
                PreparingWorldsBackupProcess(world);

                // Create directory WorldName1_DD_MM_YYYY_HH_MM_SS.
                var currentDateTime = DateTime.Now;
                var currentWorldBackupFileName = world + "_" +
                                                  currentDateTime.Day.ToString() + "-" +
                                                  currentDateTime.Month.ToString() + "-" +
                                                  currentDateTime.Year.ToString() + "_" +
                                                  currentDateTime.Hour.ToString() + "-" +
                                                  currentDateTime.Minute.ToString() + "-" +
                                                  currentDateTime.Second.ToString();

                var actualDataWorldDirectory = _configData.BackupTargetDirectory +
                                Path.DirectorySeparatorChar +
                                "Worlds" +
                                Path.DirectorySeparatorChar +
                                world +
                                Path.DirectorySeparatorChar +
                                currentWorldBackupFileName;

                Directory.CreateDirectory(actualDataWorldDirectory);

                CopyWorldDirectory(_configData.GameDataDirectory +
                                   Path.DirectorySeparatorChar +
                                   "Worlds", actualDataWorldDirectory, world);
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
            Console.WriteLine("copy: " + Path.Combine(destinationDirectory, file.Name));
            file.CopyTo(Path.Combine(playerSubDirectory, file.Name));
        }

    }

    /// <summary>
    /// Copies a directory and its contents.
    /// </summary>
    /// <param name="sourceDirectory">Directory to copy.</param>
    /// <param name="destinationDirectory">Directory to copy to.</param>
    /// <param name="world">World name.</param>
    public void CopyWorldDirectory(string sourceDirectory, string destinationDirectory, string world)
    {
        var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);

        // Copy files from source to destination
        string[] worldFiles = Directory.GetFiles(sourceDirectory, $"{world}.wld*");
        foreach (string file in worldFiles)
        {
            File.Copy(Path.Combine(sourceDirectory, Path.GetFileName(file)),
                      Path.Combine(destinationDirectory, Path.GetFileName(file)));

            Console.WriteLine("copy: " + Path.Combine(destinationDirectory, Path.GetFileName(file)));
        }

    }

}