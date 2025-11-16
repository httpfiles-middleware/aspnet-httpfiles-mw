// -----------------------------------------------------------------------
// <copyright file="HttpFile.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HttpFilesMW.Core.Models;
public record HttpFile
{
    public IDictionary<string, string> GlobalVariables { get; init; } = new Dictionary<string, string>();

    public ICollection<RequestEntry> Requests { get; init; } = Array.Empty<RequestEntry>();
}
