// -----------------------------------------------------------------------
// <copyright file="RequestEntry.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HttpFilesMW.Core.Models;
public record RequestEntry
{
    public string HttpMethod { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public string? Body { get; init; }

    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

    public IDictionary<string, string> QueryParameters { get; init; } = new Dictionary<string, string>();

    public IDictionary<string, string> RouteValues { get; init; } = new Dictionary<string, string>();
}
