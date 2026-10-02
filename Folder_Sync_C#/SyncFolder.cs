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
            Logger.Info($"Syncing the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");
             
             //Chceking if source path exists or not,if not then we will exit the program and ask the user for a relevent source path.
             if (!Directory.Exists(_sourcePath))
             {
                 Logger.Critical("Source path mentioned does not exist, please enter relevant source path");
             }

             //Creating a replica folder if it does not exist, if it exists no folder will be created
             Directory.CreateDirectory(_replicaPath);

             string[] sourcefiles = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);
             string[] sourcefolders = Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);
             
             //Emptying replica folder if source folder is empty.
             //All the necessary sync operation will be in this method, which will be called in the ExecuteSyncFolder method.
             SyncFolderOperations.AllSyncOperations(_sourcePath, sourcefiles, sourcefolders, _replicaPath, Logger);

             Logger.Info($"End of sync for the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");

         }

     }   
 }