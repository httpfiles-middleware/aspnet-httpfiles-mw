// -----------------------------------------------------------------------
// <copyright file="DefaultHttpFilesGenerator.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core.Generators;
public class DefaultHttpFilesGenerator : IApiExplorerToHttpFileGenerator
{
    private readonly ILogger<DefaultHttpFilesGenerator> logger;

    public DefaultHttpFilesGenerator(ILogger<DefaultHttpFilesGenerator> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GenerateAsync(IApiDescriptionGroupCollectionProvider apiExplorer)
    {
        if (apiExplorer == null || apiExplorer.ApiDescriptionGroups == null)
        {
            this.logger.LogError("API Explorer or its ApiDescriptionGroups is null");
            throw new ArgumentNullException(nameof(apiExplorer));
        }

        this.logger.LogInformation("Generating HTTP files from API explorer data");

        var data = this.ToHttpFileRequests(apiExplorer.ApiDescriptionGroups.Items);

        return await Task.FromResult(data.ToString());
    }

    private string ToHttpFileRequests(IEnumerable<ApiDescriptionGroup> apiDescriptionGroup)
    {
        var apis = apiDescriptionGroup.SelectMany(g => g.Items).ToList();
        var sb = new StringBuilder();
        for (int i = 0; i < apis.Count; i++)
        {
            this.logger.LogDebug("Processing API: {Method} {Path}", apis[i].HttpMethod, apis[i].RelativePath);
            sb.AppendLine(FormattableString.Invariant($"{apis[i].HttpMethod} {apis[i].RelativePath}"));
            sb.AppendLine();

            if (i < apis.Count - 1)
            {
                sb.AppendLine("###");
                sb.AppendLine();
            }
            else
            {
                sb.Append("###");
            }
        }

        return sb.ToString();
    }
}
