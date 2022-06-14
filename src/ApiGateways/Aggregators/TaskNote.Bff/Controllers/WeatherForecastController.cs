using Microsoft.AspNetCore.Mvc;

namespace TaskNote.Bff.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastClient _client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {
            return await _client.Gets();
        }
    }
}