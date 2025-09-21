// -----------------------------------------------------------------------
// <copyright file="DefaultHttpFilesGenerator.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace HttpFilesMW.Core;
public class DefaultHttpFilesGenerator : IApiExplorerToHttpFileGenerator
{
    public async Task<string> GenerateAsync(IApiDescriptionGroupCollectionProvider apiExplorer)
    {
        // Implement the logic to generate the HTTP files content based on the API explorer data
        return await Task.FromResult("Generated HTTP files content");
    }
}
