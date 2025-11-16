// -----------------------------------------------------------------------
// <copyright file="RequestEntryExtensions.cs" company="HttpFilesMW">
// Copyright © HttpFilesMW. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

using HttpFilesMW.Core.Models;

namespace HttpFilesMW.Core.Extensions;
public static class RequestEntryExtensions
{
    public static string ToHttpFileFormat(this RequestEntry requestEntry)
    {
        if (requestEntry == null)
        {
            throw new ArgumentNullException(nameof(requestEntry));
        }

        var sb = new StringBuilder();
        sb.Append(requestEntry.GetFormattedPath());
        return sb.ToString();
    }

    private static string GetFormattedPath(this RequestEntry requestEntry)
    {
        var sb = new StringBuilder();

        sb.AppendLine(FormattableString.Invariant($"{requestEntry.HttpMethod} {{{{{Constants.HostAddressVariable}}}}}/{requestEntry.Path}"));
        sb.AppendLine();
        sb.AppendLine("###");
        return sb.ToString().Trim();
    }
}
