using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBuilder;

public partial class BaseBuilder
{
    [ConsoleCommand("revive"), ConsoleCommand("rev")]
    public void OnReviveCommand(CCSPlayerController? caller, CommandInfo info)
    {
        if (isEnabled == false) return;
        if (caller == null) return;

        if (isPrepTimeEnd && isBuildTimeEnd) return;

        if(caller.TeamNum == BUILDER && (!isBuildTimeEnd || !isPrepTimeEnd))
        {
            caller.RespawnClient();
        }

        if(caller.TeamNum == ZOMBIE && isPrepTimeEnd)
        {
            caller.RespawnClient();
            TeleportToLobby(caller);
        }
    }
}