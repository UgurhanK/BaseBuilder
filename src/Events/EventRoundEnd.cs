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
        //reset color
        foreach (var prop in Utilities.FindAllEntitiesByDesignerName<CBaseProp>("prop_dynamic"))
        {
            prop.Render = Color.White;
            Utilities.SetStateChanged(prop, "CBaseModelEntity", "m_clrRender");
        }
        
        //Swap Teams
        foreach (var data in PlayerTypes)
        {
            if(data.Value.defaultTeam == 2)
            {
                data.Value.defaultTeam = 3;
                data.Value.currentTeam = 3;

                data.Key.SwitchTeam(CsTeam.CounterTerrorist);
                data.Key.CommitSuicide(false, true);
                continue;
            }

            if (data.Value.defaultTeam == 3)
            {
                data.Value.defaultTeam = 2;
                data.Value.currentTeam = 2;

                data.Key.SwitchTeam(CsTeam.Terrorist);
                data.Key.CommitSuicide(false, true);
                continue;
            }
        }

        return HookResult.Continue;
    }
}