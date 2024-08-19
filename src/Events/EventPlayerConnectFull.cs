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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseBuilder;

public partial class BaseBuilder
{
    [GameEventHandler]
    public HookResult EventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;

        if (player == null || !player.CheckValid() || player.IsBot) return HookResult.Continue;

        int tcount = Utilities.GetPlayers().Where(p => p.CheckValid() && p.Team == CsTeam.Terrorist).Count();
        int ctcount = Utilities.GetPlayers().Where(p => p.CheckValid() && p.Team == CsTeam.CounterTerrorist).Count();

        if(!PlayerDatas.TryGetValue(player, out var data))
        {
            PlayerDatas[player] = new PlayerData(colors[Random.Shared.Next(0, colors.Count)], classes.Values.First());
        }

        if (isEnabled == false) return HookResult.Continue;

        /*AddTimer(5, () =>
        {
            if (tcount < ctcount)
            {
                player.SwitchTeam(CsTeam.Terrorist);
                player.RespawnClient();
                player.SwitchTeam(CsTeam.CounterTerrorist);
                player.SwitchTeam(CsTeam.Terrorist);
                player.PendingTeamNum = 2;
                player.TeamChanged = false;
            }
            else
            {
                player.SwitchTeam(CsTeam.CounterTerrorist);
                PlayerDatas[player].wasBuilderThisRound = true;
            }
        });
        */
        return HookResult.Continue;
    }
}