using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnHttpClient.CustomHttp
{
    public class CustomHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add custom headers to the request
            request.Headers.Add("X-Custom-Header", "CustomValue");
            // Call the base class implementation to send the request
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
