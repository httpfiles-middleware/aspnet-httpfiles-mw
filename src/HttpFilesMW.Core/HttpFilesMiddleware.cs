// -----------------------------------------------------------------------
// <copyright file="HttpFilesMiddleware.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core;

public class HttpFilesMiddleware
{
    private readonly RequestDelegate next;
    private readonly IApiExplorerToHttpFileGenerator generator;
    private readonly ILogger<HttpFilesMiddleware> logger;

    public HttpFilesMiddleware(RequestDelegate next, IApiExplorerToHttpFileGenerator generator, ILogger<HttpFilesMiddleware> logger)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    public async Task InvokeAsync(HttpContext context, IApiDescriptionGroupCollectionProvider apiExplorer)
    {
        if (context.Request.Path.StartsWithSegments(Constants.HttpFilesPath, out var remainingPath))
        {
            this.logger.LogInformation("Handling request for {Path}", context.Request.Path);

            var httpFilesContent = await this.generator.GenerateAsync(apiExplorer);
            context.Response.ContentType = "text/plain";
            context.Response.Headers.ContentDisposition = "inline";

            await context.Response.WriteAsync(httpFilesContent);
        }

        await this.next(context);
    }
}
