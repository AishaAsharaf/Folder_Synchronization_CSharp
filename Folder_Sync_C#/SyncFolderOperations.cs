public class SyncFolderOperations
{
    
    public void AllSyncOperations(string sourcePath,string[] sourcefiles, string[] sourcefolders, string replicaPath, Logger logger)
    {
     CheckIfSourceFolderIsEmpty(sourcefiles, sourcefolders, replicaPath, logger);
     DeleteFilesNotInSource(sourcePath, replicaPath, logger);
     DeleteFoldersNotInSource(sourcePath, replicaPath, logger);
     CreateFoldersNotInReplica(sourcePath, replicaPath, logger);
     CreateFilesNotInReplica(sourcePath, replicaPath, logger);
     FileContentsAreSame(sourcePath, replicaPath, logger);  
    }

     private void CheckIfSourceFolderIsEmpty(string[] sourcefiles, string[] sourcefolders, string replicaPath, Logger logger)
    {   try{
        //emptying replica folder if source folder is empty.
             if (sourcefiles.Length == 0 && sourcefolders.Length == 0)
             {
                Console.WriteLine("Source folder is empty, so the replica folder contents will be emptied");
                logger.Info("Source folder is empty, so the replica folder contents will be emptied");
                DeleteAllFilesAndFolders(replicaPath,logger);
             }
            }
        catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to the source folder or files: {ex.Message}");
                logger.Error($"Access denied to the source folder or files: {ex.Message}");
            }
        catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while checking if source folder is empty: {ex.Message}");
                logger.Error($"Unexpected error while checking if source folder is empty: {ex.Message}");
            }
    }
     private void DeleteAllFilesAndFolders(string path, Logger logger)
     {
         // Delete all files in the directory
         foreach (string file in Directory.GetFiles(path))
         {   
            try
            {
             File.Delete(file);
             Console.WriteLine($"Deleted this file {file}.....{DateTime.Now.ToString()}");
             logger.Info($"Deleted this file {file}.....{DateTime.Now.ToString()}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to the file {file}: {ex.Message}");
                logger.Error($"Access denied to the file {file}: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error while deleting file {file}: {ex.Message}");
                logger.Error($"I/O error while deleting file {file}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while deleting file {file}: {ex.Message}");
                logger.Error($"Unexpected error while deleting file {file}: {ex.Message}");
            }
         }
         // Recursively delete all subdirectories
         foreach (string directory in Directory.GetDirectories(path))
         {
             DeleteAllFilesAndFolders(directory, logger);
             try{
                Directory.Delete(directory);
                Console.WriteLine($"Deleted this folder {directory}.....{DateTime.Now.ToString()}");
                logger.Info($"Deleted this folder {directory}.....{DateTime.Now.ToString()}");
             }
             catch (UnauthorizedAccessException ex)
             {
                 Console.WriteLine($"Access denied to the folder {directory}: {ex.Message}");
                 logger.Error($"Access denied to the folder {directory}: {ex.Message}");
             }
             catch (IOException ex)
             {
                 Console.WriteLine($"I/O error while deleting folder {directory}: {ex.Message}");
                 logger.Error($"I/O error while deleting folder {directory}: {ex.Message}");
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Unexpected error while deleting folder {directory}: {ex.Message}");
                 logger.Error($"Unexpected error while deleting folder {directory}: {ex.Message}");
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
                 Console.WriteLine($"Deleted this file {replicaFile}.....{DateTime.Now.ToString()}");
                 logger.Info($"Deleted this file {replicaFile}.....{DateTime.Now.ToString()}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied to the file {replicaFile}: {ex.Message}");
                    logger.Error($"Access denied to the file {replicaFile}: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"I/O error while deleting file {replicaFile}: {ex.Message}");
                    logger.Error($"I/O error while deleting file {replicaFile}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error while deleting file {replicaFile}: {ex.Message}");
                    logger.Error($"Unexpected error while deleting file {replicaFile}: {ex.Message}");
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
                     Console.WriteLine($"Deleted this folder {replicaFolder}.....{DateTime.Now.ToString()}");
                     logger.Info($"Deleted this folder {replicaFolder}.....{DateTime.Now.ToString()}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Access denied to the folder {replicaFolder}: {ex.Message}");
                        logger.Error($"Access denied to the folder {replicaFolder}: {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"I/O error while deleting folder {replicaFolder}: {ex.Message}");
                        logger.Error($"I/O error while deleting folder {replicaFolder}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error while deleting folder {replicaFolder}: {ex.Message}");
                        logger.Error($"Unexpected error while deleting folder {replicaFolder}: {ex.Message}");
                    }
                 }
             }
             else
             {
                 return;
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
                 Console.WriteLine($"Created replica folder : {folderPath}...{DateTime.Now.ToString()}");
                 logger.Info($"Created replica folder : {folderPath}...{DateTime.Now.ToString()}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Unexpected error while creating folder {folderPath}: {ex.Message}");
                    logger.Error($"Unexpected error while creating folder {folderPath}: {ex.Message}");
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
                    Console.WriteLine($"Created replica file : {filePath}...{DateTime.Now.ToString()}");
                    logger.Info($"Created replica file : {filePath}...{DateTime.Now.ToString()}");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found while comparing files: {ex.Message}");
                logger.Error($"File not found while comparing files: {ex.Message}");
            }
            catch (IOException ex)
            {
                    Console.WriteLine($"I/O error while comparing files: {ex.Message}");
                    logger.Error($"I/O error while comparing files: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                    Console.WriteLine($"Access denied while comparing files: {ex.Message}");
                    logger.Error($"Access denied while comparing files: {ex.Message}");
            }
            catch (Exception ex)
            {
                    Console.WriteLine($"Unexpected error while comparing files: {ex.Message}");
                    logger.Error($"Unexpected error while comparing files: {ex.Message}");
            }
             
         }

     }
     //Check byte by byte if all files are the same, if not edit the replica to make it the same
     //After the abpve method, only change would be contents inside the file
     private void FileContentsAreSame(string sourcePath, string replicaPath, Logger logger)
     {
        string[] sourcefiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
       
        foreach(var file in sourcefiles)
        {
             string relativePath = Path.GetRelativePath(sourcePath, file);
             string replicaFilePath = Path.Combine(replicaPath, relativePath);

             try{

                if(!AreFilesEqual(file, replicaFilePath, logger))
                  {
                    Console.WriteLine($"Source file: {file} and Replica file: {replicaFilePath} are not the same, updating to make it the same.....");
                    logger.Info($"Source file: {file} and Replica file: {replicaFilePath} are not the same, updating to make it the same.....");
                    File.Copy(file, replicaFilePath, true);
                    Console.WriteLine($"Updated replica file : {replicaFilePath}...");
                    logger.Info($"Updated replica file : {replicaFilePath}...");
                   }
                }
                catch (FileNotFoundException ex)
                {
                Console.WriteLine($"File not found while comparing files: {ex.Message}");
                logger.Error($"File not found while comparing files: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"I/O error while comparing files: {ex.Message}");
                    logger.Error($"I/O error while comparing files: {ex.Message}");
                    
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied while comparing files: {ex.Message}");
                    logger.Error($"Access denied while comparing files: {ex.Message}");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error while comparing files: {ex.Message}");
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