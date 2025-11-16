// -----------------------------------------------------------------------
// <copyright file="HttpFileExtensionsTests.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Extensions;
using HttpFilesMW.Core.Models;

namespace HttpFilesMW.Core.Tests.Extensions;

public class HttpFileExtensionsTests
{
    [Fact]
    public void GivenHttpFileWithNoData_WhenExtensionMethodIsCalled_ThenExpectedResultIsReturned()
    {
        // Arrange
        var httpFile = new HttpFile();

        // Act
        var actual = httpFile.ToHttpFileString();

        // Assert
        actual.Should().Be(string.Empty);
    }
}
