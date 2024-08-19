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
    [GameEventHandler(HookMode.Post)]
    public HookResult EventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        if (isEnabled == false) return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.CheckValid()) return HookResult.Continue;

        AddTimer(1, () =>
        {
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
        });

        if (player.TeamNum == ZOMBIE)
        {
            Server.NextFrame(() =>
            {
                player.SetHp(PlayerDatas[player].playerZombie.Health + PlayerDatas[player].extraHpForT);
                player.PlayerPawn.Value!.Speed = PlayerDatas[player].playerZombie.SpeedMultiplier * PlayerDatas[player].extraSpeedMultiplierForT;
                player.PlayerPawn.Value!.GravityScale = PlayerDatas[player].playerZombie.GravityMultiplier * PlayerDatas[player].extraGravityMultiplierForT;
                player.PlayerPawn.Value!.SetModel(PlayerDatas[player].playerZombie.ModelPath);
            });
        } else
        {
            Server.NextFrame(() =>
            {
                player.SetHp(100 + PlayerDatas[player].extraHpForCt);
                player.PlayerPawn.Value!.Speed = 1;
                player.PlayerPawn.Value!.GravityScale = 1;
            });
        }

        return HookResult.Continue;
    }
}