namespace SK.Report.Extensions;

public static class HttpRequestExtensions
{
    public static string[] GetUserLanguages(this HttpRequest request) =>
        request.GetTypedHeaders()
            .AcceptLanguage?
            .OrderByDescending(x => x.Quality ?? 1)
            .Select(x => x.Value.ToString())
            .ToArray() ?? Array.Empty<string>();
    public static string? GetDefaultUserLanguage(this HttpRequest request) =>
        request.GetTypedHeaders()
            .AcceptLanguage?
            .OrderByDescending(x => x.Quality ?? 1)
            .Select(x => x.Value.ToString())
            .FirstOrDefault();
}
