using FluentAssertions;
using NSubstitute;
using WorkshopDemo.Core.Common;

namespace WorkshopDemo.Tests.Common;

public class VersionServiceTests
{
    [Fact]
    public void GetVersion_ShouldRetrieveFileContentsOnce_WhenVersionServiceIsCreatedMultipleTimesToProtectFromWrongServiceLifetimeRegistration()
    {
        var fileContents = Guid.NewGuid().ToString();
        var fileService = Substitute.For<IFileService>();
        var versionFilePath = "version.txt";
        fileService.GetFileContents(versionFilePath).Returns(fileContents);

        var version1 = new VersionService(fileService).GetVersion();
        var version2 = new VersionService(fileService).GetVersion();

        version1.Should().Be(fileContents);
        version2.Should().Be(fileContents);
        fileService.Received(1).GetFileContents(versionFilePath);
    }
}