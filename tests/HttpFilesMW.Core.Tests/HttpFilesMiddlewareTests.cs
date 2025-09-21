// -----------------------------------------------------------------------
// <copyright file="HttpFilesMiddlewareTests.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
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
    public void Constructor_WhenNextIsNull_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new HttpFilesMiddleware(null!, this.mockGenerator.Object, this.mockLogger.Object));

        exception.ParamName.Should().Be("next");
    }

    [Fact]
    public void Constructor_WhenGeneratorIsNull_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new HttpFilesMiddleware(this.mockNext.Object, null!, this.mockLogger.Object));

        exception.ParamName.Should().Be("generator");
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new HttpFilesMiddleware(this.mockNext.Object, this.mockGenerator.Object, null!));

        exception.ParamName.Should().Be("logger");
    }

    [Fact]
    public void Constructor_WhenAllDependenciesAreNull_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new HttpFilesMiddleware(null!, null!, null!));

        // Should fail on the first null parameter (next)
        exception.ParamName.Should().Be("next");
    }

    [Fact]
    public void Constructor_WhenAllDependenciesAreValid_CreatesInstanceSuccessfully()
    {
        // Arrange & Act
        var middleware = new HttpFilesMiddleware(this.mockNext.Object, this.mockGenerator.Object, this.mockLogger.Object);

        // Assert
        middleware.Should().NotBeNull();
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
        context.Response.Headers.ContentDisposition.Should().Contain("inline");

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

    [Fact]
    public async Task InvokeAsync_WhenPathDoesNotStartWithHttpFiles_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";

        var apiExplorerMock = new Mock<IApiDescriptionGroupCollectionProvider>();

        // Act
        await this.middleware.InvokeAsync(context, apiExplorerMock.Object);

        // Assert
        this.mockNext.Verify(n => n(context), Times.Once);
        this.mockGenerator.Verify(m => m.GenerateAsync(It.IsAny<IApiDescriptionGroupCollectionProvider>()), Times.Never);
        
        // Verify no logging for HTTP files handling
        this.mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling request for")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
