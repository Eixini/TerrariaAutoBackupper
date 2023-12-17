using System.Text.Json;
using TerrariaAutoBackupper.Library.Model;

namespace TerrariaAutoBackupper.Library;

public class BackupContext
{
    private ConfigContent _configData = new ConfigContent();

    public ConfigContent ConfigData { get { return _configData; } }


    /// <summary>
    /// Method for initializing data.
    /// </summary>
    public void Initialization()
    {
        ApplicationDirectory();
        ApplicationConfiguration();
        ReadDataFromConfigFile();
    }

    /// <summary>
    /// Method for checking the existence of the application directory.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void ApplicationDirectory()
    {
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
    }

    /// <summary>
    /// Checks if the shared destination directory exists.
    /// If there is none, it will be created.
    /// </summary>
    public void GeneralDestinationDataDirectory()
    {
        if (!Directory.Exists(_configData.BackupTargetDirectory))
        {
            Directory.CreateDirectory(_configData.BackupTargetDirectory);
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
            LaunchSystemStartup = false,
            AvailableLanguages = new List<string>(),
            SelectedLanguage = "en"
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(defaultConfigContent, options);

        return jsonString;
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
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Application configuration file exist.");
            Console.ResetColor();
        }
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

    }

    /// <summary>
    /// Method for writing settings to a configuration file.
    /// </summary>
    public void WriteDataToConfigFile()
    {
        // It is necessary in order not to change some aspects, for example, screening paths.
        var teamConfigContent = new ConfigContent();
        teamConfigContent = _configData;

        string ConfigFileName = "tab_config.json";
        string ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                 Path.DirectorySeparatorChar +
                 "TerrariaAutoBackupper" +
                 Path.DirectorySeparatorChar +
                 ConfigFileName;

        if (File.Exists(ConfigFilePath))
        {
            // For Windows
            teamConfigContent.GameDataDirectory.Replace(@"\", @"\\");
            teamConfigContent.BackupTargetDirectory.Replace(@"\", @"\\");

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(teamConfigContent, options);

            using (StreamWriter streamWriter = File.CreateText(ConfigFilePath))
            {
                streamWriter.WriteLine(jsonString);
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Application configuration file does not exist.");
            Console.ResetColor();
        }

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
    /// Method for changing the directory with game data (players, worlds).
    /// </summary>
    public void ChangeGameDataDirectory(string newGameDataDirectory)
    {
        _configData.GameDataDirectory = newGameDataDirectory;

        WriteDataToConfigFile();
    }

    /// <summary>
    /// Method for changing destination directory.
    /// </summary>
    public void ChangeBackupTargetDirectory(string newBackupTargetDirectory)
    {
        _configData.BackupTargetDirectory = newBackupTargetDirectory;

        WriteDataToConfigFile();
    }

    /// <summary>
    /// Adding players to the list for subsequent backup of these players' files.
    /// </summary>
    /// <param name="playerName">Player name.</param>
    public void AddPlayer(string playerName)
    {
        _configData.BackupPlayersTargets.Add(playerName);

        WriteDataToConfigFile();
    }

    /// <summary>
    /// Removing player from the backup list.
    /// Player files will not be deleted.
    /// </summary>
    /// <param name="playerName">Player name.</param>
    public void RemovePlayer(string playerName)
    {
        if (_configData.BackupPlayersTargets.Remove(playerName))
            WriteDataToConfigFile();
        else
            return;
    }

    /// <summary>
    /// Adding worlds to the list for subsequent backup of files of these worlds.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void AddWorld(string worldName)
    {
        _configData.BackupWorldsTargets.Add(worldName);

        WriteDataToConfigFile();
    }

    /// <summary>
    /// Removing a world from the list for backup.
    /// World files will not be deleted.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void RemoveWorld(string worldName)
    {
        if (_configData.BackupWorldsTargets.Remove(worldName))
            WriteDataToConfigFile();
        else
            return;
    }

    /// <summary>
    /// Setting a value for the application to autostart at system startup.
    /// </summary>
    /// <param name="newValue">true - run at system startup, fasle - do not run at system startup.</param>
    public void ChangeLaunchSystemStartup(bool newValue)
    {
        if(_configData.LaunchSystemStartup != newValue)
        {
            _configData.LaunchSystemStartup = newValue;
            WriteDataToConfigFile();
        }
        else
            return;
    }

    /// <summary>
    /// Allows you to change the language.
    /// </summary>
    public void ChangeLanguage()
    {
        if(_configData.AvailableLanguages.Count != 0)
        {
            Console.WriteLine("Available languages:");
            foreach (var language in _configData.AvailableLanguages)
            {
                Console.WriteLine($"{_configData.AvailableLanguages.IndexOf(language) + 1}. {language}");
            }

            Console.WriteLine("Enter the language, for example \"en\":");

            string selectLanguage = Console.ReadLine();

            if (_configData.AvailableLanguages.Contains(selectLanguage))
            {
                _configData.SelectedLanguage = selectLanguage;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect value.");
                Console.ResetColor();

                return;
            }


        }
        else
        {
            Console.WriteLine("No languages to choose from.");
            return;
        }
    }

    /// <summary>
    /// Displays current settings.
    /// </summary>
    public void ShowConfiguration()
    {
        Console.WriteLine("Current configuration:\n");

        // SOURCE DIRECTORY
        Console.WriteLine($"Source data directory:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_configData.GameDataDirectory);
        Console.ResetColor();

        // DESTINATION DIRECTORY
        Console.WriteLine($"Destination directory:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_configData.BackupTargetDirectory);
        Console.ResetColor();

        // PLAYERS
        Console.WriteLine($"Selected players for backup:");
        Console.ForegroundColor = ConsoleColor.Green;
        if (_configData.BackupPlayersTargets.Count() > 0)
        {
            foreach (var player in _configData.BackupPlayersTargets)
                Console.Write($"\"{player}\" ");
        }
        else
        {
            Console.WriteLine("");
        }
        Console.ResetColor();
        Console.WriteLine();

        // WOLRDS
        Console.WriteLine($"Selected worlds for backup:");
        Console.ForegroundColor = ConsoleColor.Green;
        if (_configData.BackupWorldsTargets.Count() > 0)
        {
            foreach (var world in _configData.BackupWorldsTargets)
                Console.Write($"\"{world}\" ");
        }
        else
        {
            Console.WriteLine("");
        }
        Console.ResetColor();
        Console.WriteLine();

        // SYSTEM STARTUP
        Console.Write($"Launching the program at system startup: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(_configData.LaunchSystemStartup);
        Console.ResetColor();
        Console.WriteLine();
    }

}
