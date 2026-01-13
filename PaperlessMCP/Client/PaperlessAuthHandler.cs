using Microsoft.Extensions.Options;
using PaperlessMCP.Configuration;

namespace PaperlessMCP.Client;

/// <summary>
/// HTTP message handler that adds the Paperless-ngx API token to requests.
/// </summary>
public class PaperlessAuthHandler : DelegatingHandler
{
    private readonly PaperlessOptions _options;

    public PaperlessAuthHandler(IOptions<PaperlessOptions> options)
    {
        _options = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_options.ApiToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _options.ApiToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
