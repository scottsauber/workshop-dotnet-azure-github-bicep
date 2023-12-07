namespace WorkshopDemo.Core.Common;

public interface IFileService
{
    string GetFileContents(string fileName);
}

public class FileService : IFileService
{
    public string GetFileContents(string fileName)
    {
        return File.ReadAllText(fileName);
    }
}