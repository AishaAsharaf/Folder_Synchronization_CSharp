
//Program to run the folder sync application

//Requesting the user to provide source folder path
Console.WriteLine("Provide the source folder path..");
var sourceFolderPath = Console.ReadLine();
while(!Directory.Exists(sourceFolderPath))
{
    Console.WriteLine("If it exists in the path mentioned and returning the does not exist");
    Console.WriteLine("If it exists in the path mentioned and returning the same path, please check if you have the access to the source folder");
    Console.WriteLine("Please enter relevant source folder path");
    sourceFolderPath = Console.ReadLine();
}

//Requesting the user to provide replica folder path
Console.WriteLine("Provide replica folder path..");
var replicaFolderPath = Console.ReadLine();
while(!Directory.Exists(replicaFolderPath))
{
    Console.WriteLine("Replica path mentioned does not exist");
    Console.WriteLine("If it exists in the path mentioned and returning the same path, please check if you have the access to the replica folder");
    Console.WriteLine("Please enter relevant replica folder path");
    replicaFolderPath = Console.ReadLine();
}

//Requesting the user to provide log file path
Console.WriteLine("Provide log file path..");
var logFilePath = Console.ReadLine();
while(!File.Exists(logFilePath))
{
    Console.WriteLine("If it exists in the path mentioned and returning the does not exist, please check if you have the access to the log file");
    Console.WriteLine("Please enter relevant log file path below");
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
