using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

namespace BaseBuilder;

public partial class BaseBuilder
{
    public CenterHtmlMenu ShopMenu(CCSPlayerController player)
    {
        var menu = new CenterHtmlMenu("Choose Team", this);

        menu.AddMenuOption("Zombie", (controller, option) =>
        {
            var menu = new CenterHtmlMenu("Choose One", this);

            menu.AddMenuOption("Gravity x2 | 10 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 10)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].extraGravityMultiplierForT /= 2;
                AddToBalance(controller, -10);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            menu.AddMenuOption("Speed x2 | 10 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 10)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].extraSpeedMultiplierForT *= 2;
                AddToBalance(controller, -10);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            menu.AddMenuOption("Extra 2000 HP | 15 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 15)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].extraHpForT += 2000;
                AddToBalance(controller, -15);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            menu.AddMenuOption("Super Knife | 80 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 80)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].isSuperKnifeActivatedForT = true;
                controller.PrintToCenterAlert("Super Knife Activated For 1 Round");
                AddToBalance(controller, -80);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            MenuManager.OpenCenterHtmlMenu(this, controller, menu);
        });

        menu.AddMenuOption("Builder", (controller, option) =>
        {
            var menu = new CenterHtmlMenu("Choose One", this);

            menu.AddMenuOption("Extra 100 HP | 35 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 35)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].extraHpForCt += 100;
                AddToBalance(controller, -35);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            menu.AddMenuOption("Super Knife | 100 Credit", (controller, option) =>
            {
                if (!CheckBalance(controller, 100)) { controller.PrintToChat(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.NotEnoughMoney)); MenuManager.CloseActiveMenu(player); return; }
                PlayerDatas[controller].isSuperKnifeActivatedForCt = true;
                controller.PrintToCenterAlert("Super Knife Activated For 1 Round");
                AddToBalance(controller, -100);
                controller.PrintToCenter(ReplaceColorTags(cfg.texts.Prefix + cfg.texts.PurchaseSuccesful).Replace("{credit}", PlayerDatas[player].balance.ToString()));
            });

            MenuManager.OpenCenterHtmlMenu(this, controller, menu);
        });

        return menu;
    }

    public int GetPlayerBalance(CCSPlayerController player)
    {
        return PlayerDatas[player].balance;
    }

    public int AddToBalance(CCSPlayerController player, int credit)
    {
        return PlayerDatas[player].balance += credit;
    }

    public bool CheckBalance(CCSPlayerController player, int credit)
    {
        return PlayerDatas[player].balance > credit;
    }

    public int GetBalance(CCSPlayerController player)
    {
        return PlayerDatas[player].balance;
    }
}