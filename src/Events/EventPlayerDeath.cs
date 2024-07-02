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
    public void EventPlayerDeath(EventPlayerDeath @event)
    {
        if (@event == null) return;

        CheckPlayersBlocks(@event);
        ChangeToZombie(@event);

        var player = @event.Attacker;
        var victim = @event.Userid;

        if (player == null || victim == null || !player.CheckValid() || !victim.CheckValid() || player == victim) return;
        PlayerTypes[player].balance += cfg.Economy.OnKill;
        player.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.EarnMoneyKill).Replace("{enemy}", victim.PlayerName).Replace("{credit}", cfg.Economy.OnKill.ToString()));

        CCSPlayerController? assister = @event.Assister;
        if (assister != null && assister.CheckValid()){ PlayerTypes[assister].balance += cfg.Economy.OnAssist; player.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.EarnMoneyAssist).Replace("{enemy}", victim.PlayerName).Replace("{credit}", cfg.Economy.OnAssist.ToString()));}

        return;
    }

    public void CheckPlayersBlocks(EventPlayerDeath @event)
    {
        var player = @event.Userid;

        if (player == null || !player.CheckValid()) return;

        List<PropData> ownedBlocks = UsedBlocks.Values.Where(x => x.owner == player).ToList();

        foreach(var data in ownedBlocks)
        {
            if (data.myProp != null && data.myProp.IsValid) data.myProp.Remove();
        }
    }
    public void ChangeToZombie(EventPlayerDeath @event)
    {
        var player = @event.Userid;

        if (player == null || !player.CheckValid() || player.TeamNum == BUILDER) return;

        PlayerTypes[player].currentTeam = 2;
        player.SwitchTeam(CsTeam.Terrorist);
    }
}