namespace TerrariaAutoBackupper.Library;

public class WorldsBackupper
{
    BackupContext backupContext;

    public WorldsBackupper()
    {
        backupContext = new BackupContext();
        backupContext.Initialization();

        if (backupContext.SourceDataDirectoryExist())
        {
            if (PresenceWorldInConfiguration())
                WorldsBackupProcess();
        }
    }

    /// <summary>
    /// Checks if a specific world's destination directory exists.
    /// If it does not exist, it will be created.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void CurrentWorldDirectory(string worldName)
    {
        var worldDirectory = backupContext.ConfigData.BackupTargetDirectory +
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
    /// Checking the existence of the original world data file, in ".wld" format.
    /// </summary>
    /// <param name="worldName">World name, without file extension.</param>
    /// <returns>true - the world file exists, false - otherwise.</returns>
    public bool WorldIsExists(string worldName)
    {
        var pathToWorld = backupContext.ConfigData.GameDataDirectory +
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
    /// Checks if there are worlds in the configuration list.
    /// </summary>
    /// <returns>true - if present, otherwise - false</returns>
    public bool PresenceWorldInConfiguration()
    {
        if (backupContext.ConfigData.BackupWorldsTargets.Count() == 0)
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
    /// Checks if the destination directory with worlds exists.
    /// If the directory does not exist, it will be created.
    /// </summary>
    public void WorldDestinationDataDirectory()
    {
        var worldsBackupTargetDirectory = backupContext.ConfigData.BackupTargetDirectory +
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
    /// Preparation before the backup process.
    /// The necessary directories are checked and created.
    /// </summary>
    /// <param name="worldName">World name.</param>
    public void PreparingWorldsBackupProcess(string worldName)
    {
        backupContext.GeneralDestinationDataDirectory();
        WorldDestinationDataDirectory();
        CurrentWorldDirectory(worldName);
    }

    /// <summary>
    /// The process of backing up world files.
    /// </summary>
    public void WorldsBackupProcess()
    {
        foreach (string world in backupContext.ConfigData.BackupWorldsTargets)
        {

            if (WorldIsExists(world))
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

                var actualDataWorldDirectory = backupContext.ConfigData.BackupTargetDirectory +
                                Path.DirectorySeparatorChar +
                                "Worlds" +
                                Path.DirectorySeparatorChar +
                                world +
                                Path.DirectorySeparatorChar +
                                currentWorldBackupFileName;

                Directory.CreateDirectory(actualDataWorldDirectory);

                CopyWorldDirectory(backupContext.ConfigData.GameDataDirectory +
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
    /// <param name="world">World name.</param>
    public void CopyWorldDirectory(string sourceDirectory, string destinationDirectory, string world)
    {
        var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);

        // Copy files from source to destination
        string[] worldFiles = Directory.GetFiles(sourceDirectory, $"{world}.wld*");
        foreach (string file in worldFiles)
        {
            if (File.Exists(Path.Combine(destinationDirectory, Path.GetFileName(file))))
                File.Copy(Path.Combine(sourceDirectory, Path.GetFileName(file)),
                          Path.Combine(destinationDirectory, Path.GetFileName(file)));

            Console.WriteLine("copy: " + Path.Combine(destinationDirectory, Path.GetFileName(file)));
        }

    }

}
