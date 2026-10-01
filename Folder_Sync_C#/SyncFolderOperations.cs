public class SyncFolderOperations
{
    
    public void AllSyncOperations(string sourcePath, string replicaPath, Logger logger)
    {
     DeleteFilesNotInSource(sourcePath, replicaPath, logger);
     DeleteFoldersNotInSource(sourcePath, replicaPath, logger);
     CreateFoldersNotInReplica(sourcePath, replicaPath, logger);
     CreateFilesNotInReplica(sourcePath, replicaPath, logger);
     FileContentsAreSame(sourcePath, replicaPath, logger);  

    }
     public void DeleteAllFilesAndFolders(string path, Logger logger)
     {
         // Delete all files in the directory
         foreach (string file in Directory.GetFiles(path))
         {   
             File.Delete(file);
             logger.Log($"Deleted this file {file}.....{DateTime.Now.ToString()}");
         }
         // Recursively delete all subdirectories
         foreach (string directory in Directory.GetDirectories(path))
         {
             DeleteAllFilesAndFolders(directory, logger);
             Directory.Delete(directory);
             logger.Log($"Deleted this folder {directory}.....{DateTime.Now.ToString()}");
         }
     }

    //Check if both folder and files are equal in both paths, if not return the contents thats in source but not in replica and vice versa.
    //Delete the files and folders that are not in source but in replica.
     private void DeleteFilesNotInSource(string sourcePath, string replicaPath, Logger logger)
     {
         // Get all files in the replica folder
         string[] replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);
         foreach (string replicaFile in replicaFiles)
         {
             // Get the relative path of the file in the replica folder
             string relativePath = Path.GetRelativePath(replicaPath, replicaFile);
             // Construct the corresponding file path in the source folder
             string sourceFile = Path.Combine(sourcePath, relativePath);
             // If the file does not exist in the source folder, delete it from the replica folder
             if (!File.Exists(sourceFile))
             {
                 File.Delete(replicaFile);
                 logger.Log($"Deleted this file {replicaFile}.....{DateTime.Now.ToString()}");
             }
         }

     }
     private void DeleteFoldersNotInSource(string sourcePath, string replicaPath, Logger logger)
     {
         // Get all folders in the replica folder
         string[] replicaFolders = Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories);
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
                     DeleteAllFilesAndFolders(replicaFolder, logger);
                     Directory.Delete(replicaFolder);
                     logger.Log($"Deleted this file {replicaFolder}.....{DateTime.Now.ToString()}");
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
         // Get all folders in the source folder
         string[] sourceFolders = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
         foreach (string folder in sourceFolders)
         {
             // Get the relative path of the folder in the source folder
             string relativePath = Path.GetRelativePath(sourcePath, folder);
             // Construct the corresponding folder path in the replica folder
             string folderPath = Path.Combine(replicaPath, relativePath);
             // If the folder does not exist in the replica folder, create it in the replica folder
             if (!Directory.Exists(folderPath))
             {
                 Directory.CreateDirectory(folderPath);
                 logger.Log($"Created replica folder : {folderPath}...{DateTime.Now.ToString()}");
             }
         }

     }
     private void CreateFilesNotInReplica(string sourcePath, string replicaPath, Logger logger)
     {
         // Get all files in the source folder
         string[] sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
         foreach (string file in sourceFiles)
         {
             // Get the relative path of the folder in the source folder
             string relativePath = Path.GetRelativePath(sourcePath, file);
             // Construct the corresponding folder path in the replica folder
             string filePath = Path.Combine(replicaPath, relativePath);
             // If the folder does not exist in the replica folder, create it in the replica folder
             if (!File.Exists(filePath))
             {
                 File.Copy(file,filePath);
                 logger.Log($"Created replica file : {filePath}...{DateTime.Now.ToString()}");
             }
         }

     }
     //Check byte by byte if all files are the same, if not edit the replica to make it the same
     //After the abpve method, only change would be contents inside the file
     private void FileContentsAreSame(string sourcePath, string replicaPath, Logger logger)
     {
         string[] sourcefiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
         string[] replicafiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);

         if(sourcefiles.Length != replicafiles.Length)
         {
             logger.Log("They are not the same content");
         }

         foreach(var file in sourcefiles)
         {
             string relativePath = Path.GetRelativePath(sourcePath, file);
             string replicaFilePath = Path.Combine(replicaPath, relativePath);
             if(!AreFilesEqual(file, replicaFilePath, logger)){
                File.Copy(file, replicaFilePath, true);
                logger.Log($"Updated replica file : {replicaFilePath}...{DateTime.Now.ToString()}");
               }
            ;
         }
     }
     private bool AreFilesEqual(string sourcefile, string replicafile, Logger logger)
    {
         Console.WriteLine($"sourcefile = [{sourcefile}]");
         Console.WriteLine($"Full path = [{Path.GetFullPath(sourcefile)}]");
         Console.WriteLine($"Exists = {File.Exists(sourcefile)}");
         Console.WriteLine($"replicafile = [{replicafile}]");
         Console.WriteLine($"Full path = [{Path.GetFullPath(replicafile)}]");
         Console.WriteLine($"Exists = {File.Exists(replicafile)}");
         
         using FileStream sourcefileStream = File.OpenRead(sourcefile);
         using FileStream replicafileStream = File.OpenRead(replicafile);
         
         if(sourcefileStream.Length != replicafileStream.Length)
         {
             logger.Log("They are not the same content, file sizes are not the same");
             return false;
         }

         int sourceByte;
         int replicaByte;

         do
         {
             sourceByte = sourcefileStream.ReadByte();
             replicaByte = replicafileStream.ReadByte();

             if (sourceByte != replicaByte)
             {
                 logger.Log("They are not the same content, bytes are not the same");
                 return false;
             }
             
         }
         while (sourceByte != -1);
         logger.Log("They are the same content");
         return true;
     
    }
}