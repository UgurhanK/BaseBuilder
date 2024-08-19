using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.File;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using System.Runtime.InteropServices;
using CSTimer = CounterStrikeSharp.API.Modules.Timers;
using System.Data;
using CounterStrikeSharp.API.Core.Translations;

namespace BaseBuilder;

public partial class BaseBuilder
{
    [GameEventHandler]
    public HookResult EventPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        if (isEnabled == false) return HookResult.Continue;

        CCSPlayerController? player = @event.Attacker;
        CCSPlayerController? victim = @event.Userid;

        if (player == null || victim == null || !player.CheckValid() || !victim.CheckValid()) return HookResult.Continue;

        if (player.TeamNum == BUILDER && PlayerDatas[player].isSuperKnifeActivatedForCt)
        {
            Server.NextFrame(() => victim.SetHp(0));
        }
        else if (player.TeamNum == ZOMBIE && PlayerDatas[player].isSuperKnifeActivatedForT)
        {
            Server.NextFrame(() => victim.SetHp(0));
        }

        return HookResult.Continue;
    }
}