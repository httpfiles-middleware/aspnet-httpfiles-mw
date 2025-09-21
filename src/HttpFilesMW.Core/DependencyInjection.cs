// -----------------------------------------------------------------------
// <copyright file="DependencyInjection.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HttpFilesMW.Core.Generators;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HttpFilesMW.Core;
public static class DependencyInjection
{
    public static IServiceCollection AddHttpFilesProcessing(this IServiceCollection services)
    {
        services.AddSingleton<IApiExplorerToHttpFileGenerator, DefaultHttpFilesGenerator>();
        return services;
    }

    public static IApplicationBuilder UseHttpFilesProcessing(this IApplicationBuilder app)
    {
        return app.UseMiddleware<HttpFilesMiddleware>();
    }
}
