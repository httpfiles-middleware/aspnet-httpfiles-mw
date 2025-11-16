// -----------------------------------------------------------------------
// <copyright file="HttpFilesGeneratorTests.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core.Tests.Generators;

public class HttpFilesGeneratorTests
{
    private readonly Mock<IApiDescriptionGroupCollectionProvider> mockApiProfile;
    private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
    private readonly Mock<ILogger<HttpFilesGenerator>> mockLogger;
    private readonly HttpFilesGenerator generator;

    public HttpFilesGeneratorTests()
    {
        this.mockApiProfile = new Mock<IApiDescriptionGroupCollectionProvider>();
        this.mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        this.mockLogger = new Mock<ILogger<HttpFilesGenerator>>();
        this.generator = new HttpFilesGenerator(this.mockApiProfile.Object, this.mockHttpContextAccessor.Object, this.mockLogger.Object);
    }

    [Fact]
    public void GenerateAsync_WhenApiExplorerIsEmpty_ReturnsGeneratedContent()
    {
        // Arrange
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup>(), 1);
        this.mockApiProfile.Setup(x => x.ApiDescriptionGroups).Returns(collection);

        // Act
        var actual = this.generator.GenerateAsync();

        // Assert
        actual.Should().NotBeNull();
        //actual.Should().Be(string.Empty);

        // Verify logging
        this.mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Generating HTTP files from API explorer data")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void GenerateAsync_WhenApiExplorerHasSingleEndpoint_ReturnsGeneratedContent()
    {
        // Arrange
        var apiDescription = new ApiDescription
        {
            HttpMethod = "GET",
            RelativePath = "api/users",
            ActionDescriptor = new ActionDescriptor
            {
                DisplayName = "GetUsers",
            },
        };

        var group = new ApiDescriptionGroup("v1", new List<ApiDescription> { apiDescription });
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup> { group }, 1);

        this.mockApiProfile.Setup(x => x.ApiDescriptionGroups).Returns(collection);
        this.mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        // Act
        var actual = this.generator.GenerateAsync();

        // Assert
        actual.Should().NotBeNull();
        actual.GlobalVariables[$"@{Constants.HostAddressVariable}"].Should().Be("://");
        actual.Requests.Count.Should().Be(1);
        //actual.Should().Be("""
        //    GET api/users

        //    ###
        //    """);

        // Verify logging
        this.mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Generating HTTP files from API explorer data")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    //[Fact]
    //public async Task GenerateAsync_WhenApiExplorerHasMultipleEndpoints_ReturnsGeneratedContent()
    //{
    //    // Arrange
    //    var apiExplorer = CreateApiExplorerWithMultipleEndpoints();

    //    // Act
    //    var actual = await this.generator.GenerateAsync(apiExplorer);

    //    // Assert
    //    actual.Should().NotBeNull();
    //    actual.Should().Be("""
    //        GET api/users

    //        ###

    //        POST api/users

    //        ###

    //        GET api/products

    //        ###
    //        """);

    //    this.mockLogger.Verify(
    //        x => x.Log(
    //            LogLevel.Information,
    //            It.IsAny<EventId>(),
    //            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Generating HTTP files from API explorer data")),
    //            It.IsAny<Exception>(),
    //            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    //        Times.Once);
    //}

    //[Fact]
    //public async Task GenerateAsync_WhenApiExplorerHasEndpointWithParameters_ReturnsGeneratedContent()
    //{
    //    // Arrange
    //    var apiExplorer = CreateApiExplorerWithParameterizedEndpoint();

    //    // Act
    //    var actual = await this.generator.GenerateAsync(apiExplorer);

    //    // Assert
    //    actual.Should().NotBeNull();
    //    actual.Should().Be("""
    //        GET api/users/{id}

    //        ###
    //        """);

    //    // Verify logging was called
    //    this.mockLogger.Verify(
    //        x => x.Log(
    //            It.IsAny<LogLevel>(),
    //            It.IsAny<EventId>(),
    //            It.IsAny<It.IsAnyType>(),
    //            It.IsAny<Exception>(),
    //            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    //        Times.AtLeastOnce);
    //}

    private static IApiDescriptionGroupCollectionProvider CreateEmptyApiExplorer()
    {
        var mock = new Mock<IApiDescriptionGroupCollectionProvider>();
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup>(), 1);
        mock.Setup(x => x.ApiDescriptionGroups).Returns(collection);
        return mock.Object;
    }

    private static IApiDescriptionGroupCollectionProvider CreateApiExplorerWithSingleEndpoint()
    {
        var mock = new Mock<IApiDescriptionGroupCollectionProvider>();

        var apiDescription = new ApiDescription
        {
            HttpMethod = "GET",
            RelativePath = "api/users",
            ActionDescriptor = new ActionDescriptor
            {
                DisplayName = "GetUsers",
            },
        };

        var group = new ApiDescriptionGroup("v1", new List<ApiDescription> { apiDescription });
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup> { group }, 1);

        mock.Setup(x => x.ApiDescriptionGroups).Returns(collection);
        return mock.Object;
    }

    private static IApiDescriptionGroupCollectionProvider CreateApiExplorerWithMultipleEndpoints()
    {
        var mock = new Mock<IApiDescriptionGroupCollectionProvider>();

        var apiDescriptions = new List<ApiDescription>
        {
            new ApiDescription
            {
                HttpMethod = "GET",
                RelativePath = "api/users",
                ActionDescriptor = new ActionDescriptor { DisplayName = "GetUsers" },
            },
            new ApiDescription
            {
                HttpMethod = "POST",
                RelativePath = "api/users",
                ActionDescriptor = new ActionDescriptor { DisplayName = "CreateUser" },
            },
            new ApiDescription
            {
                HttpMethod = "GET",
                RelativePath = "api/products",
                ActionDescriptor = new ActionDescriptor { DisplayName = "GetProducts" },
            },
        };

        var group = new ApiDescriptionGroup("v1", apiDescriptions);
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup> { group }, 1);

        mock.Setup(x => x.ApiDescriptionGroups).Returns(collection);
        return mock.Object;
    }

    private static IApiDescriptionGroupCollectionProvider CreateApiExplorerWithParameterizedEndpoint()
    {
        var mock = new Mock<IApiDescriptionGroupCollectionProvider>();

        var apiDescription = new ApiDescription
        {
            HttpMethod = "GET",
            RelativePath = "api/users/{id}",
            ActionDescriptor = new ActionDescriptor { DisplayName = "GetUserById" },
        };

        // Add parameters
        apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
        {
            Name = "id",
            Source = BindingSource.Path,
            Type = typeof(int),
            IsRequired = true,
        });

        apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
        {
            Name = "includeDetails",
            Source = BindingSource.Query,
            Type = typeof(bool),
            IsRequired = false,
        });

        // Add supported request formats
        apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat
        {
            MediaType = "application/json",
        });

        // Add supported response types
        apiDescription.SupportedResponseTypes.Add(new ApiResponseType
        {
            StatusCode = 200,
            Type = typeof(object),
        });

        var group = new ApiDescriptionGroup("v1", new List<ApiDescription> { apiDescription });
        var collection = new ApiDescriptionGroupCollection(new List<ApiDescriptionGroup> { group }, 1);

        mock.Setup(x => x.ApiDescriptionGroups).Returns(collection);
        return mock.Object;
    }
}
