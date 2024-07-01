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
    public HookResult EventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Attacker;
        var victim = @event.Userid;

        if (player == null || victim == null || !player.CheckValid() || !victim.CheckValid() || player == victim) return HookResult.Continue;
        PlayerTypes[player].balance += cfg.Economy.OnKill;
        player.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.EarnMoneyKill).Replace("{enemy}", victim.PlayerName));

        CCSPlayerController? assister = @event.Assister;
        if (assister != null && assister.CheckValid()){ PlayerTypes[assister].balance += cfg.Economy.OnAssist; player.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.EarnMoneyAssist).Replace("{enemy}", victim.PlayerName));}

        return HookResult.Continue;
    }
}
