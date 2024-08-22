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
             // Create a memory stream to capture the response body
  var originalBodyStream = context.Response.Body;
  using var newBodyStream = new MemoryStream();
  context.Response.Body = newBodyStream;
  try
  { 
      await _next(context);
      //if(context.Response.StatusCode==StatusCodes.Status401Unauthorized)
      //{
      //    await _slackService.SendMessageAsync($"[Warning {DateTime.UtcNow.AddHours(1):MM/dd/yy HH:mm:ss}] {context.Response.StatusCode}: Unauthorized request made to {context.Request.Path}");
      //}

      // Copy the contents of the new body stream to a string
      newBodyStream.Seek(0, SeekOrigin.Begin);
      var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

      // Log the response body and status code
      if (context.Response.StatusCode >= 400)
      {
          await _slackService.SendMessageAsync($"Response Status Code: {context.Response.StatusCode}, Response Body: {responseBody}");
      }

      // Reset the stream position to the beginning and copy it back to the original stream
      newBodyStream.Seek(0, SeekOrigin.Begin);
      await newBodyStream.CopyToAsync(originalBodyStream);

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
