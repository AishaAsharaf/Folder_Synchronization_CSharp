public class Logger
{
    private string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

     public void Info(string message)
    {
        Log("INFO", message);
    }

    public void Warning(string message)
    {
        Log("WARNING", message);
    }

    public void Error(string message)
    {
        Log("ERROR", message);
    }

    public void Critical(string message)
    {
        Log("CRITICAL", message);
    }

    public void Log(string level, string message)
    {
        using (StreamWriter writer = new StreamWriter(_logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now}:[{level}]  {message}");
        }
    }

}