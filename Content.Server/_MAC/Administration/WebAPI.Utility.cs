


using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Robust.Server.ServerStatus;

namespace Content.Server.Administration;
public sealed partial class ServerApi
{
    private const string WebTokenScheme = "SS14WebToken";
    private string _webToken = string.Empty;
    private void RegisterWebHandler(HttpMethod method, string exactPath, Func<IStatusHandlerContext, Task> handler)
    {
        _statusHost.AddHandler(async context =>
        {
            if (context.RequestMethod != method || context.Url.AbsolutePath != exactPath)
                return false;

            if (!await CheckWebAccess(context))
                return true;

            await handler(context);
            return true;
        });
    }

    private async Task<bool> CheckWebAccess(IStatusHandlerContext context)
    {
        var auth = context.RequestHeaders.TryGetValue("Authorization", out var authToken);
        if (!auth)
        {
            await RespondError(
                context,
                ErrorCode.AuthenticationNeeded,
                HttpStatusCode.Unauthorized,
                "Authorization is required");
            return false;
        }

        var authHeaderValue = authToken.ToString();
        var spaceIndex = authHeaderValue.IndexOf(' ');
        if (spaceIndex == -1)
        {
            await RespondBadRequest(context, "Invalid Authorization header value");
            return false;
        }

        var authScheme = authHeaderValue[..spaceIndex];
        var authValue = authHeaderValue[spaceIndex..].Trim();

        if (authScheme != WebTokenScheme)
        {
            await RespondBadRequest(context, "Invalid Authorization scheme");
            return false;
        }

        if (_webToken == "")
        {
            _sawmill.Debug("No Web API token set for the Web API");
            await RespondError(
                context,
                ErrorCode.AuthenticationNeeded,
                HttpStatusCode.Unauthorized,
                "Web API token is not set in the configuration");
        }
        else if (CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(authValue),
                Encoding.UTF8.GetBytes(_webToken)))
        {
            return true;
        }

        await RespondError(
            context,
            ErrorCode.AuthenticationInvalid,
            HttpStatusCode.Unauthorized,
            "Authorization is invalid");

        // Invalid auth header, no access
        _sawmill.Info($"Unauthorized access attempt to Web API from {context.RemoteEndPoint}");
        return false;
    }

}
