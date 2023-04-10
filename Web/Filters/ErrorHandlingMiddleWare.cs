using Serilog.Events;
using Serilog;
using System.Net;
using Core.Exceptions;

namespace Web.Filters;

public class ErrorHandlingMiddleWare
{
    private readonly RequestDelegate _next;
    private static readonly Serilog.ILogger _logger = (Serilog.ILogger)new LoggerConfiguration()
.Enrich.FromLogContext()
.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
//.MinimumLevel.Debug()
.WriteTo.Console()
.CreateLogger();

    public ErrorHandlingMiddleWare(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status = HttpStatusCode.OK;
        string message = string.Empty;
        var stackTrace = string.Empty;
        var exceptionType = exception.GetType();

        _logger.Information("Error Message Start.");

        if (exceptionType == typeof(BadRequestException))
            status = HttpStatusCode.BadRequest;
        else if (exceptionType == typeof(ForbiddenException))
            status = HttpStatusCode.Forbidden;
        else if (exceptionType == typeof(UnprocessableException))
            status = HttpStatusCode.UnprocessableEntity;
        else if (exceptionType == typeof(Core.Exceptions.KeyNotFoundException))
            status = HttpStatusCode.Unauthorized;
        else if (exceptionType == typeof(Core.Exceptions.NotImplementedException))
            status = HttpStatusCode.NotImplemented;
        else if (exceptionType == typeof(Core.Exceptions.UnauthorizedAccessException))
            status = HttpStatusCode.Unauthorized;
        else if (exceptionType == typeof(InternalServerException))
            status = HttpStatusCode.InternalServerError;
        else
            status = HttpStatusCode.InternalServerError;

        message = exception.Message;
        stackTrace = exception.StackTrace;
        _logger.Error($"Error Code::{(int)status} with message:: {message}");
        _logger.Error($"Error Details:: {stackTrace}");
        _logger.Information("Error Message End.");

        var exceptionResult = System.Text.Json.JsonSerializer.Serialize(new
        {
            details = message
            //, stackTrace
        });


        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(exceptionResult);

    }


}
