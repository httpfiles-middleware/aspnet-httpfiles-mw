// -----------------------------------------------------------------------
// <copyright file="IApiExplorerToHttpFileGenerator.cs" company="HttpFilesMW">
// HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace HttpFilesMW.Core.Generators;
public interface IApiExplorerToHttpFileGenerator
{
    Task<string> GenerateAsync(IApiDescriptionGroupCollectionProvider apiExplorer);
}
