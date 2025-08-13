


using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<string> WebApiToken =
    CVarDef.Create("adminweb.apitoken", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> DiscordHostId =
    CVarDef.Create("adminweb.discordhostid", string.Empty, CVar.SERVERONLY);

}
