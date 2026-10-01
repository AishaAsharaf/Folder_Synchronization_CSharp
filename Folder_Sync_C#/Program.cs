
//Program to run the folder sync application

//Requesting the user to provide source folder path
Console.WriteLine("Provide the source folder path..");
var sourceFolderPath = Console.ReadLine();
while(!Directory.Exists(sourceFolderPath))
{
    Console.WriteLine("Source path mentioned does not exist, please enter relevant source path");
    sourceFolderPath = Console.ReadLine();
}

//Requesting the user to provide replica folder path
Console.WriteLine("Provide replica folder path..");
var replicaFolderPath = Console.ReadLine();

//Requesting the user to provide log file path
Console.WriteLine("Provide log file path..");
var logFilePath = Console.ReadLine();
while(!File.Exists(logFilePath))
{
    Console.WriteLine("Log file path mentioned does not exist, please enter relevant log file path");
    logFilePath = Console.ReadLine();
}

//Requesting the user to provide time interval to sync
Console.WriteLine("Provide time interval to sync");
int syncTimeInterval = int.Parse(Console.ReadLine());
while (syncTimeInterval <= 0 || syncTimeInterval.GetType() != typeof(int))
{
    Console.WriteLine("Time period is not an int and cannot be less than 0");
    syncTimeInterval  = int.Parse(Console.ReadLine());
}

//Calling the SyncFolder class to start the sync process
SyncFolder syncFolder = new SyncFolder(sourceFolderPath, replicaFolderPath, syncTimeInterval, new Logger(logFilePath), new SyncFolderOperations());
await syncFolder.ExecuteSyncFolder();
