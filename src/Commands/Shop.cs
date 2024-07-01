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
    [ConsoleCommand("shop")]
    public void OnShopCommand(CCSPlayerController? caller, CommandInfo info)
    {
        if (caller == null) return;

        MenuManager.OpenCenterHtmlMenu(this, caller, ShopMenu(caller));
    }
}