// -----------------------------------------------------------------------
// <copyright file="HttpFilesMiddlewareTests.cs" company="HttpFilesMW">
// HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core.Tests;

public class HttpFilesMiddlewareTests
{
    private readonly Mock<RequestDelegate> mockNext;
    private readonly Mock<IApiExplorerToHttpFileGenerator> mockGenerator;
    private readonly Mock<ILogger<HttpFilesMiddleware>> mockLogger;
    private readonly HttpFilesMiddleware middleware;

    public HttpFilesMiddlewareTests()
    {
        this.mockNext = new Mock<RequestDelegate>();
        this.mockGenerator = new Mock<IApiExplorerToHttpFileGenerator>();
        this.mockLogger = new Mock<ILogger<HttpFilesMiddleware>>();
        this.middleware = new HttpFilesMiddleware(this.mockNext.Object, this.mockGenerator.Object, this.mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenPathStartsWithHttpFiles_HandlesRequestAndCallsNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/http-files";
        context.Response.Body = new MemoryStream();

        var apiExplorerMock = new Mock<IApiDescriptionGroupCollectionProvider>();

        this.mockGenerator.Setup(m => m.GenerateAsync(apiExplorerMock.Object))
            .ReturnsAsync("You requested the file at path: ");

        // Act
        await this.middleware.InvokeAsync(context, apiExplorerMock.Object);

        // Assert
        context.Response.ContentType.Should().Be("text/plain");
        context.Response.Headers["Content-Disposition"].Should().Contain("inline");

        // Read the response body
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.Should().Be("You requested the file at path: ");

        // Verify next middleware was called
        this.mockNext.Verify(n => n(context), Times.Once);

        this.mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling request for /http-files")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    //[Fact]
    //public async Task InvokeAsync_WhenPathStartsWithHttpFilesAndHasSubPath_ExtractsRemainingPath()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = "/http-files/folder/subfolder/document.pdf";
    //    context.Response.Body = new MemoryStream();

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.ContentType.Should().Be("text/plain");

    //    context.Response.Body.Seek(0, SeekOrigin.Begin);
    //    using var reader = new StreamReader(context.Response.Body);
    //    var responseBody = await reader.ReadToEndAsync();
    //    responseBody.Should().Be("You requested the file at path: /folder/subfolder/document.pdf");

    //    this.mockNext.Verify(n => n(context), Times.Once);
    //}

    //[Fact]
    //public async Task InvokeAsync_WhenPathIsExactlyHttpFiles_HandlesWithEmptyRemainingPath()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = "/http-files";
    //    context.Response.Body = new MemoryStream();

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.ContentType.Should().Be("text/plain");
        
    //    context.Response.Body.Seek(0, SeekOrigin.Begin);
    //    using var reader = new StreamReader(context.Response.Body);
    //    var responseBody = await reader.ReadToEndAsync();
    //    responseBody.Should().Be("You requested the file at path: ");

    //    this.mockNext.Verify(n => n(context), Times.Once);
    //}

    //[Fact]
    //public async Task InvokeAsync_WhenPathDoesNotStartWithHttpFiles_Returns404AndDoesNotCallNext()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = "/api/users";

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

    //    // Verify next middleware was NOT called
    //    this.mockNext.Verify(n => n(context), Times.Never);

    //    // Verify warning log
    //    this.mockLogger.Verify(
    //        x => x.Log(
    //            LogLevel.Warning,
    //            It.IsAny<EventId>(),
    //            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request path /api/users does not start with the expected prefix /http-files")),
    //            It.IsAny<Exception>(),
    //            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    //        Times.Once);
    //}

    //[Fact]
    //public async Task InvokeAsync_WhenPathIsEmpty_Returns404AndDoesNotCallNext()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = "";

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //    this.mockNext.Verify(n => n(context), Times.Never);
    //}

    //[Fact]
    //public async Task InvokeAsync_WhenPathIsRoot_Returns404AndDoesNotCallNext()
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = "/";

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //    this.mockNext.Verify(n => n(context), Times.Never);
    //}

    //[Theory]
    //[InlineData("/http-files/")]
    //[InlineData("/http-files/image.jpg")]
    //[InlineData("/http-files/docs/readme.md")]
    //[InlineData("/http-files/uploads/2024/file.zip")]
    //public async Task InvokeAsync_WhenPathStartsWithHttpFiles_AlwaysCallsNext(string requestPath)
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = requestPath;
    //    context.Response.Body = new MemoryStream();

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    this.mockNext.Verify(n => n(context), Times.Once);
    //}

    //[Theory]
    //[InlineData("/http-file")]  // Missing 's'
    //[InlineData("/HTTP-FILES")] // Different case
    //[InlineData("/api/http-files")]
    //[InlineData("/http-files-backup")]
    //public async Task InvokeAsync_WhenPathDoesNotExactlyMatch_Returns404(string requestPath)
    //{
    //    // Arrange
    //    var context = new DefaultHttpContext();
    //    context.Request.Path = requestPath;

    //    // Act
    //    await this.middleware.InvokeAsync(context);

    //    // Assert
    //    context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //    this.mockNext.Verify(n => n(context), Times.Never);
    //}
}
