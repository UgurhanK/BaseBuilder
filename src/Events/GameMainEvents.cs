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
using CounterStrikeSharp.API.Modules.Menu;
using System.Collections.Generic;

namespace BaseBuilder;

public partial class BaseBuilder
{
    public int buildTime = 120;
    public int prepTime = 30;

    public CSTimer.Timer? timer;

    public bool isBuildTimeEnd = false;
    public bool isPrepTimeEnd = false;

    public Dictionary<CCSPlayerController, PlayerData> PlayerTypes = new Dictionary<CCSPlayerController, PlayerData>();

    public void PrintChatOnFrame()
    {
        var players = Utilities.GetPlayers().Where(p => p.CheckValid());

        foreach (var player in players)
        {
            if (!isBuildTimeEnd && !isPrepTimeEnd)
            {
                player.PrintToCenter(ReplaceString(cfg.texts.building, 0));
                continue;
            }

            if (isBuildTimeEnd && !isPrepTimeEnd)
            {
                player.PrintToCenter(ReplaceString(cfg.texts.preparing, 1));
                continue;
            }

            if (isBuildTimeEnd && isPrepTimeEnd)
            {
                player.PrintToCenter(ReplaceString(cfg.texts.released, 2));
                continue;
            }

            //switch (player.Team)
            //{
            //    case CsTeam.CounterTerrorist:


            //        break;

            //    case CsTeam.Terrorist:

            //        break;

            //    default:
            //        break;
            //}
        }
    }

    public string ReplaceString(string message, int type)
    {
        if(type == 0)
        {
            return ReplaceColorTags(message.Replace("{time}", buildTime.ToString()));
        } else if(type == 1)
        {
            return ReplaceColorTags(message.Replace("{time}", prepTime.ToString()));
        }
        else
        {
            return ReplaceColorTags(message);
        }
    }

    public string ReplaceColorTags(string input)
    {

        string[] colorPatterns =
            {
            "{DEFAULT}", "{DARKRED}", "{LIGHTPURPLE}", "{GREEN}", "{OLIVE}", "{LIME}", "{RED}", "{GREY}",
            "{YELLOW}", "{SILVER}", "{BLUE}", "{DARKBLUE}", "{ORANGE}", "{PURPLE}"
        };
        string[] colorReplacements =
        {
            "\x01", "\x02", "\x03", "\x04", "\x05", "\x06", "\x07", "\x08", "\x09", "\x0A", "\x0B", "\x0C", "\x10", "\x0E"
        };

        for (var i = 0; i < colorPatterns.Length; i++)
            input = input.Replace(colorPatterns[i], colorReplacements[i]);

        return input;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Reset();

        foreach (var p in Utilities.GetPlayers().Where(o => o != null && o.CheckValid()))
        {
            if (p.TeamNum == ZOMBIE) continue;

            p.PlayerPawn.Value!.Render = PlayerTypes[p].playerColor;
            Utilities.SetStateChanged(p.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");

            colors.Remove(PlayerTypes[p].playerColor);
        }

        //Now add timer
        timer = AddTimer(1, () =>
        {
            if (prepTime == cfg.prepTime && buildTime == cfg.buildTime) ExecutePlay("sounds/basebuilder/phase_build3.vsnd");

            if (isPrepTimeEnd && isBuildTimeEnd) return;
            
            if (buildTime > 0) buildTime--;

            switch (buildTime)
            {
                case 120:
                    ExecutePlay("sounds/basebuilder/2min-2.vsnd");
                    break;
                case 60:
                    ExecutePlay("sounds/basebuilder/1min.vsnd");
                    break;
                case 30:
                    ExecutePlay("sounds/basebuilder/30sec.vsnd");
                    break;
                case 10:
                    ExecutePlay("sounds/basebuilder/10sec.vsnd");
                    break;
                case 5:
                    ExecutePlay("sounds/basebuilder/5sec.vsnd");
                    break;
                case 0:
                    if(prepTime == cfg.prepTime)
                    {
                        ExecutePlay("sounds/basebuilder/phase_prep3.vsnd");
                    }
                    break;

            }

            if(buildTime == 0)
            {
                if(prepTime == cfg.prepTime)
                {
                    //Give Guns Menu To Players
                    foreach (var p in Utilities.GetPlayers().Where(o => o != null && o.CheckValid() && o.TeamNum == BUILDER))
                    {
                        MenuManager.OpenCenterHtmlMenu(this, p, Guns());
                    }
                }

                //Teleport Builders To Lobby And Remove Not Used Blocks
                TeleportToLobby(CsTeam.CounterTerrorist);
                if(!isPrepTimeEnd && prepTime == cfg.prepTime) RemoveNotUsedBlocks();

                isBuildTimeEnd = true;

                if(prepTime > 0) prepTime--;

                if(prepTime == 0)
                {
                    List<string> paths = new List<string>() { "sounds/basebuilder/round_start.vsnd", "sounds/basebuilder/round_start2.vsnd" };
                    ExecutePlay(paths[Random.Shared.Next(0,paths.Count)]);

                    //Release Zombies
                    TeleportToLobby(CsTeam.Terrorist);

                    isPrepTimeEnd = true;
                    timer!.Kill();
                    timer = null;
                }
            }
        }, TimerFlags.REPEAT);

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        EventPlayerDeath(@event);

        if (@event == null || !player.CheckValid()) return HookResult.Continue;

        if(player!.TeamNum == ZOMBIE && isPrepTimeEnd && isBuildTimeEnd)
        {
            //if player is zombie then teleport it to lobby
            AddTimer(1, () =>
            {
                player.RespawnClient();
                TeleportToLobby(player);
            });

            return HookResult.Continue;
        }

        if (player.TeamNum == BUILDER && (!isPrepTimeEnd || !isBuildTimeEnd))
        {
            AddTimer(1, () =>
            {
                player.RespawnClient();
            });
        }

        if (player.TeamNum == ZOMBIE && (!isPrepTimeEnd || !isBuildTimeEnd))
        {
            AddTimer(1, () =>
            {
                player.RespawnClient();
            });
        }

        return HookResult.Continue;
    }

    public void Reset()
    {
        Server.ExecuteCommand("mp_death_drop_gun 0");
        Server.ExecuteCommand("mp_respawn_on_death_t 1");
        Server.ExecuteCommand("mp_ct_default_secondary \"\"");
        Server.ExecuteCommand("mp_t_default_secondary \"\"");

        //Reset All Timers
        foreach (var timer in Timers)
        {
            if (timer != null) timer.Kill();
        }

        //Reset Teams
        foreach (var data in PlayerTypes)
        {
            data.Value.currentTeam = data.Value.defaultTeam;
        }

        //Reset Game
        isBuildTimeEnd = false;
        isPrepTimeEnd = false;

        buildTime = cfg.buildTime;
        prepTime = cfg.prepTime;

        UsedBlocks.Clear();
        PlayerHolds.Clear();

        colors = new List<Color>() { Color.AliceBlue, Color.Aqua, Color.Blue, Color.Brown, Color.BurlyWood, Color.Chocolate, Color.Cyan, Color.DarkBlue, Color.DarkGreen, Color.DarkMagenta, Color.DarkOrange, Color.DarkRed, Color.Green, Color.Yellow, Color.Red, Color.Silver, Color.Pink, Color.Purple };
    }

    public void TeleportToLobby(CsTeam Team)
    {
        if(Team == CsTeam.Terrorist && isPrepTimeEnd)
        {
            return;
        } 
        
        if(Team == CsTeam.CounterTerrorist && isBuildTimeEnd)
        {
            return;
        }

        Vector destination = Vector.Zero;

        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CInfoTeleportDestination>("info_teleport_destination"))
        {
            if(entity != null && entity.IsValid && entity.Entity != null && entity.Entity.Name != null && entity.Entity.Name.Contains("teleport_lobby"))
            {
                destination = entity.AbsOrigin!;
                break;
            }
        }

        destination.Z += 20;

        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.IsValid && p.PlayerPawn.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == Team))
        {
            player.PlayerPawn.Value!.Teleport(destination);
        }
    }

    public void TeleportToLobby(CCSPlayerController player)
    {
        Vector destination = Vector.Zero;

        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CInfoTeleportDestination>("info_teleport_destination"))
        {
            if (entity != null && entity.IsValid && entity.Entity != null && entity.Entity.Name != null && entity.Entity.Name.Contains("teleport_lobby"))
            {
                destination = entity.AbsOrigin!;
                break;
            }
        }

        player.PlayerPawn.Value!.Teleport(destination);
        
    }
}

public class PlayerData
{
    public int currentTeam = 2;
    public int defaultTeam = 2;
    public Color playerColor = Color.White;
    public Zombie playerZombie = new Zombie();

    public int balance = 0;
    
    public bool isSuperKnifeActivatedForCt = false;
    public bool isSuperKnifeActivatedForT = false;
    public int extraHpForCt = 0;
    public int extraHpForT = 0;
    public float extraGravityMultiplierForT = 1;
    public float extraGravityMultiplierForCt = 1;
    public float extraSpeedMultiplierForT = 1;
    public float extraSpeedMultiplierForCT = 1;
}