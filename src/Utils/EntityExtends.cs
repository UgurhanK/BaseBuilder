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
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using System.Runtime.InteropServices;

namespace BaseBuilder;

public static class EntityExtends
{
    public static void SetHp(this CCSPlayerController controller, int health = 100)
    {
        if (health <= 0 || !controller.PawnIsAlive || controller.PlayerPawn.Value == null || !controller.IsValid) return;

        controller.Health = health;
        controller.PlayerPawn.Value.Health = health;

        if (health > 100)
        {
            controller.MaxHealth = health;
            controller.PlayerPawn.Value.MaxHealth = health;
        }

        Server.NextFrame(() => Utilities.SetStateChanged(controller.PlayerPawn.Value, "CBaseEntity", "m_iHealth"));
    }

    public static bool CheckValid(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid || !player.PlayerPawn.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV) return false;

        return true;
    }

    public static void RespawnClient(this CCSPlayerController client)
    {
        if (!client.IsValid || client.PawnIsAlive)
            return;

        var clientPawn = client.PlayerPawn.Value;

        MemoryFunctionVoid<CCSPlayerController, CCSPlayerPawn, bool, bool> CBasePlayerController_SetPawnFunc = new(GameData.GetSignature("CBasePlayerController_SetPawn"));
        CBasePlayerController_SetPawnFunc.Invoke(client, clientPawn!, true, false);
        VirtualFunction.CreateVoid<CCSPlayerController>(client.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(client);
    }

    internal static bool IsPlayer(this CCSPlayerController? player)
    {
        return player is { IsValid: true, IsHLTV: false, IsBot: false, UserId: not null, SteamID: > 0 };
    }
}