// -----------------------------------------------------------------------
// <copyright file="RequestEntryExtensionsTests.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Extensions;
using HttpFilesMW.Core.Models;

namespace HttpFilesMW.Core.Tests.Extensions;
public class RequestEntryExtensionsTests
{
    [Fact]
    public void GivenRequestEntry_WhenToHttpFileFormatIsCalled_ThenExpectedResultIsReturned()
    {
        // Arrange
        var requestEntry = new RequestEntry()
        {
            HttpMethod = "GET",
            Path = "api/users",
        };

        // Act
        var actual = requestEntry.ToHttpFileFormat();

        // Assert
        actual.Should().Be("""
                GET {{HostAddress}}/api/users

                ###
                """);
    }
}
