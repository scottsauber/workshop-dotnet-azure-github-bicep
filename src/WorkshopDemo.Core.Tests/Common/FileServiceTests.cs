using FluentAssertions;
using WorkshopDemo.Core.Common;

namespace WorkshopDemo.Core.Tests.Common;

public class FileServiceTests
{
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        _fileService = new FileService();
    }

    [Fact]
    public void GetFileContents_ShouldReturnFileContents_WhenFileIsValid()
    {
        var fileContents = Guid.NewGuid().ToString();
        var fileName = "file-service-test.txt";
        File.WriteAllText(fileName, fileContents);

        var result = _fileService.GetFileContents(fileName);

        result.Should().Be(fileContents);
    }

    [Fact]
    public void GetFileContents_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var result = () => _fileService.GetFileContents("Nope.txt");

        result.Should().Throw<FileNotFoundException>();
    }
}