using E_Menu.Caching.Interfaces;
using E_Menu.Engine;
using E_Menu.Engine.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Shared.Kernel;
using Shared.Kernel.Interfaces;
using System.Text.Json;

namespace E_Menu.API.Abstraction;

[Route("api/[controller]/[action]")]
[EnableRateLimiting("Default")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IOrganizationServiceAsync _service;
    private readonly IMediator _mediator;
    private readonly ICacheService _cacheService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    // Önbellek için varsayılan süre - yapılandırılabilir olması için
    private readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromHours(1);

    public ApiController(IOrganizationServiceAsync service, IMediator mediator, ICacheService cacheService)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ExecuteCustomApiAsync(ICustomApiRequest request, bool? cache = null, TimeSpan? expiration = null)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        // Try to get from cache first if caching is enabled
        var cacheKey = GenerateCacheKey(request);
        if (TryGetFromCache(cacheKey, cache, out IActionResult cachedResult))
            return cachedResult;

        // Find query handler type
        var queryType = FindQueryHandlerType(request.GetType());
        if (queryType == null)
            return BadRequest($"No query handler found for request type '{request.GetType().Name}'.");

        // Execute request
        var result = await ExecuteOrganizationRequestAsync(request, queryType);
        if (result == null)
            return BadRequest("Failed to get response from the custom API.");

        // Get status code
        var statusCode = GetStatusCode(result);

        // Cache the result if caching is enabled
        await CacheResultIfEnabledAsync(cacheKey, result, cache, expiration);

        return StatusCode(statusCode, result);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ExecuteInternalApiAsync<TResponse>(IRequest<TResponse> request, bool? cache = null, TimeSpan? expiration = null)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        // Try to get from cache first if caching is enabled
        var cacheKey = GenerateCacheKey(request);
        if (TryGetFromCache(cacheKey, cache, out IActionResult cachedResult))
            return cachedResult;

        // Process internal API request
        var result = await _mediator.Send(request);
        if (result == null)
            return BadRequest("No data returned from the internal API.");

        // Get status code
        var statusCode = GetStatusCode(result);

        // Cache the result if caching is enabled
        await CacheResultIfEnabledAsync(cacheKey, result, cache, expiration);

        return StatusCode(statusCode, result);
    }

    #region Helper Methods

    /// <summary>
    /// Finds the query handler type for the given request type
    /// </summary>
    private Type FindQueryHandlerType(Type requestType)
    {
        var assembly = typeof(EngineExtensions).Assembly;

        return assembly
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(IPlugin).IsAssignableFrom(t) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICustomApiCommand<,>) &&
                    i.GetGenericArguments()[0] == requestType
                )
            )!;
    }

    /// <summary>
    /// Gets the response type from the query handler
    /// </summary>
    private Type GetResponseType(Type queryType, Type requestType)
    {
        var queryInterface = queryType.GetInterfaces()
            .First(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(ICustomApiCommand<,>) &&
                      i.GetGenericArguments()[0] == requestType);

        return queryInterface.GetGenericArguments()[1];
    }

    /// <summary>
    /// Executes the organization request and returns the result
    /// </summary>
    private async Task<object> ExecuteOrganizationRequestAsync(ICustomApiRequest request, Type queryType)
    {
        var apiName = $"{CustomApiConstants.Prefix}{queryType.Name}";

        var requestType = request.GetType();
        var organizationRequest = new OrganizationRequest
        {
            RequestName = apiName,
            Parameters = new ParameterCollection { { CustomApiConstants.Input, JsonSerializer.Serialize(request, requestType, _jsonSerializerOptions) } },
            RequestId = Guid.NewGuid()
        };

        var response = await _service.ExecuteAsync(organizationRequest);
        var json = response.Results[CustomApiConstants.Output]?.ToString();

        if (string.IsNullOrEmpty(json))
            return null;

        // Get response type
        var responseType = GetResponseType(queryType, request.GetType());
        var resultType = typeof(Result<>).MakeGenericType(responseType);

        return JsonSerializer.Deserialize(json, resultType, _jsonSerializerOptions)!;
    }

    /// <summary>
    /// Gets the status code from the result object
    /// </summary>
    private int GetStatusCode(object result)
    {
        if (result == null) return 500;

        var resultType = result.GetType();
        if (!resultType.IsGenericType || resultType.GetGenericTypeDefinition() != typeof(Result<>))
            return 200;

        var statusCodeProperty = resultType.GetProperty(nameof(Result<object>.StatusCode));
        return (int)(statusCodeProperty?.GetValue(result) ?? 200);
    }

    /// <summary>
    /// Generates a cache key for the request
    /// </summary>
    private string GenerateCacheKey(object request)
    {
        var requestJson = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var requestHash = requestJson.GetHashCode();
        return $"{request.GetType().Name}_{requestHash}";
    }

    /// <summary>
    /// Tries to get the result from cache
    /// </summary>
    private bool TryGetFromCache(string cacheKey, bool? cacheEnabled, out IActionResult result)
    {
        result = null;

        if (!cacheEnabled.GetValueOrDefault(false))
            return false;

        try
        {
            var cachedJsonString = _cacheService.GetAsync<string>(cacheKey).Result;
            if (string.IsNullOrWhiteSpace(cachedJsonString))
                return false;

            // Deserialize the cached result
            var cachedResult = JsonSerializer.Deserialize<Result<dynamic>>(cachedJsonString, _jsonSerializerOptions);
            if (cachedResult == null)
                return false;

            result = StatusCode(cachedResult.StatusCode, cachedResult);
            return true;
        }
        catch
        {
            // Önbellek hatalarını yutuyoruz, normal akışa devam ediyoruz
            return false;
        }
    }

    /// <summary>
    /// Caches the result if caching is enabled
    /// </summary>
    private async Task CacheResultIfEnabledAsync(string cacheKey, object result, bool? cacheEnabled, TimeSpan? expiration = null)
    {
        if (!cacheEnabled.GetValueOrDefault(false) || result == null)
            return;

        try
        {
            var resultType = result.GetType();
            var isSuccess = false;

            // Result<T> tipinde mi kontrol et
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var isSuccessProperty = resultType.GetProperty(nameof(Result<object>.IsSuccess));
                isSuccess = (bool)(isSuccessProperty?.GetValue(result) ?? false);
            }

            if (isSuccess)
            {
                var resultJson = JsonSerializer.Serialize(result, _jsonSerializerOptions);
                var cacheExpiration = expiration ?? _defaultCacheExpiration;
                await _cacheService.SetAsync(cacheKey, resultJson, cacheExpiration);
            }
        }
        catch
        {
            // Önbellek hatalarını yutuyoruz, API yanıtını etkilemesin
        }
    }

    #endregion
}