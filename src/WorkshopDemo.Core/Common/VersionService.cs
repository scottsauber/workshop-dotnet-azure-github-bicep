namespace WorkshopDemo.Core.Common;

public interface IVersionService
{
    string GetVersion();
}

public class VersionService : IVersionService
{
    private readonly IFileService _fileService;
    private static string? _version;

    public VersionService(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public string GetVersion()
    {
        if (string.IsNullOrEmpty(_version))
        {
            _version = _fileService.GetFileContents("version.txt");
        }

        return _version;
    }
}