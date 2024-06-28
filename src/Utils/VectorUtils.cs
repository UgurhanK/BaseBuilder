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
using System.Runtime.InteropServices;

namespace BaseBuilder;

public static class VectorUtils
{
    private static bool IsPointOnLine(Vector start, Vector end, Vector point, double threshold)
    {
        // Calculate the direction vector of the line
        double lineDirectionX = end.X - start.X;
        double lineDirectionY = end.Y - start.Y;
        double lineDirectionZ = end.Z - start.Z;

        // Calculate the vector from start to the point
        double pointVectorX = point.X - start.X;
        double pointVectorY = point.Y - start.Y;
        double pointVectorZ = point.Z - start.Z;

        // Calculate the scalar projection of pointVector onto the lineDirection
        double scalarProjection = (pointVectorX * lineDirectionX + pointVectorY * lineDirectionY + pointVectorZ * lineDirectionZ) /
                                 (lineDirectionX * lineDirectionX + lineDirectionY * lineDirectionY + lineDirectionZ * lineDirectionZ);

        // Check if the scalar projection is within [0, 1], meaning the point is between start and end
        if (scalarProjection >= 0 && scalarProjection <= 1)
        {
            // Calculate the closest point on the line to the given point
            double closestPointX = start.X + scalarProjection * lineDirectionX;
            double closestPointY = start.Y + scalarProjection * lineDirectionY;
            double closestPointZ = start.Z + scalarProjection * lineDirectionZ;

            // Calculate the distance between the given point and the closest point on the line
            double distance = Math.Sqrt(Math.Pow(point.X - closestPointX, 2) + Math.Pow(point.Y - closestPointY, 2) + Math.Pow(point.Z - closestPointZ, 2));

            // Check if the distance is within the specified threshold
            return distance <= threshold;
        }

        // Point is not between start and end
        return false;
    }

    public static CBaseProp? GetClientAimTarget(this CCSPlayerController player)
    {
        var GameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()?.GameRules;

        if (GameRules is null)
            return null;

        VirtualFunctionWithReturn<IntPtr, IntPtr, IntPtr> findPickerEntity = new(GameRules.Handle, 27);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) findPickerEntity = new(GameRules.Handle, 28);

        var target = new CBaseProp(findPickerEntity.Invoke(GameRules.Handle, player.Handle));

        if (target != null && target.IsValid && target.Entity != null && target.DesignerName.Contains("prop_dynamic")) return target;

        return null;
    }

    public static CBaseProp? GetClosestProp(Vector start, Vector end, double threshold)
    {
        CBaseProp? ent = null;

        foreach (var prop in Utilities.GetAllEntities().Where(e => e.DesignerName.Contains("prop")))
        {
            //bool isOnLine = IsPointOnLine(start, end, player.PlayerPawn?.Value.CBodyComponent?.SceneNode?.AbsOrigin);
            var pos = prop.As<CBaseProp>().CBodyComponent!.SceneNode!.AbsOrigin!;
            pos = new Vector(pos.X, pos.Y, pos.Z + 30);
            bool isOnLine = IsPointOnLine(start, end, pos, threshold);

            if (isOnLine)
            {
                ent = prop.As<CBaseProp>();
                break;
            }
        }

        return ent;
    }

    public static float CalculateDistance(Vector a, Vector b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
    }

    public static Vector GetEndXYZ(CCSPlayerController player, double distance = 250)
    {
        double karakterX = (float)player.PlayerPawn.Value!.AbsOrigin!.X;
        double karakterY = (float)player.PlayerPawn.Value.AbsOrigin.Y;
        double karakterZ = (float)player.PlayerPawn.Value.AbsOrigin.Z + player.PlayerPawn.Value!.CameraServices!.OldPlayerViewOffsetZ;

        // Açý deðerleri
        double angleA = -player.PlayerPawn.Value.EyeAngles.X;   // (-90, 90) arasýnda
        double angleB = player.PlayerPawn.Value.EyeAngles.Y; // (-180, 180) arasýnda

        // Açýlarý dereceden radyana çevir
        double radianA = (Math.PI / 180) * angleA;
        double radianB = (Math.PI / 180) * angleB;

        // Açýlara göre XYZ koordinatlarýný hesapla
        double x = karakterX + distance * Math.Cos(radianA) * Math.Cos(radianB);
        double y = karakterY + distance * Math.Cos(radianA) * Math.Sin(radianB);
        double z = karakterZ + distance * Math.Sin(radianA);

        return new Vector((float)x, (float)y, (float)z);
    }

    public static Vector AnglesToDirection(Vector angles)
    {
        float pitch = -angles.X * (float)(Math.PI / 180.0);
        float yaw = angles.Y * (float)(Math.PI / 180.0);

        float x = (float)(Math.Cos(pitch) * Math.Cos(yaw));
        float y = (float)(Math.Cos(pitch) * Math.Sin(yaw));
        float z = (float)Math.Sin(pitch);

        return new Vector(x, y, z);
    }

    public static Vector AddInFrontOf(Vector origin, QAngle angles, float units)
    {
        Vector direction = AnglesToDirection(new Vector(angles.X, angles.Y, angles.Z));
        return origin + (direction * units);
    }
}