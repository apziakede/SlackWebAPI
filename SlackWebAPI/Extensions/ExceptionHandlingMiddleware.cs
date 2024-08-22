using SlackWebAPI.Services;

namespace SlackWebAPI.Extensions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SlackService _slackService;

        public ExceptionHandlingMiddleware(RequestDelegate next, SlackService slackHelper)
        {
            _next = next;
            _slackService = slackHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    await _slackService.SendMessageAsync($"{ex.Source}: {ex} \n\n[Error {DateTime.UtcNow.AddHours(1):MM/dd/yy HH:mm:ss}] {context.Response.StatusCode}: {ex.Message}");
                }
                await Task.CompletedTask;
            }
        }
    }
}
