using Microsoft.IO;
using System.Text;
using ZUMA.SharedKernel.Application.Utils;

namespace ZUMA.API.Middleware;

/// <summary>
/// Middleware pro logování HTTP requestů a responsí.
/// Zaznamenává HTTP metodu, URL, headers, body, status kód, atd.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await LogRequestAsync(context);

        Stream originalBodyStream = context.Response.Body;
        using RecyclableMemoryStream responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            await LogResponseAsync(context);

            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpContext context)
    {
        // Povolí opětovné čtení request body
        context.Request.EnableBuffering();

        HttpRequest request = context.Request;
        string body = await ReadStreamAsync(request.Body);
        request.Body.Position = 0;

        string fullUrl = GetFullUrl(context);

        var logData = new
        {
            Timestamp = DateTime.UtcNow,
            Method = request.Method,
            FullUrl = fullUrl,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Headers = GetHeadersAsObject(request.Headers),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            Body = TrySafelyParseJson(body),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            Host = request.Host.Host
        };

        using IDisposable? scope = _logger.BeginMessageScope(context.TraceIdentifier, identificationData: logData);
        _logger.LogInformation($"{logData.Method} HTTP Request - {logData.Path}");
    }

    private async Task LogResponseAsync(HttpContext context)
    {
        HttpResponse response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);

        string body = await ReadStreamAsync(response.Body);
        response.Body.Seek(0, SeekOrigin.Begin);

        string fullUrl = GetFullUrl(context);

        var logData = new
        {
            Timestamp = DateTime.UtcNow,
            Method = context.Request.Method,
            FullUrl = fullUrl,
            Path = context.Request.Path.Value,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            Headers = GetHeadersAsObject(response.Headers),
            Body = TrySafelyParseJson(body),
        };

        LogLevel logLevel = response.StatusCode switch
        {
            >= 200 and < 300 => LogLevel.Information,
            >= 300 and < 400 => LogLevel.Information,
            >= 400 and < 500 => LogLevel.Warning,
            _ => LogLevel.Error
        };

        using IDisposable? scope = _logger.BeginMessageScope(context.TraceIdentifier, identificationData: logData);

        _logger.LogInformation($"{logData.Method} HTTP Response ({response.StatusCode}) - {logData.Path}");
    }


    private static string GetFullUrl(HttpContext context)
    {
        HttpRequest request = context.Request;
        string scheme = request.Scheme;
        string host = request.Host.ToString();
        string? pathBase = request.PathBase.Value;
        string? path = request.Path.Value;
        string? queryString = request.QueryString.Value;

        return $"{scheme}://{host}{pathBase}{path}{queryString}";
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using StreamReader reader = new(stream, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private static object GetHeadersAsObject(IHeaderDictionary headers)
    {
        return new
        {
            Accept = headers["Accept"].ToString(),
            ContentType = headers["Content-Type"].ToString(),
            Authorization = !string.IsNullOrEmpty(headers["Authorization"].ToString()) ? "[REDACTED]" : string.Empty,
            UserAgent = headers["User-Agent"].ToString(),
            Host = headers["Host"].ToString(),
            CacheControl = headers["Cache-Control"].ToString(),
        };
    }

    private static object? TrySafelyParseJson(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            if (body.Length > 1000)
                return body.Substring(0, 1000) + "... [truncated]";

            return body;
        }
        catch
        {
            return "[Unable to parse body]";
        }
    }
}