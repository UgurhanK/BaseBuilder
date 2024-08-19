using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseBuilder;

public partial class BaseBuilder
{
    [ConsoleCommand("class"), ConsoleCommand("zombie"), ConsoleCommand("zombi")]
    public void OnClassCommand(CCSPlayerController? caller, CommandInfo info)
    {
        if (isEnabled == false) return;
        if (caller == null) return;

        if(caller.TeamNum == ZOMBIE)
        {
            MenuManager.OpenCenterHtmlMenu(this, caller, Class());
        }
    }

    public CenterHtmlMenu Class()
    {
        var menu = new CenterHtmlMenu("Choose Class", this);

        foreach (var @class in classes)
        {
            menu.AddMenuOption(@class.Key, (player, option) =>
            {
                if (player.TeamNum != ZOMBIE) { MenuManager.CloseActiveMenu(player); return; }

                PlayerDatas[player].playerZombie = @class.Value;

                MenuManager.CloseActiveMenu(player);
            });
        }

        return menu;
    }
}