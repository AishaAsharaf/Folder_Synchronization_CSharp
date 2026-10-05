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

        do
        {
            Logger.Info($"Syncing the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");

            //Chceking if source path exists or not,if not then we will exit the program.
            if(!Directory.Exists(_sourcePath))
            {
                Logger.Critical("Source path mentioned does not exist, please enter relevant source path...If error still persists even if the path exists ..check if you have access");
                continue;
            }

            //Checking if replica folder exist, if it exists no folder will be created
            Directory.CreateDirectory(_replicaPath);

            string[] sourcefiles;
            string[] sourcefolders;

            try
            {
                //Getting all the files and folders from source folder to compare with replica folder.
                sourcefiles = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);
                sourcefolders = Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error($"Access denied to the source folder or files: {ex.Message}");
                continue; // Skip this iteration and wait for the next tick
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while accessing the source folder or files: {ex.Message}");
                continue; // Skip this iteration and wait for the next tick
            }
            
            //Emptying replica folder if source folder is empty.
            //All the necessary sync operation will be in this method, which will be called in the ExecuteSyncFolder method.
            SyncFolderOperations.AllSyncOperations(_sourcePath, sourcefiles, sourcefolders, _replicaPath, Logger);

            Logger.Info($"End of sync for the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");

        }
        while (await setTimer.WaitForNextTickAsync());

     }   
 }