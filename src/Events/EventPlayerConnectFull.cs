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
        var player = @event.Userid;

        if (player == null || !player.CheckValid()) return HookResult.Continue;

        int tcount = Utilities.GetPlayers().Where(p => p.CheckValid() && p.Team == CsTeam.Terrorist).Count();
        int ctcount = Utilities.GetPlayers().Where(p => p.CheckValid() && p.Team == CsTeam.CounterTerrorist).Count();

        if (tcount < ctcount)
        {
            PlayerTypes[player] = new()
            {
                currentTeam = 2,
                defaultTeam = 2,
                playerColor = colors[new Random().Next(0, colors.Count)]
            };

            AddTimer(1, () => { player.SwitchTeam(CsTeam.Terrorist); player.CommitSuicide(false, true); });
        }else
        {
            PlayerTypes[player] = new()
            {
                currentTeam = 3,
                defaultTeam = 3,
                playerColor = colors[new Random().Next(0, colors.Count)]
            };

            AddTimer(1, () => { player.SwitchTeam(CsTeam.CounterTerrorist); player.CommitSuicide(false, true); });
        }

        return HookResult.Continue;
    }
}