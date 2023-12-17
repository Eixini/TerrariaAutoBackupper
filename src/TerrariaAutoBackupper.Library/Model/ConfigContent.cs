namespace TerrariaAutoBackupper.Library.Model;

public class ConfigContent
{
    // Директории, где находятся данные игры - персонажи, миры и т.д
    public string? GameDataDirectory { get; set; }

    // Целевые директории, куда нужно бекапить данные
    public string? BackupTargetDirectory { get; set; }

    // Список персонажей, файлы которых нужно бекапить
    public List<string>? BackupPlayersTargets { get; set; }
    public List<string>? BackupWorldsTargets { get; set; }

    // Конфигурация самого приложения
    public bool LaunchSystemStartup { get; set; }

    // Список доступных языков
    public List<string>? AvailableLanguages { get; set; }
    public string? SelectedLanguage { get; set; }
}
