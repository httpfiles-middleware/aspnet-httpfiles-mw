// -----------------------------------------------------------------------
// <copyright file="HttpFileExtensions.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

using HttpFilesMW.Core.Models;

namespace HttpFilesMW.Core.Extensions;
public static class HttpFileExtensions
{
    public static string ToHttpFileString(this HttpFile httpFile)
    {
        if (httpFile == null)
        {
            throw new ArgumentNullException(nameof(httpFile));
        }

        // Placeholder implementation
        if (httpFile.Requests.Count == 0 && httpFile.GlobalVariables.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append(httpFile.GetGlobalVariablesSection());
        foreach (var request in httpFile.Requests)
        {
            sb.Append(request.ToHttpFileFormat());
            sb.AppendLine();
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }

    private static string GetGlobalVariablesSection(this HttpFile httpFile)
    {
        var sb = new StringBuilder();
        foreach (var variable in httpFile.GlobalVariables)
        {
            sb.AppendLine(FormattableString.Invariant($"{variable.Key}={variable.Value}"));
        }

        sb.AppendLine();
        return sb.ToString();
    }
}
