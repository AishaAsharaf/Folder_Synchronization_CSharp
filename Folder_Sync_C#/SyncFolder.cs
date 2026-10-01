 public class SyncFolder
 {
     private string _sourcePath;

     private string _replicaPath;

     public int Timer { get; set; }

     public Logger Logger { get; set; }

     public SyncFolderOperations SyncFolderOperations { get; set; } 

     public SyncFolder(string sourcePath, string replicaPath, int timer, Logger logger, SyncFolderOperations syncFolderOperations)
     {
         this._sourcePath = sourcePath;
         this._replicaPath = replicaPath;
         this.Timer = timer;
         this.Logger = logger;
         this.SyncFolderOperations = syncFolderOperations;
     }

     public async Task ExecuteSyncFolder()
     {
         using PeriodicTimer setTimer = new(TimeSpan.FromSeconds(Timer));

         while (await setTimer.WaitForNextTickAsync())
         {
             
             //Chceking if source path exists or not,if not then we will exit the program and ask the user for a relevent source path.
             if (!Directory.Exists(_sourcePath))
             {
                 Logger.Log("Source path mentioned does not exist, please enter relevant source path");
             }

             //Creating a replica folder if it does not exist, if it exists no folder will be created
             Directory.CreateDirectory(_replicaPath);

             string[] sourcefiles = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);
             string[] sourcefolders = Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

             //emptying replica folder if source folder is empty.
             if (sourcefiles.Length == 0 && sourcefolders.Length == 0)
             {
                Logger.Log("Source folder is empty, the replica folder also will be empty");
                SyncFolderOperations.DeleteAllFilesAndFolders(_replicaPath,Logger);

             }

             //All the necessary sync operation will be in this method, which will be called in the ExecuteSyncFolder method.
             SyncFolderOperations.AllSyncOperations(_sourcePath, _replicaPath, Logger);

            //  //Check if both folder and files are equal in both paths, if not return the contents thats in source but not in replica and vice versa.
            //  //Delete the files and folders that are not in source but in replica.
            //  SyncFolderOperations.DeleteFilesNotInSource(_sourcePath, _replicaPath,Logger);
            //  SyncFolderOperations.DeleteFoldersNotInSource(_sourcePath, _replicaPath,Logger);

             //Check for folders and files that are in source but not in replica and create a copy of them in replica in the same path.
            //  SyncFolderOperations.CreateFoldersNotInReplica(_sourcePath, _replicaPath, Logger);
            //  SyncFolderOperations.CreateFilesNotInReplica(_sourcePath, _replicaPath, Logger);

            //  //Check byte by byte if all files are the same, if not edit the replica to make it the same
            //  //After the abpve method, only change would be contents inside the file
            //  SyncFolderOperations.FileContentsAreSame(_sourcePath, _replicaPath, Logger);
         }

     }   
 }