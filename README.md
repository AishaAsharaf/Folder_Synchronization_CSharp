# Folder_Synchronization_CSharp
A program that synchronizes two folders: source and replica.
The program should maintain a full, identical copy of source folder at replica
folder.

# How to Run the Folder Synchronization Program

## Prerequisites

Make sure the following are installed:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Run

Clone the repository and open a terminal in its app folder:

```powershell
git clone https://github.com/AishaAsharaf/Folder_Synchronization_CSharp.git
cd Folder_Synchronization_CSharp/Folder_Sync_C#
```

Run the app with four arguments:

```powershell
dotnet run <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>
```

Example (syncs every 10 seconds):

```powershell
dotnet run "C:\Sync\Source" "C:\Sync\Replica" 10 "C:\Sync\log.txt"
```

- Put paths with spaces in double quotes.
- The replica folder and log file are created if they do not exist.
- Press **Ctrl + C** to stop.

## Test

Check your current folder:

```powershell
pwd
```
Run the tests from the root folder (the one that contains `Folder_Sync_C#.sln`).
If you are still in `Folder_Sync_C#` after running the app, go up one level first:

To check what the contents of the folder is:
```powershell
ls
```

If it is not the root folder, write the below command to go up:
```powershell
cd ..
```

From the root folder:

```powershell
dotnet test
```

The NUnit tests cover new, changed and deleted files, nested and empty folders, and extra content in the replica.

## How it works

Every interval, the app:

1. Deletes files and folders in the replica that are not in the source.
2. Creates folders that are missing in the replica.
3. Copies files that are missing in the replica.
4. Updates replica files whose content differs from the source (compared byte by byte).

Every create, copy, update and delete is logged to the console and the log file.

## Safety checks

- The app stops with a clear message if arguments are missing, the source folder does not exist, or the interval is not a whole number greater than 0.
- Source and replica cannot be the same folder or inside each other, to prevent data loss.
- The log file cannot be inside the source or replica folder.
- If the source becomes unavailable while running (for example, a drive is unplugged), that cycle is skipped and the replica is left untouched. Syncing resumes automatically when the source is back.
- An empty source folder results in an empty replica.
- An error on one file is logged and does not stop the rest of the sync.

## Project structure

```text
Folder_Sync_C#/                 Console app
  Program.cs                    Reads and validates the arguments
  SyncFolder.cs                 Runs the sync on a timer
  SyncFolderOperations.cs       Compares, copies and deletes files and folders
  Logger.cs                     Writes to the console and the log file
Folder_Sync.Tests/              NUnit tests
```

## Rules :

* Synchronization must be one-way: after the synchronization content of the replica
  folder should be modified to exactly match content of the source folder;

* Synchronization should be performed periodically;

* File creation/copying/removal operations should be logged to a file and to the
  console output;

* Folder paths, synchronization interval and log file path should be provided using
  the command line arguments;

* It is undesirable to use third-party libraries that implement folder synchronization;

* It is allowed (and recommended) to use external libraries implementing other well-
  known algorithms. For example, there is no point in implementing yet
  another function that calculates MD5 if you need it for the task – it is perfectly
  acceptable to use a third-party (or built-in) library;
