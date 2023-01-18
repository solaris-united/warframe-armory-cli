using System.Collections.Generic;
using Newtonsoft.Json;

namespace SolarisUnited.Warframe.Armory.DataModels
{
    public class RawMissions
    {
        public string Name;
        public List<RawRotation> Rotations = new List<RawRotation>();
    }

    public class RawRelics
    {
        public string Name;
        public List<RawReward> Rewards = new List<RawReward>();
    }

    public class RawBountyRewards
    {
        public string Name;
        public List<RawBountyRotation> Rotations = new List<RawBountyRotation>();
    }

    public class RawDropsBySource
    {
        public string Source;
        public string Chance;
        public List<RawReward> Rewards = new List<RawReward>();
    }

    public class RawDropsByItem
    {
        public string Name;
        public List<RawDropSource> Sources = new List<RawDropSource>();
    }

    public class RawRotation
    {
        public string Name;
        public List<RawReward> Rewards = new List<RawReward>();
    }

    public class RawBountyRotation
    {
        public string Name;
        public List<RawBountyStage> Stages = new List<RawBountyStage>();
    }

    public class RawBountyStage
    {
        public string Description;
        public List<RawReward> Rewards = new List<RawReward>();
    }

    public class RawReward
    {
        public string Description;
        public string Chance;
    }

    public class RawDropSource
    {
        public string Source;
        public string DropChance;
        public string Chance;
    }

    public class WarframeDrops
    {
        public DropsLastUpdated SourceData;
        public List<RawMissions> Missions = new List<RawMissions>();
        public List<RawRelics> Relics = new List<RawRelics>();
        public List<RawMissions> Keys = new List<RawMissions>();
        public List<RawMissions> DynamicLocationRewards = new List<RawMissions>();
        public List<RawMissions> Sorties = new List<RawMissions>();
        public List<RawBountyRewards> CetusBountyRewards = new List<RawBountyRewards>();
        public List<RawBountyRewards> OrbVallisBountyRewards = new List<RawBountyRewards>();
        public List<RawBountyRewards> CambionDriftBountyRewards = new List<RawBountyRewards>();
        public List<RawBountyRewards> ZarimanBountyRewards = new List<RawBountyRewards>();
        public List<RawDropsBySource> ModDropsBySource = new List<RawDropsBySource>();
        public List<RawDropsByItem> ModDropsByMod = new List<RawDropsByItem>();
        public List<RawDropsBySource> PartDropsBySource = new List<RawDropsBySource>();
        public List<RawDropsByItem> PartDropsByItems = new List<RawDropsByItem>();
        public List<RawDropsBySource> ResourceDropsBySource = new List<RawDropsBySource>();
        public List<RawDropsByItem> ResourceDropsByResource = new List<RawDropsByItem>();
        public List<RawDropsBySource> SigilDropsBySource = new List<RawDropsBySource>();
        public List<RawDropsBySource> AdditionalItemDropsBySource = new List<RawDropsBySource>();
        public List<RawDropsBySource> RelicDropsBySource = new List<RawDropsBySource>();
    }

    public class DropsLastUpdated
    {
        [JsonProperty("id")]
        public string Id;
        public string SourceUrl;
        public string LastUpdated;
        public string LastRun;
    }
}