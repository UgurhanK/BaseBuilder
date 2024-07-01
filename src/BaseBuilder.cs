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
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using System.Runtime.CompilerServices;

namespace BaseBuilder;

public partial class BaseBuilder : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "BaseBuilder";
    public override string ModuleVersion => "2.1.0";
    public override string ModuleAuthor => "UgurhanK & BoinK";
    public override string ModuleDescription => "Basically BaseBuilder";

    public Config Config { get; set; } = null!;
    public static Config cfg = null!;

    public Dictionary<string, Zombie> classes = new Dictionary<string, Zombie>();

    public const int ZOMBIE = 2;
    public const int BUILDER = 3;
    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnGameFrame);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnPrecache);

        Server.PrintToConsole("BASEBUILDER LOADED by UgurhanK");

        base.Load(hotReload);
    }

    public void OnConfigParsed(Config config)
    {
        Config = config;
        cfg = config;

        classes = config.zombies;
    }
}