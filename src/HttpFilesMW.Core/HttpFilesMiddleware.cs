// -----------------------------------------------------------------------
// <copyright file="HttpFilesMiddleware.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Extensions;
using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core;

public class HttpFilesMiddleware
{
    private readonly RequestDelegate next;
    private readonly IHttpFileGenerator generator;
    private readonly ILogger<HttpFilesMiddleware> logger;

    public HttpFilesMiddleware(RequestDelegate next, IHttpFileGenerator generator, ILogger<HttpFilesMiddleware> logger)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(Constants.HttpFilesPath, out var remainingPath))
        {
            this.logger.LogInformation("Handling request for {Path}", context.Request.Path);

            var httpFile = this.generator.GenerateAsync();
            context.Response.ContentType = "text/plain";
            context.Response.Headers.ContentDisposition = "inline";

            await context.Response.WriteAsync(httpFile.ToHttpFileString());
        }

        await this.next(context);
    }
}
