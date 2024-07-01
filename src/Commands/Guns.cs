using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBuilder;

public partial class BaseBuilder
{
    [ConsoleCommand("guns")]
    public void OnGunsCommand(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null) return;

        if (isBuildTimeEnd && !isPrepTimeEnd)
        {
            MenuManager.OpenCenterHtmlMenu(this, caller, Guns());
        }
    }


    public CenterHtmlMenu Guns()
    {
        var menu = new CenterHtmlMenu("Choose Weapon", this);

        menu.AddMenuOption("AK-47", (player, option) =>
        {
            if (isPrepTimeEnd || player.TeamNum == ZOMBIE || isPrepTimeEnd || !player.PawnIsAlive) { MenuManager.CloseActiveMenu(player); return; }

            MenuManager.CloseActiveMenu(player);
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_ak47");
            player.GiveNamedItem("weapon_deagle");
        });

        menu.AddMenuOption("M4A4", (player, option) =>
        {
            if (isPrepTimeEnd || player.TeamNum == ZOMBIE || isPrepTimeEnd || !player.PawnIsAlive) { MenuManager.CloseActiveMenu(player); return; }
            MenuManager.CloseActiveMenu(player);
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_m4a1");
            player.GiveNamedItem("weapon_deagle");
        });

        menu.AddMenuOption("M4A1-S", (player, option) =>
        {
            if (isPrepTimeEnd || player.TeamNum == ZOMBIE || isPrepTimeEnd || !player.PawnIsAlive) { MenuManager.CloseActiveMenu(player); return; }
            MenuManager.CloseActiveMenu(player);
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_m4a1_silencer");
            player.GiveNamedItem("weapon_deagle");
        });

        menu.AddMenuOption("AWP", (player, option) =>
        {
            if (isPrepTimeEnd || player.TeamNum == ZOMBIE || isPrepTimeEnd || !player.PawnIsAlive) { MenuManager.CloseActiveMenu(player); return; }
            MenuManager.CloseActiveMenu(player);
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_awp");
            player.GiveNamedItem("weapon_deagle");
        });

        menu.AddMenuOption("SSG 08", (player, option) =>
        {
            if (isPrepTimeEnd || player.TeamNum == ZOMBIE || isPrepTimeEnd || !player.PawnIsAlive) { MenuManager.CloseActiveMenu(player); return; }
            MenuManager.CloseActiveMenu(player);
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_ssg08");
            player.GiveNamedItem("weapon_deagle");
        });

        return menu;
    }
}