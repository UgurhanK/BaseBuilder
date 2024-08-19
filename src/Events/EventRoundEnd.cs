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
    public HookResult EventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (isEnabled == false) return HookResult.Continue;

        //reset color
        foreach (var prop in Utilities.FindAllEntitiesByDesignerName<CBaseProp>("prop_dynamic"))
        {
            prop.Render = Color.White;
            Utilities.SetStateChanged(prop, "CBaseModelEntity", "m_clrRender");
        }

        //TeamSWAP
        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.IsValid && p.PlayerPawn.IsValid && p.Connected == PlayerConnectedState.PlayerConnected))
        {
            if (!PlayerDatas.TryGetValue(player, out var data)) continue;

            if (data.wasBuilderThisRound)
            {
                player.SwitchTeam(CsTeam.Terrorist);
            }
            else
            {
                player.SwitchTeam(CsTeam.CounterTerrorist);
            }

            player.CommitSuicide(false, true);

            data.wasBuilderThisRound = false;
        }

        return HookResult.Continue;
    }
}