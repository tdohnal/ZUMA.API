using Microsoft.IO;
using System.Text;

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

        var originalBodyStream = context.Response.Body;
        using var responseBody = _recyclableMemoryStreamManager.GetStream();
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

        var request = context.Request;
        var body = await ReadStreamAsync(request.Body);
        request.Body.Position = 0;

        var fullUrl = GetFullUrl(context);

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

        _logger.LogInformation(
            "_________________________________________________________________ HTTP REQUEST _________________________________________________________________\n" +
            "{Method} {FullUrl}\n" +
            "Path: {Path} | Query: {QueryString}\n" +
            "Host: {Host} | IP: {RemoteIpAddress}\n" +
            "Content-Type: {ContentType} | Content-Length: {ContentLength}\n" +
            "Body: {Body}\n" +
            "_________________________________________________________________________________________________________________________________________________________",
            logData.Method,
            logData.FullUrl,
            logData.Path,
            logData.QueryString,
            logData.Host,
            logData.RemoteIpAddress,
            logData.ContentType,
            logData.ContentLength,
            logData.Body);
    }

    private async Task LogResponseAsync(HttpContext context)
    {
        var response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);

        var body = await ReadStreamAsync(response.Body);
        response.Body.Seek(0, SeekOrigin.Begin);

        // ✅ Vytvoř úplnou URL
        var fullUrl = GetFullUrl(context);

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

        var logLevel = response.StatusCode switch
        {
            >= 200 and < 300 => LogLevel.Information,
            >= 300 and < 400 => LogLevel.Information,
            >= 400 and < 500 => LogLevel.Warning,
            _ => LogLevel.Error
        };

        _logger.Log(
            logLevel,
            "_________________________________________________________________ HTTP RESPONSE _________________________________________________________________\n" +
            "{Method} {FullUrl}\n" +
            "Status: {StatusCode} | Content-Type: {ContentType}\n" +
            "Body: {Body}\n" +
            "_________________________________________________________________________________________________________________________________________________________",
            logData.Method,
            logData.FullUrl,
            logData.StatusCode,
            logData.ContentType,
            logData.Body);
    }

    /// <summary>
    /// Vytvoří úplnou URL adresu (scheme + host + path + query)
    /// </summary>
    private static string GetFullUrl(HttpContext context)
    {
        var request = context.Request;
        var scheme = request.Scheme; // http nebo https
        var host = request.Host.ToString(); // localhost:5044
        var pathBase = request.PathBase.Value; // prázdné nebo /api
        var path = request.Path.Value; // /user/1
        var queryString = request.QueryString.Value; // ?page=1

        return $"{scheme}://{host}{pathBase}{path}{queryString}";
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
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
            // Pokud je body příliš dlouhý, stav zkrácený
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