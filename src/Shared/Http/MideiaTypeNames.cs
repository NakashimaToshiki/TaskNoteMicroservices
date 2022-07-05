namespace Shared.Http;

/// <summary>
/// <see cref="System.Net.Mime.MediaTypeNames"/>で欠落したタイプを追加
/// </summary>
/// <remarks>
/// https://github.com/dotnet/runtime/issues/1489
/// </remarks>
public static class MideiaTypeNamesExtentions
{
    public static class Application
    {
        public static string ProblemJson = "application/problem+json";
    }
}
