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

        while (true)
        {
            //playersBackupper.PlayersAutoBackup();
            process = MainMenu();

            if (process == -1)
                break;
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
        Console.WriteLine("4. Help");
        Console.WriteLine("5. Quit");

        bool selectIsCorrect = int.TryParse(Console.ReadLine(), out select);

        if (select >= 1 && select <= 5)
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
                    Help();
                    return 0;
                case 5:
                    return -1;
            }
        }
        else
        {
            MainMenu();
            return 0;
        }

        return 1;
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
    public void Help()
    {
        Console.WriteLine("Help menu.");
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
