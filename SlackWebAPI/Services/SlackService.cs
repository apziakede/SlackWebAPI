using Newtonsoft.Json;
using System.Text;

namespace SlackWebAPI.Services
{
    public class SlackService
    {
        private readonly string _botToken;
        private readonly string _webhookUrl;
        private readonly string _channel;
        private readonly HttpClient _httpClient;

        public SlackService(IConfiguration configuration, HttpClient httpClient)
        {
            _botToken = configuration["Slack:BotToken"];
            _httpClient = httpClient;
            _webhookUrl = configuration["Slack:WebhookUrl"];
            _channel = configuration["Slack:Channel"];
        }

        public async Task SendMessageAsync(string message)
        {
            var requestUri = _webhookUrl;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Headers =
                {
                    Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _botToken)
                },
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    _channel,
                    text = message
                }), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
