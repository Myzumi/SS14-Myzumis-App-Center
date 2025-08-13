using System.Net.Http;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Server.ServerStatus;

namespace Content.Server.Administration;

public sealed partial class ServerApi : IPostInjectInit
{

    private const string APIVersion = "1.0.0";

    private void UpdateWebToken(string token)
    {
        _webToken = token;
    }

    private void WebAPIInit()
    {

        RegisterWebHandler(HttpMethod.Get, "/webadmin/basicinfo", BasicInfoCommand);

    }



    private async Task BasicInfoCommand(IStatusHandlerContext context)
    {
        var info = new
        {
            APIVersion,
            ServerName = CCVars.GameHostName.DefaultValue,
            Host = CCVars.ConsoleLoginHostUser.DefaultValue,
            DiscordHostId = CCVars.DiscordHostId.DefaultValue,
        };
        await context.RespondJsonAsync(info);
    }



}
