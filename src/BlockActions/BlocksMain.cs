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
using System.Data;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Reflection;
using System.Diagnostics.Tracing;

namespace BaseBuilder;

public partial class BaseBuilder
{
    public Dictionary<CBaseProp, PropData> UsedBlocks = new Dictionary<CBaseProp, PropData>();
    public Dictionary<CCSPlayerController, Builder> PlayerHolds = new Dictionary<CCSPlayerController, Builder>();
    public Dictionary<uint, CCSPlayerController> BlocksOwner = new Dictionary<uint, CCSPlayerController>();
    public List<Color> colors = new List<Color>() { Color.AliceBlue, Color.Aqua, Color.Blue, Color.Brown, Color.BurlyWood, Color.Chocolate, Color.Cyan, Color.DarkBlue, Color.DarkGreen, Color.DarkMagenta, Color.DarkOrange, Color.DarkRed, Color.Green, Color.Yellow, Color.Red, Color.Silver, Color.Pink, Color.Purple };

    public void OnGameFrame()
    {
        PrintChatOnFrame();

        //Disable block actions in prep time
        if (isBuildTimeEnd) return;

        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.PawnIsAlive))
        {
            //Continue if player is zombie
            if (player.TeamNum == ZOMBIE) continue;

            if (player.Buttons.HasFlag(PlayerButtons.Reload))
            {
                if (PlayerHolds.ContainsKey(player) && !PlayerHolds[player].isRotating)
                {
                    PlayerHolds[player].emptyProp.Teleport(null, new QAngle(PlayerHolds[player].emptyProp.AbsRotation!.X, PlayerHolds[player].emptyProp.AbsRotation!.Y + 45, PlayerHolds[player].emptyProp.AbsRotation!.Z));
                    PlayerHolds[player].isRotating = true;
                }
            } else
            {
                if (PlayerHolds.ContainsKey(player))
                {
                    PlayerHolds[player].isRotating = false;
                }
            }

            if (player.Buttons.HasFlag(PlayerButtons.Use))
            {
                if (PlayerHolds.ContainsKey(player))
                {
                    PressRepeat(player, PlayerHolds[player].emptyProp!);
                } 
                else
                {
                    var block = player.GetClientAimTarget();
                    if (block != null) 
                    {
                        if (UsedBlocks.ContainsKey(block) && UsedBlocks[block].owner != player) return;

                        FirstPress(player, block); 
                    }
                }
            } else
            {
                if (PlayerHolds.ContainsKey(player))
                {
                    var newprop = Utilities.CreateEntityByName<CPhysicsProp>("prop_dynamic");
                    if (newprop != null && newprop.IsValid)
                    {
                        newprop.Teleport(new Vector(-10, -10, -10));
                        CBaseEntity_SetParent(PlayerHolds[player].mainProp, newprop);
                        PlayerHolds[player].emptyProp.Remove();
                        PlayerHolds.Remove(player);
                    }
                }
            }
        }
    }

    public void FirstPress(CCSPlayerController player, CBaseProp prop)
    {
        var hitPoint = TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);
        
        if (prop != null && prop.IsValid && hitPoint != null && hitPoint.HasValue)
        {
            //Change block color to player color
            prop.Render = PlayerTypes[player].playerColor;
            Utilities.SetStateChanged(prop, "CBaseModelEntity", "m_clrRender");

            //fixed some bugs
            if (VectorUtils.CalculateDistance(prop.AbsOrigin!, Vector3toVector(hitPoint.Value)) > 150) return;

            var emptyProp = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
            if(emptyProp != null && emptyProp.IsValid)
            {
                emptyProp.Render = Color.Transparent;
                emptyProp.DispatchSpawn();
                emptyProp.Teleport(Vector3toVector(hitPoint.Value));
                if(emptyProp.Entity != null) emptyProp.Entity.Name = "parent_prop";

                //Distance
                int distance = (int)VectorUtils.CalculateDistance(emptyProp.AbsOrigin!, player.PlayerPawn.Value!.AbsOrigin!);

                PlayerHolds.Add(player, new Builder() { mainProp = prop, emptyProp = emptyProp, owner = player, isRotating = false, distance = distance});
            }
        }
    }

    public void PressRepeat(CCSPlayerController player, CDynamicProp block)
    {
        //To Remove Block On Prep Time
        if (!UsedBlocks.ContainsKey(PlayerHolds[player].mainProp)) UsedBlocks.Add(PlayerHolds[player].mainProp, new PropData(player, PlayerHolds[player].mainProp));
        block.Teleport(VectorUtils.GetEndXYZ(player, PlayerHolds[player].distance), null, player.PlayerPawn.Value!.AbsVelocity!);

        //Checking ATTACK2 & ATTACK buttons for distance.
        if (player.Buttons.HasFlag(PlayerButtons.Attack))
        {
            if (PlayerHolds[player].distance > 350) PlayerHolds[player].distance += 7;
            PlayerHolds[player].distance += 3;
        }
        else if (player.Buttons.HasFlag(PlayerButtons.Attack2) && PlayerHolds[player].distance > 3)
        {
            if (PlayerHolds[player].distance > 350) PlayerHolds[player].distance -= 7;
            PlayerHolds[player].distance -= 3;
        }

        CBaseEntity_SetParent(PlayerHolds[player].mainProp, PlayerHolds[player].emptyProp);
    }

    public void RemoveNotUsedBlocks()
    {
        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseProp>("prop_dynamic"))
        {
            if (entity != null && entity.IsValid && entity.Entity != null && entity.Entity.Name != null && entity.Entity.Name == "parent_prop") continue;

            //Checking if removing parent empty prop
            if (!UsedBlocks.ContainsKey(entity) && entity.AbsOrigin!.Z > -9) entity.Remove();
        }
    }

    //Win sig
    private static MemoryFunctionVoid<CBaseEntity, CBaseEntity, CUtlStringToken?, matrix3x4_t?> CBaseEntity_SetParentFunc
        = new("\\x4D\\x8B\\xD9\\x48\\x85\\xD2\\x74\\x2A");

    public static void CBaseEntity_SetParent(CBaseEntity childrenEntity, CBaseEntity parentEntity)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CBaseEntity_SetParentFunc = new MemoryFunctionVoid<CBaseEntity, CBaseEntity, CUtlStringToken?, matrix3x4_t?>("\\x48\\x85\\xF6\\x74\\x2A\\x48\\x8B\\x47\\x10\\xF6\\x40\\x31\\x02\\x75\\x2A\\x48\\x8B\\x46\\x10\\xF6\\x40\\x31\\x02\\x75\\x2A\\xB8\\x2A\\x2A\\x2A\\x2A");
        }

        if (!childrenEntity.IsValid || !parentEntity.IsValid) return;

        var origin = new Vector(childrenEntity.AbsOrigin!.X, childrenEntity.AbsOrigin!.Y, childrenEntity.AbsOrigin!.Z);
        CBaseEntity_SetParentFunc.Invoke(childrenEntity, parentEntity, null, null);
        // If not teleported, the childrenEntity will not follow the parentEntity correctly.
        childrenEntity.Teleport(origin, new QAngle(IntPtr.Zero), new Vector(IntPtr.Zero));
    }
}

public class Builder
{
    public CDynamicProp emptyProp = null!;
    public CBaseProp mainProp = null!;
    public Vector offset = null!;
    public CCSPlayerController owner = null!;
    public bool isRotating;
    public int distance;
}

public class PropData
{
    public PropData(CCSPlayerController player, CBaseProp prop)
    {
        owner = player;
        myProp = prop;
    }

    //public bool isLocked = false;
    public CCSPlayerController owner;
    public CBaseProp myProp;
}