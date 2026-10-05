
if (args.Length != 4)
{
    Console.WriteLine("You must provide 4 arguments.");
    Console.WriteLine("dotnet run <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>");
    return 1;
}

string sourceFolderPath = Path.GetFullPath(args[0]);
string replicaFolderPath = Path.GetFullPath(args[1]);
string logFilePath = Path.GetFullPath(args[3]);

//Checking if source folder path exists.

if (!Directory.Exists(sourceFolderPath))
{
    Console.WriteLine("Path mentioned does not exist");
    Console.WriteLine("If it exists in the path mentioned and returning the same path, please check if you have the access to the source folder");
    Console.WriteLine($"Checked path: {sourceFolderPath}");
    return 1;
}

//Checking if time interval is a whole number greater than 0
if (!int.TryParse(args[2], out int syncTimeInterval) || syncTimeInterval <= 0)
{
    Console.WriteLine($"Interval must be a whole number of seconds greater than 0, got '{args[2]}'.");
    return 1;
}

if (IsSameOrInside(replicaFolderPath, sourceFolderPath) || IsSameOrInside(sourceFolderPath, replicaFolderPath))
{
    Console.WriteLine("Source and replica must not be the same folder or inside each other.");
    return 1;
}
if (IsSameOrInside(logFilePath, sourceFolderPath) || IsSameOrInside(logFilePath, replicaFolderPath))
{
    Console.WriteLine("Log file must not be inside the source or replica folder.");
    return 1;
}

Directory.CreateDirectory(replicaFolderPath);
Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);

//Calling the SyncFolder class to start the sync process
SyncFolder syncFolder = new SyncFolder(sourceFolderPath, replicaFolderPath, syncTimeInterval, new Logger(logFilePath), new SyncFolderOperations());
await syncFolder.ExecuteSyncFolder();
return 0;

static bool IsSameOrInside(string path, string folder)
{
    string relative = Path.GetRelativePath(folder, path);
    return relative == "." || (!relative.StartsWith("..") && !Path.IsPathRooted(relative));
}