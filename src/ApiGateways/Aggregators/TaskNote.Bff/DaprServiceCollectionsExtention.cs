namespace TaskNote.Bff;

using Dapr.Client;
using Identity.Http.Client;
using Microsoft.Extensions.DependencyInjection;

public static class DaprServiceCollectionExtentions
{
    public static IServiceCollection AddTaskNoteHttpClient(this IServiceCollection services) =>
        services
        .AddSingleton<WeatherForecastClient>(_ => new WeatherForecastClient(CreateHttpClient("IdentityApiId")))
        ;

    private static HttpClient CreateHttpClient(string environmentName)
    {
        var uri = Environment.GetEnvironmentVariable(environmentName) ?? throw new Exception($"環境変数 \"{environmentName}\" が見つかりません");
        var ret = DaprClient.CreateInvokeHttpClient(uri);
        return ret;
    }
}
