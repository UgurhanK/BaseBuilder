using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BaseBuilder;

public class Config : BasePluginConfig
{
    [JsonProperty] public string[] PluginStartIn { get; set; } = { "bb_", "basebuilder"};
    [JsonProperty] public int buildTime { get; set; } = 120;
    [JsonProperty] public int prepTime { get; set; } = 30;
    [JsonProperty] public int RoundTime { get; set; } = 600;
    [JsonProperty] public Dictionary<string, Zombie> zombies { get; set; } = new Dictionary<string, Zombie>() { { "Classic Zombie", new Zombie() { Name = "Classic" } }, { "Tanker Zombie", new Zombie() { Health = 5000, Name = "Tanker" } } };
    [JsonProperty] public Strings texts { get; set; } = new Strings();
    [JsonProperty] public Economy Economy { get; set; } = new Economy();
}

public class Strings
{
    [JsonProperty] public string building { get; set; } = "Remaining Build Time : {time}";
    [JsonProperty] public string preparing { get; set; } = "Remaining Prepare Time : {time}";
    [JsonProperty] public string released { get; set; } = "Zombies Are Released!";
    [JsonProperty] public string Prefix { get; set; } = "[{RED}BASEBUILDER{DEFAULT}] ";
    [JsonProperty] public string NotEnoughMoney { get; set; } = "You Dont Have Enough Money";
    [JsonProperty] public string EarnMoneyKill { get; set; } = "You earned {credit} credit by killing {enemy}.";
    [JsonProperty] public string EarnMoneyAssist { get; set; } = "You earned {credit} credit by assisting {enemy}.";
    [JsonProperty] public string PurchaseSuccesful { get; set; } = "The purchase is successful. Balance: {credit}";
    [JsonProperty] public string Balance { get; set; } = "Balance: {credit}";
}

public class Zombie
{
    [JsonProperty] public string Name { get; set; } = "Classic Zombie";
    [JsonProperty] public float GravityMultiplier { get; set; } = 1.0f;
    [JsonProperty] public float SpeedMultiplier { get; set; } = 1;
    [JsonProperty] public int Health { get; set; } = 2000;
    [JsonProperty] public string ModelPath { get; set; } = "characters/x.vmdl";
    [JsonProperty] public string ModelArmPath { get; set; } = "characters/x_arms.vmdl";
}

public class Economy
{
    [JsonProperty] public int OnKill { get; set; } = 3;
    [JsonProperty] public int OnAssist { get; set; } = 1;
}