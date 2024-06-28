using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseBuilder;

public class Config : BasePluginConfig
{
    [JsonPropertyName("BuildTime")] public int buildTime { get; set; } = 120;
    [JsonPropertyName("PrepTime")] public int prepTime { get; set; } = 30;
    [JsonPropertyName("Texts")] public TimeStrings texts { get; set; } = new TimeStrings();
}

public class TimeStrings
{
    [JsonPropertyName("OnBuilding")] public string building { get; set; } = "Remaining Build Time : {time}";
    [JsonPropertyName("OnPreparing")] public string preparing { get; set; } = "Remaining Prepare Time : {time}";
    [JsonPropertyName("OnZombiesRelased")] public string released { get; set; } = "Zombies Are Released!";
}