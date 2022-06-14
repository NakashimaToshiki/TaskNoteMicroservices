using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Shared;

/// <summary>
/// エラーメッセージの規定値をクラス名とメソッド名にする<see cref="ILogger"/>の拡張メソッドを定義します。
/// </summary>
public static class LoggerExtentions
{
    private static string GetText(string filePath, string memberName) => $"{Path.GetFileNameWithoutExtension(filePath)}.{memberName}";

    public static void LogWarning(this ILogger me, Exception e, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        => LoggerExtensions.LogWarning(me, e, GetText(filePath, memberName));

    public static void LogError(this ILogger me, Exception e, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        => LoggerExtensions.LogError(me, e, GetText(filePath, memberName));

    public static void LogCritical(this ILogger me, Exception e, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
        => LoggerExtensions.LogCritical(me, e, GetText(filePath, memberName));
}

