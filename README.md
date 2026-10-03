# Folder_Synchronization_CSharp
A program that synchronizes two folders: source and replica.
The program should maintain a full, identical copy of source folder at replica
folder.

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

# How to Run the Folder Synchronization Program

## Prerequisites

Make sure the following are installed:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* Git (if cloning the repository)

## 1. Clone the repository

Clone the repository to a location of your choice:

```powershell
git clone <repository-url>
```

Then navigate into the repository:

```powershell
cd Folder_Synchronization_C#
```

The repository should contain a structure similar to:

```text
Folder_Synchronization_C#
│
├── Folder_Sync_CSharp
│   ├── Folder_Sync_CSharp.csproj
│   ├── Program.cs
│   ├── SyncFolder.cs
│   └── ...
│
├── .gitignore
├── README.md
└── Folder_Sync_CSharp.sln
```

## 2. Navigate to the project directory

The `dotnet run` command must be executed from the directory containing the `.csproj` file.

Navigate to:

```powershell
cd Folder_Sync_CSharp
```

You should now be in:

```text
Folder_Synchronization_C#\Folder_Sync_CSharp
```

Verify that the project file exists:

```powershell
dir *.csproj
```

You should see:

```text
Folder_Sync_CSharp.csproj
```

> **Important:** The exact location of the repository on your computer does not matter. Do not use a hard-coded path such as `C:\Users\...\Desktop\...`. The important requirement is that the terminal's current directory is the `Folder_Sync_CSharp` directory containing the `.csproj` file.

## 3. Build the project

Run:

```powershell
dotnet build
```

A successful build should display:

```text
Build succeeded
```

Warnings may be displayed, but the build can still succeed as long as there are no errors.

## 4. Prepare the folders

Create three locations for:

* **Source folder** – contains the files and folders to synchronize.
* **Replica folder** – will be synchronized to match the source folder.
* **Log file** – stores synchronization activity and errors.

For example, you could create:

```text
C:\FolderSync\Source
C:\FolderSync\Replica
C:\FolderSync\log.txt
```

You may choose **any locations you want**.

## 5. Run the program

The program requires four command-line arguments:

```text
<sourcePath> <replicaPath> <intervalSeconds> <logFilePath>
```

Run:

```powershell
dotnet run "<sourcePath>" "<replicaPath>" <intervalSeconds> "<logFilePath>"
```

### Example

If your folders are:

```text
C:\FolderSync\Source
C:\FolderSync\Replica
C:\FolderSync\log.txt
```

run:

```powershell
dotnet run "C:\FolderSync\Source" "C:\FolderSync\Replica" 10 "C:\FolderSync\log.txt"
```

This means:

```text
Source:           C:\FolderSync\Source
Replica:          C:\FolderSync\Replica
Interval:         10 seconds
Log file:         C:\FolderSync\log.txt
```

The program will synchronize the replica folder with the source folder every 10 seconds.

## 6. Using paths containing spaces

If a path contains spaces, enclose the path in double quotes.

For example:

```powershell
dotnet run "C:\My Projects\Source Folder" "C:\My Projects\Replica Folder" 10 "C:\My Projects\sync.log"
```

## 7. Stop the program

To stop the running synchronization program, press:

```text
Ctrl + C
```

## 8. Running the compiled application

After building the project, the compiled DLL will be located under:

```text
bin\Debug\net10.0\Folder_Sync_CSharp.dll
```

From the project directory, it can be run with:

```powershell
dotnet .\bin\Debug\net10.0\Folder_Sync_CSharp.dll "<sourcePath>" "<replicaPath>" <intervalSeconds> "<logFilePath>"
```

For normal development and testing, `dotnet run` is recommended.


### Running Tests

Run the NUnit tests with:

```powershell
dotnet test .\Folder_Sync.Tests\Folder_Sync.Tests.csproj
```

The test project contains 5 NUnit tests covering the main folder synchronization operations.

If all tests pass, the output will include:

```text
NUnit3TestExecutor discovered 5 of 5 NUnit test cases

Test summary: total: 5, failed: 0, succeeded: 5, skipped: 0
```

The tests verify that:

1. A file existing in the source folder is copied to the replica.
2. A file existing only in the replica is deleted.
3. A changed source file updates the corresponding replica file.
4. Missing folders are created in the replica.
5. Nested files preserve their directory structure in the replica.

`[SetUp]` runs before each test to create isolated temporary source and replica folders, while `[TearDown]` removes them after each test.
