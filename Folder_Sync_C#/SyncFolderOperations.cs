public class SyncFolderOperations
{
    
    public void AllSyncOperations(string sourcePath,string[] sourcefiles, string[] sourcefolders, string replicaPath, Logger logger)
    {
     DeleteFilesNotInSource(sourcePath, replicaPath, logger);
     DeleteFoldersNotInSource(sourcePath, replicaPath, logger);
     CreateFoldersNotInReplica(sourcePath, replicaPath, logger);
     CreateFilesNotInReplica(sourcePath, replicaPath, logger);
     FileContentsAreSame(sourcePath, replicaPath, logger);  
    }

     private void DeleteAllFilesAndFolders(string path, Logger logger)
     {
         // Delete all files in the directory
         foreach (string file in Directory.GetFiles(path))
         {   
            try
            {
             File.Delete(file);
             logger.Info($"Deleted this file {file}.....{DateTime.Now.ToString()}");
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Error($"Access denied to the file {file}: {ex.Message}");
                continue;
            }
            catch (IOException ex)
            {
                logger.Error($"I/O error while deleting file {file}: {ex.Message}");
                continue;
            }
            catch (Exception ex)
            {
                logger.Error($"Unexpected error while deleting file {file}: {ex.Message}");
                continue;
            }
         }
         // Recursively delete all subdirectories
         foreach (string directory in Directory.GetDirectories(path))
         {
             DeleteAllFilesAndFolders(directory, logger);
             try{
                Directory.Delete(directory);
                logger.Info($"Deleted this folder {directory}.....{DateTime.Now.ToString()}");
                continue;
             }
             catch (UnauthorizedAccessException ex)
             {
                 logger.Error($"Access denied to the folder {directory}: {ex.Message}");
                 continue;
             }
             catch (IOException ex)
             {
                 logger.Error($"I/O error while deleting folder {directory}: {ex.Message}");
                 continue;
             }
             catch (Exception ex)
             {
                 logger.Error($"Unexpected error while deleting folder {directory}: {ex.Message}");
                 continue;
             }
         }
     }

    //Check if both folder and files are equal in both paths, if not return the contents thats in source but not in replica and vice versa.
    //Delete the files and folders that are not in source but in replica.
     private void DeleteFilesNotInSource(string sourcePath, string replicaPath, Logger logger)
     { 
        string[] replicaFiles;
        try
        {
            // Get all files in the replica folder
         replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            logger.Error(
                $"Could not get replica files: {ex.Message}");

            return;
        }
         
         foreach (string replicaFile in replicaFiles)
         {
             // Get the relative path of the file in the replica folder
             string relativePath = Path.GetRelativePath(replicaPath, replicaFile);
             // Construct the corresponding file path in the source folder
             string sourceFile = Path.Combine(sourcePath, relativePath);
             // If the file does not exist in the source folder, delete it from the replica folder
             if (!File.Exists(sourceFile))
             {
                try
                {
                 File.Delete(replicaFile);
                 logger.Info($"Deleted this file {replicaFile}.....{DateTime.Now.ToString()}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.Error($"Access denied to the file {replicaFile}: {ex.Message}");
                    continue;
                }
                catch (IOException ex)
                {
                    logger.Error($"I/O error while deleting file {replicaFile}: {ex.Message}");
                    continue;
                }
                catch (Exception ex)
                {
                    logger.Error($"Unexpected error while deleting file {replicaFile}: {ex.Message}");
                    continue;
                }
             }  
         }

     }
     private void DeleteFoldersNotInSource(string sourcePath, string replicaPath, Logger logger)
     {
        string[] replicaFolders;
        try{
         // Get all folders in the replica folder
         replicaFolders = Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            logger.Error(
                $"Could not get replica folders: {ex.Message}");

            return;
        }
         foreach (string replicaFolder in replicaFolders)
         {
             if (Directory.Exists(replicaFolder))
             {
                 // Get the relative path of the folder in the replica folder
                 string relativePath = Path.GetRelativePath(replicaPath, replicaFolder);
                 // Construct the corresponding folder path in the source folder
                 string sourceFolder = Path.Combine(sourcePath, relativePath);
                 // If the folder does not exist in the source folder, delete it from the replica folder
                 if (!Directory.Exists(sourceFolder))
                 {
                    try{
                     DeleteAllFilesAndFolders(replicaFolder, logger);
                     Directory.Delete(replicaFolder);
                     logger.Info($"Deleted this folder {replicaFolder}.....{DateTime.Now.ToString()}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        logger.Error($"Access denied to the folder {replicaFolder}: {ex.Message}");
                        continue;
                    }
                    catch (IOException ex)
                    {
                        logger.Error($"I/O error while deleting folder {replicaFolder}: {ex.Message}");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Unexpected error while deleting folder {replicaFolder}: {ex.Message}");
                        continue;
                    }
                 }
             }
             else
             {
                 continue;
             }
            
         }

     }

     //Check for folders and files that are in source but not in replica and create a copy of them in replica in the same path.
     private void CreateFoldersNotInReplica(string sourcePath, string replicaPath, Logger logger)
     {
        string[] sourceFolders;
        try{
         // Get all folders in the source folder
          sourceFolders = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            logger.Error(
                $"Could not get source folders: {ex.Message}");

            return;
        }
         foreach (string folder in sourceFolders)
         {
             // Get the relative path of the folder in the source folder
             string relativePath = Path.GetRelativePath(sourcePath, folder);
             // Construct the corresponding folder path in the replica folder
             string folderPath = Path.Combine(replicaPath, relativePath);
             // If the folder does not exist in the replica folder, create it in the replica folder
             if (!Directory.Exists(folderPath))
             {
                try{
                 Directory.CreateDirectory(folderPath);
                 logger.Info($"Created replica folder : {folderPath}...{DateTime.Now.ToString()}");
                }
                catch(Exception ex)
                {
                    logger.Error($"Unexpected error while creating folder {folderPath}: {ex.Message}");
                    continue;
                }
             }

         }

     }
     private void CreateFilesNotInReplica(string sourcePath, string replicaPath, Logger logger)
     {
        string[] sourceFiles;
        try{
         // Get all files in the source folder
         sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            logger.Error(
                $"Could not get source files: {ex.Message}");

            return;
        }
         foreach (string file in sourceFiles)
         {
             // Get the relative path of the folder in the source folder
             string relativePath = Path.GetRelativePath(sourcePath, file);
             // Construct the corresponding folder path in the replica folder
             string filePath = Path.Combine(replicaPath, relativePath);
            // If the folder does not exist in the replica folder, create it in the replica folder

            try
            {
                if (!File.Exists(filePath))
                {
                    File.Copy(file,filePath);
                    logger.Info($"Created replica file : {filePath}...{DateTime.Now.ToString()}");
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.Error($"File not found while comparing files: {ex.Message}");
                continue;
            }
            catch (IOException ex)
            {
                    logger.Error($"I/O error while comparing files: {ex.Message}");
                    continue;
            }
            catch (UnauthorizedAccessException ex)
            {
                    logger.Error($"Access denied while comparing files: {ex.Message}");
                    continue;
            }
            catch (Exception ex)
            {
                    logger.Error($"Unexpected error while comparing files: {ex.Message}");
                    continue;
            }
             
         }

     }
    //Check byte by byte if all files are the same, if not edit the replica to make it the same
    //After the abpve method, only change would be contents inside the file
    private void FileContentsAreSame(string sourcePath, string replicaPath, Logger logger)
    {
        string[] sourcefiles;

        try { 
        sourcefiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        }
        catch(Exception ex)
        {
            logger.Error($"Unexpected error while comparing files: {ex.Message}");
            return;
        }
        foreach (var file in sourcefiles)
        {
             string relativePath = Path.GetRelativePath(sourcePath, file);
             string replicaFilePath = Path.Combine(replicaPath, relativePath);

             try{

                if(!AreFilesEqual(file, replicaFilePath, logger))
                  {
                    logger.Info($"Source file: {file} and Replica file: {replicaFilePath} are not the same, updating to make it the same.....");
                    File.Copy(file, replicaFilePath, true);
                    logger.Info($"Updated replica file : {replicaFilePath}...");
                   }
                }
                catch (FileNotFoundException ex)
                {
                    logger.Error($"File not found while comparing files: {ex.Message}");
                }
                catch (IOException ex)
                {
                    logger.Error($"I/O error while comparing files: {ex.Message}");
                    
                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.Error($"Access denied while comparing files: {ex.Message}");
                    
                }
                catch (Exception ex)
                {
                    logger.Error($"Unexpected error while comparing files: {ex.Message}");
                    
                }
         }
     }
     private bool AreFilesEqual(string sourcefile, string replicafile, Logger logger)
    {   
        using FileStream sourceStream = File.OpenRead(sourcefile);
        using FileStream replicaStream = File.OpenRead(replicafile);

        if (sourceStream.Length != replicaStream.Length)
            {
                return false;
            }

        int sourceByte;
        int replicaByte;

        do
            {
                sourceByte = sourceStream.ReadByte();
                replicaByte = replicaStream.ReadByte();

                if (sourceByte != replicaByte)
                {
                    return false;
                }

            } while (sourceByte != -1);

        return true;
    }

        
    
}