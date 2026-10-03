using NUnit.Framework;

namespace Folder_Sync.Tests;

public class SyncFolderOperationsTests
{
    private string _sourcePath = null!;
    private string _replicaPath = null!;
    private Logger _logger = null!;

    [SetUp]
    public void Setup()
    {
         _sourcePath = Path.Combine(
            Path.GetTempPath(),
            "FolderSyncTests",
            Guid.NewGuid().ToString());

        _replicaPath = Path.Combine(
            Path.GetTempPath(),
            "FolderSyncTests",
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(_sourcePath);
        Directory.CreateDirectory(_replicaPath);

        string logPath = Path.Combine(
            Path.GetTempPath(),
            $"FolderSyncTest_{Guid.NewGuid()}.log");

        _logger = new Logger(logPath);
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_sourcePath))
            Directory.Delete(_sourcePath, true);

        if (Directory.Exists(_replicaPath))
            Directory.Delete(_replicaPath, true);
    }

    [Test]
    public void AllSyncOperations_FileExistsInSource_CopiesFileToReplica()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourcePath, "test.txt");
        File.WriteAllText(sourceFile, "Hello World");

        string[] sourceFiles =
            Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);

        string[] sourceFolders =
            Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

        SyncFolderOperations operations = new();

        // Act
        operations.AllSyncOperations(
            _sourcePath,
            sourceFiles,
            sourceFolders,
            _replicaPath,
            _logger);

        // Assert
        string replicaFile = Path.Combine(_replicaPath, "test.txt");

        Assert.That(File.Exists(replicaFile), Is.True);
        Assert.That(File.ReadAllText(replicaFile), Is.EqualTo("Hello World"));
    }

    [Test]
    public void AllSyncOperations_FileExistsOnlyInReplica_DeletesFile()
    {
        // Arrange
        string replicaFile = Path.Combine(_replicaPath, "old.txt");
        File.WriteAllText(replicaFile, "Old file");

        string[] sourceFiles =
            Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);

        string[] sourceFolders =
            Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

        SyncFolderOperations operations = new();

        // Act
        operations.AllSyncOperations(
            _sourcePath,
            sourceFiles,
            sourceFolders,
            _replicaPath,
            _logger);

        // Assert
        Assert.That(File.Exists(replicaFile), Is.False);
    }

    [Test]
    public void AllSyncOperations_SourceFileChanged_UpdatesReplicaFile()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourcePath, "test.txt");
        string replicaFile = Path.Combine(_replicaPath, "test.txt");

        File.WriteAllText(sourceFile, "New content");
        File.WriteAllText(replicaFile, "Old content");

        string[] sourceFiles =
            Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);

        string[] sourceFolders =
            Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

        SyncFolderOperations operations = new();

        // Act
        operations.AllSyncOperations(
            _sourcePath,
            sourceFiles,
            sourceFolders,
            _replicaPath,
            _logger);

        // Assert
        Assert.That(
            File.ReadAllText(replicaFile),
            Is.EqualTo("New content"));
    }

    [Test]
    public void AllSyncOperations_SourceFolderDoesNotExistInReplica_CreatesFolder()
    {
        // Arrange
        string sourceFolder = Path.Combine(_sourcePath, "Documents");
        Directory.CreateDirectory(sourceFolder);

        string[] sourceFiles =
            Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);

        string[] sourceFolders =
            Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

        SyncFolderOperations operations = new();

        // Act
        operations.AllSyncOperations(
            _sourcePath,
            sourceFiles,
            sourceFolders,
            _replicaPath,
            _logger);

        // Assert
        string replicaFolder = Path.Combine(_replicaPath, "Documents");

        Assert.That(Directory.Exists(replicaFolder), Is.True);
    }
    
    [Test]
    public void AllSyncOperations_NestedFile_PreservesDirectoryStructure()
    {
        // Arrange
        string sourceFolder = Path.Combine(_sourcePath, "Documents", "Projects");
        Directory.CreateDirectory(sourceFolder);

        string sourceFile = Path.Combine(sourceFolder, "test.txt");
        File.WriteAllText(sourceFile, "Test content");

        string[] sourceFiles =
            Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);

        string[] sourceFolders =
            Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);

        SyncFolderOperations operations = new();

        // Act
        operations.AllSyncOperations(
            _sourcePath,
            sourceFiles,
            sourceFolders,
            _replicaPath,
            _logger);

        // Assert
        string replicaFile = Path.Combine(
            _replicaPath,
            "Documents",
            "Projects",
            "test.txt");

        Assert.That(File.Exists(replicaFile), Is.True);
        Assert.That(File.ReadAllText(replicaFile), Is.EqualTo("Test content"));
    }

}