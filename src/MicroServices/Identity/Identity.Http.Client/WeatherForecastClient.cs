namespace Identity.Http.Client;

public class WeatherForecastClient
{
    private readonly HttpClient _client;
    private readonly string _scheme = "WeatherForecast";

    public WeatherForecastClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IEnumerable<WeatherForecast>?> Gets()
    {
        return await _client.GetFromJsonAsync<IEnumerable<WeatherForecast>>($"{_scheme}");
    }
}