using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using TerrariaAutoBackupper.Library;

namespace TerrariaAutoBackupper.CLI;

internal class ConsoleInterface
{
    ResourceManager resourceManager;
    CultureInfo cultureInfo;

    BackupContext backupContext;
    PlayersBackupper playersBackupper;
    WorldsBackupper worldsBackupper;

    public ConsoleInterface()
    {
        backupContext = new BackupContext();
        playersBackupper = new PlayersBackupper();
        worldsBackupper = new WorldsBackupper();

        backupContext.Initialization();

        //resourceManager = new ResourceManager("Resources", Assembly.GetExecutingAssembly());
        //cultureInfo = new CultureInfo("en-US"); 
    }

    /// <summary>
    /// The main method of the application.
    /// Performs work in a loop until the user decides to terminate the application.
    /// </summary>
    public void ApplicationWork()
    {
        WelcomeMessage();

        var process = 0;

        while (process != -1)
        {
            //playersBackupper.PlayersAutoBackup();
            process = MainMenu();
        }

    }

    /// <summary>
    /// Message shown when the application starts.
    /// </summary>
    public void WelcomeMessage()
    {
        Console.WriteLine("Hello. This is an application for automatically backing up game files such as game character files and worlds.");
        //Console.WriteLine(resourceManager.GetString("Welcome_message", cultureInfo));
        Console.WriteLine();
    }

    /// <summary>
    /// Application management.
    /// </summary>
    public int MainMenu()
    {
        Console.Clear();

        int select = 0;

        Console.WriteLine("Select item number: ");
        Console.WriteLine("1. Configuration menu");
        Console.WriteLine("2. Manual backup players");
        Console.WriteLine("3. Manual backup worlds");
        Console.WriteLine("4. Interactive backup world");
        Console.WriteLine("5. Help");
        Console.WriteLine("6. About");
        Console.WriteLine("7. Quit");

        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out select);

        if (select >= 1 && select <= 7)
        {
            switch (select)
            {
                case 1:
                    ConfigurationMenu();
                    return 0;
                case 2:
                    playersBackupper.PlayersBackupProcess();
                    return 0;
                case 3:
                    worldsBackupper.WorldsBackupProcess();
                    return 0;
                case 4:
                    InteractiveBackupWorld();
                    return 0;
                case 5:
                    HelpMenu();
                    return 0;
                case 6:
                    About();
                    return 0;
                case 7:
                    return -1;
            }
        }
        else
        {
            MainMenu();
        }

        return 1;
    }

    public void InteractiveBackupWorld()
    {
        Console.Clear();

        var worlds = backupContext.ConfigData.BackupWorldsTargets;

        if (worlds.Count() > 0)
        {
            // Show a list of available worlds
            int count = 1;

            Console.WriteLine("Select a world for backup: ");
            foreach (string world in worlds)
            {
                Console.WriteLine($"{count}. {world}");
                count++;
            }

            count = 1;

            // Selecting a world for interactive backup
            int select = 0;
            int.TryParse(Console.ReadLine(), out select);

            // Entering the changelog text
            Console.Clear();
            Console.WriteLine("Enter text changelog: ");

            var changelogText = Console.ReadLine();

            if (select >= worlds.Count() || select <= worlds.Count())
                worldsBackupper.InteractiveBackupWorld(worlds[select - 1], changelogText);
            else
                InteractiveBackupWorld();

        }
        else
        {
            Console.WriteLine("No worlds for backup.");
        }

        // Return to main menu
        Console.WriteLine("Enter something to return to the main menu ...");

        string value = Console.ReadLine();

        if (value != null)
            MainMenu();
        else
            MainMenu();

    }

    /// <summary>
    /// Configuration management.
    /// </summary>
    public void ConfigurationMenu()
    {
        Console.Clear();

        int select = 0;

        Console.WriteLine("Select item number: ");
        Console.WriteLine("1. Change game data directory");
        Console.WriteLine("2. Change target directory");
        Console.WriteLine("3. Add player");
        Console.WriteLine("4. Delete player");
        Console.WriteLine("5. Add world");
        Console.WriteLine("6. Delete world");
        Console.WriteLine("7. Change launch system startup");
        Console.WriteLine("8. Show current configuration");
        Console.WriteLine("9. Main menu");

        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out select);

        if (select >= 1 && select <= 9)
        {
            switch (select)
            {
                case 1:
                    ChangeSourceDirectory();
                    break;
                case 2:
                    ChangeDestinationDirectory();
                    break;
                case 3:
                    AddPlayer();
                    break;
                case 4:
                    DeletePlayer();
                    break;
                case 5:
                    AddWorld();
                    break;
                case 6:
                    DeleteWorld();
                    break;
                case 7:
                    SetAutorunValue();
                    break;
                case 8:
                    ShowConfiguration();
                    break;
                case 9:
                    MainMenu();
                    break;
            }
        }
        else
        {
            ConfigurationMenu();
        }

    }

    /// <summary>
    /// Method for displaying information and helping with navigation.
    /// </summary>
    public void HelpMenu()
    {
        Console.Clear();

        Console.WriteLine("Help menu:");
        Console.WriteLine();
        Console.WriteLine("1. Data entry example.");
        Console.WriteLine("2. Main Menu");

        int value = 0;
        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out value);

        if (value >= 1 && value <= 2)
        {
            switch (value)
            {
                case 1:
                    DataEntryExample();
                    break;
                case 2:
                    MainMenu();
                    break;
            }
        }
        else
        {
            HelpMenu();
        }
    }

    public void DataEntryExample()
    {
        Console.Clear();

        // SOURCE DIRECTORY
        Console.WriteLine($"Source data directory:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"C:\Users\UserDirectory\Documents\My Games\Terraria");
        Console.ResetColor();

        // DESTINATION DIRECTORY
        Console.WriteLine($"Destination directory:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"K:\SomeFolder\Other\TerrariaBackup");
        Console.ResetColor();

        // PLAYER
        Console.WriteLine($"Player name:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("PlayerName");
        Console.ResetColor();

        // WORLD
        Console.WriteLine($"Destination directory:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"MyCozyWorld");
        Console.ResetColor();

        // ================================================================
        Console.WriteLine("\n");
        Console.WriteLine("1. Back");
        Console.WriteLine("2. Main Menu");

        int value = 0;
        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out value);

        if (value == 1)
        {
            HelpMenu();
        }
        else if (value == 2)
        {
            MainMenu();
        }
        else
        {
            Console.WriteLine("Incorrect value.");
            HelpMenu();
        }
    }

    public void About()
    {
        Console.Clear();

        Console.WriteLine("TerrariaAutoBackupper");
        Console.WriteLine("A program for conveniently backing up game data such as player data and world data.");
        Console.WriteLine();
        Console.WriteLine("Developer - Eixini");

        Console.WriteLine();

        Console.WriteLine("1. Main Menu");

        int value = 0;
        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out value);

        if (value == 1)
        {
            MainMenu();
        }
        else
        {
            HelpMenu();
        }

    }

    /// <summary>
    /// Method for selecting the application language.
    /// </summary>
    public void LanguageSelection()
    {
        var selectLanguage = backupContext.ConfigData.SelectedLanguage;

        switch (selectLanguage)
        {
            case "en":
                cultureInfo = CultureInfo.CreateSpecificCulture("en");
                break;
            case "ru":
                cultureInfo = CultureInfo.CreateSpecificCulture("ru");
                break;
        }
    }


    public void ChangeSourceDirectory()
    {
        Console.Clear();
        Console.WriteLine("Enter the directory where the game data is located.");

        var newSourceDirectory = Console.ReadLine();

        backupContext.ChangeGameDataDirectory(newSourceDirectory);

        ConfigurationMenu();
    }


    public void ChangeDestinationDirectory()
    {
        Console.Clear();
        Console.WriteLine("Enter the destination directory.");

        var newDestinationDirectory = Console.ReadLine();

        backupContext.ChangeBackupTargetDirectory(newDestinationDirectory);

        ConfigurationMenu();
    }

    public void AddPlayer()
    {
        Console.Clear();
        Console.WriteLine("Enter player nickname.");

        var player = Console.ReadLine();

        backupContext.AddPlayer(player);

        ConfigurationMenu();
    }

    public void DeletePlayer()
    {
        Console.Clear();
        Console.WriteLine("Enter player nickname.");

        var player = Console.ReadLine();

        backupContext.RemovePlayer(player);

        ConfigurationMenu();
    }

    public void AddWorld()
    {
        Console.Clear();
        Console.WriteLine("Enter world name.");

        var world = Console.ReadLine();

        backupContext.AddWorld(world);

        ConfigurationMenu();
    }

    public void DeleteWorld()
    {
        Console.Clear();
        Console.WriteLine("Enter world name.");

        var world = Console.ReadLine();

        backupContext.RemoveWorld(world);

        ConfigurationMenu();
    }

    public void SetAutorunValue()
    {
        Console.Clear();
        Console.WriteLine("Select the autorun option:");
        Console.WriteLine("1. Run the program at system startup.");
        Console.WriteLine("2. Do not run the program at system startup.");

        int value = 0;
        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out value);

        if(value == 1)
        {
            backupContext.ChangeLaunchSystemStartup(true);
        }
        else if (value == 2)
        {
            backupContext.ChangeLaunchSystemStartup(false);
        }
        else
        {
            SetAutorunValue();
        }

        ConfigurationMenu();
    }

    public void ShowConfiguration()
    {
        Console.Clear();
        backupContext.ShowConfiguration();

        Console.WriteLine("\n");
        Console.WriteLine("1. Back");
        Console.WriteLine("2. Main Menu");

        int value = 0;
        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out value);

        if (value == 1)
        {
            ConfigurationMenu();
        }
        else if (value == 2)
        {
            MainMenu();
        }
        else
        {
            Console.WriteLine("Incorrect value.");
            ShowConfiguration();
        }
    }

}
