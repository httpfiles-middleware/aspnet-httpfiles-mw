// -----------------------------------------------------------------------
// <copyright file="HttpFilesGenerator.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

using HttpFilesMW.Core.Extensions;
using HttpFilesMW.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core.Generators;
public class HttpFilesGenerator : IHttpFileGenerator
{
    private readonly IApiDescriptionGroupCollectionProvider apiExplorer;
    private readonly ILogger<HttpFilesGenerator> logger;
    private readonly IHttpContextAccessor contextAccessor;

    public HttpFilesGenerator(
        IApiDescriptionGroupCollectionProvider apiExplorer,
        IHttpContextAccessor contextAccessor,
        ILogger<HttpFilesGenerator> logger)
    {
        this.apiExplorer = apiExplorer ?? throw new ArgumentNullException(nameof(apiExplorer));
        this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public HttpFile GenerateAsync()
    {
        if (this.apiExplorer.ApiDescriptionGroups == null)
        {
            this.logger.LogError("ApiDescriptionGroups is null");
            throw new ArgumentNullException(nameof(this.apiExplorer));
        }

        this.logger.LogInformation("Generating HTTP files from API explorer data");
        return this.GenerateHttpFile();
    }

    private HttpFile GenerateHttpFile()
    {
        var globalVariables = this.GetGlobalVariables();
        var requestEntries = this.GetRequestEntries();

        var httpFile = new HttpFile
        {
            GlobalVariables = globalVariables,
            Requests = requestEntries,
        };

        return httpFile;
    }

    private IDictionary<string, string> GetGlobalVariables()
    {
        var (hostVariable, hostAddress) = this.GetHostAddress();
        return new Dictionary<string, string>
        {
            { hostVariable, hostAddress },
        };
    }

    private IList<RequestEntry> GetRequestEntries()
    {
        var requestEntries = new List<RequestEntry>();
        foreach (var apiDescription in this.apiExplorer.ApiDescriptionGroups.Items.SelectMany(g => g.Items))
        {
            this.logger.LogDebug("Processing API: {Method} {Path}", apiDescription.HttpMethod, apiDescription.RelativePath);
            var requestEntry = new RequestEntry
            {
                HttpMethod = apiDescription.HttpMethod ?? string.Empty,
                Path = apiDescription.RelativePath ?? string.Empty,
            };
            requestEntries.Add(requestEntry);
        }

        return requestEntries;
    }

    private (string, string) GetHostAddress()
    {
        var request = this.contextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return (string.Empty, string.Empty);
        }

        return ($"@{Constants.HostAddressVariable}", $"{request.Scheme}://{request.Host}");
    }
}
