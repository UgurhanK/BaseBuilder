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
    public static void Health(this CCSPlayerController player, int health)
    {
        if (player.PlayerPawn == null || player.PlayerPawn.Value == null)
        {
            return;
        }

        player.Health = health;
        player.PlayerPawn.Value.Health = health;

        if (health > 100)
        {
            player.MaxHealth = health;
            player.PlayerPawn.Value.MaxHealth = health;
        }

        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
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

    public static MemoryFunctionVoid<IntPtr, IntPtr, IntPtr, IntPtr> _SetParent = new("\\x4D\\x8B\\xD9\\x48\\x85\\xD2\\x74\\x2A");
    public static void SetParent(this CBaseEntity? entity, CBaseEntity? target)
    {
        if (entity == null || !entity.IsValid)
            throw new ArgumentNullException("Entity is null");

        if (target != null && target.IsValid)
            _SetParent.Invoke(entity!.Handle, target!.Handle, 0, 0);
        else
            _SetParent.Invoke(entity!.Handle, 0, 0, 0);
    }
}