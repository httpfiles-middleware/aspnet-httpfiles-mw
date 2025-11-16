// -----------------------------------------------------------------------
// <copyright file="DependencyInjection.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HttpFilesMW.Core;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddHttpFilesProcessing(this IServiceCollection services)
    {
        bool alreadyRegistered = services.Any(s => s.ServiceType == typeof(IHttpContextAccessor));
        if (!alreadyRegistered)
        {
            services.AddHttpContextAccessor();
        }

        services.AddSingleton<IHttpFileGenerator>(sp =>
        {
            var apiExplorer = sp.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var logger = sp.GetRequiredService<ILogger<HttpFilesGenerator>>();
            return new HttpFilesGenerator(apiExplorer, contextAccessor, logger);
        });

        return services;
    }

    public static IApplicationBuilder UseHttpFilesProcessing(this IApplicationBuilder app)
    {
        return app.UseMiddleware<HttpFilesMiddleware>();
    }
}
