 public class SyncFolder
 {
     private readonly string _sourcePath;

     private readonly string _replicaPath;

     private readonly int _intervalSeconds;

     private readonly Logger _logger;

     private readonly SyncFolderOperations _syncFolderOperations; 

     public SyncFolder(string sourcePath, string replicaPath, int timer, Logger logger, SyncFolderOperations syncFolderOperations)
     {
         this._sourcePath = sourcePath;
         this._replicaPath = replicaPath;
         this._intervalSeconds = timer;
         this._logger = logger;
         this._syncFolderOperations = syncFolderOperations;
     }

     public async Task ExecuteSyncFolder()
     {
         using PeriodicTimer setTimer = new(TimeSpan.FromSeconds(_intervalSeconds));

        do
        {
            _logger.Info($"Syncing the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");

            //Checking if source path exists or not,if not then we will skip the sync operation and wait for the next tick to check again.
            if(!Directory.Exists(_sourcePath))
            {
                _logger.Critical($"Source folder {_sourcePath} is not available. Skipping this cycle, replica left untouched.");
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
                _logger.Error($"Access denied to the source folder or files: {ex.Message}");
                continue; // Skip this iteration and wait for the next tick
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while accessing the source folder or files: {ex.Message}");
                continue; // Skip this iteration and wait for the next tick
            }
            
            //Running all sync operations: delete extras, create missing folders, copy new files, update changed files.
            //An empty source results in an empty replica.
            _syncFolderOperations.AllSyncOperations(_sourcePath, sourcefiles, sourcefolders, _replicaPath, _logger);

            _logger.Info($"End of sync for the source folder {_sourcePath} with replica folder {_replicaPath} at {DateTime.Now.ToString()}");

        }
        while (await setTimer.WaitForNextTickAsync());

     }   
 }