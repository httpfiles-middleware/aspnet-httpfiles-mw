// -----------------------------------------------------------------------
// <copyright file="IHttpFileGenerator.cs" company="HttpFilesMW">
// HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Models;

namespace HttpFilesMW.Core.Generators;

public interface IHttpFileGenerator
{
    HttpFile GenerateAsync();
}
