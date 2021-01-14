using System.Threading.Tasks;
using EduJournal.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EduJournal.Presentation.Web
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnexpectedDataException e)
            {
                _logger.LogError(e,"Some database error occurred.");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "plain/text";
                const string message = "An error occurred while performing an operation on the database.\n" +
                                       "We hope this will not happen again, but we do not promise.";
                await context.Response.WriteAsync(message);
            }
        }
    }
}