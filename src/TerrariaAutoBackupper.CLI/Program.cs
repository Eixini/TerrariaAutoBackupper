namespace TerrariaAutoBackupper.CLI;

internal class Program
{
    static void Main(string[] args)
    {
        ConsoleInterface consoleInterface = new ConsoleInterface();
        consoleInterface.ApplicationWork();
    }
}