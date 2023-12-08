using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaAutoBackupper;

internal class ConfigContent
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
}
