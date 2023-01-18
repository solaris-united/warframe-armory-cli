using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SolarisUnited.Warframe.Armory.DataModels;
using SolarisUnited.Warframe.Armory.Helpers;

namespace SolarisUnited.Warframe.Armory.Logic
{
    public static class RawDataScrapers
    {
        public static void RetrieveSourceContent(string url, HttpClient client, out string html, out HtmlDocument doc)
        {
            // Get the HTML content
            html = GetHtmlContent(url, client).Result;

            // Convert the HTML string to a HTML Document for ease of parsing
            doc = new HtmlDocument();
            doc.LoadHtml(html);
        }

        private static async Task<string> GetHtmlContent(string url, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public static string GetHtmlHash(string html)
        {
            // Get a unique hash from the HTML document to identify the 'seed' of the document version
            string hash = "";
            // Create a new instance of the SHA256CryptoServiceProvider object
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the HTML
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(html));

                // Convert the hash to a string
                hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }

            return hash;
        }

        public static DropsLastUpdated DetermineSourceLastUpdate(string url, string hash, HtmlDocument doc)
        {
            // Select the paragraphs as the Last Update is stored in one of the three nodes of this type
            var paragraphs = doc.DocumentNode.SelectNodes("//p");

            // Find the last updated value
            string lastUpdate;
            if (paragraphs != null)
            {
                var paragraph = paragraphs.FirstOrDefault(p => p.InnerText.Contains("Last Update:"));
                if (paragraph != null)
                {
                    // Strip away the 'Last Update:' block from the paragraph and store only the raw date
                    lastUpdate = paragraph.InnerText.Split("\r\n")[1];
                }
                else
                {
                    // If the paragraph was empty, set the date to unknown
                    lastUpdate = "1 March, 2013";
                }
            }
            else
            {
                // If there are no paragraphs, set the date to unknown
                lastUpdate = "1 March, 2013";
            }

            var lastUpdateResponse = new DropsLastUpdated
            {
                Id = hash,
                SourceUrl = url,
                LastUpdated = DateOnly.Parse(lastUpdate).ToString("yyyy-MM-dd"),
                LastRun = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd")
            };
            return lastUpdateResponse;
        }

        public static void SaveOutput(string filePath, string jsonString)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            // Write the JSON string to the file at the previous path
            File.WriteAllText(filePath, jsonString);
        }

        public static void DetermineTableAction(WarframeDrops warframeDrops, DataHelpers helpers, HtmlNode table)
        {
            // Get the header name to determine which type of data is being retrieved
            string header = table.PreviousSibling.PreviousSibling.InnerHtml;

            // Switch based on the header to set the correct property in the WarframeDrops object
            switch (header)
            {
                case "Missions:":
                    // Get all rows in the table
                    var missionRows = table.SelectNodes("tr");

                    // Store the missions in this object
                    List<RawMissions> missions;
                    ExtractRawMissions(helpers, missionRows, out missions);

                    warframeDrops.Missions = missions;

                    break;

                case "Relics:":
                    // Get all rows in the table
                    var relicRows = table.SelectNodes("tr");

                    // Store the relics in this object
                    List<RawRelics> relics;
                    ExtractRawRelics(helpers, relicRows, out relics);

                    warframeDrops.Relics = relics;

                    break;

                case "Keys:":
                    // Get all rows in the table
                    var keyRows = table.SelectNodes("tr");

                    // Store the keys in this object
                    List<RawMissions> keys;
                    ExtractRawMissions(helpers, keyRows, out keys);

                    warframeDrops.Keys = keys;

                    break;

                case "Dynamic Location Rewards:":
                    // Get all rows in the table
                    var dynamicRows = table.SelectNodes("tr");

                    // Store the dynamic location rewards in this object
                    List<RawMissions> dynamicRewards;
                    ExtractRawMissions(helpers, dynamicRows, out dynamicRewards);

                    warframeDrops.DynamicLocationRewards = dynamicRewards;

                    break;

                case "Sorties:":
                    // Get all rows in the table
                    var sortieRows = table.SelectNodes("tr");

                    // Store the sorties in this object
                    List<RawMissions> sorties;
                    ExtractRawMissions(helpers, sortieRows, out sorties);

                    warframeDrops.Sorties = sorties;

                    break;

                case "Cetus Bounty Rewards:":
                    // Get all rows in the table
                    var cetusRows = table.SelectNodes("tr");

                    // Store the bounties in this object
                    List<RawBountyRewards> cetusBounties;
                    ExtractRawBounties(helpers, cetusRows, out cetusBounties);

                    warframeDrops.CetusBountyRewards = cetusBounties;

                    break;

                case "Orb Vallis Bounty Rewards:":
                    // Get all rows in the table
                    var orbVallisRows = table.SelectNodes("tr");

                    // Store the bounties in this object
                    List<RawBountyRewards> orbVallisBounties;
                    ExtractRawBounties(helpers, orbVallisRows, out orbVallisBounties);

                    warframeDrops.OrbVallisBountyRewards = orbVallisBounties;

                    break;

                case "Cambion Drift Bounty Rewards:":
                    // Get all rows in the table
                    var cambionDriftRows = table.SelectNodes("tr");

                    // Store the bounties in this object
                    List<RawBountyRewards> cambionDriftBounties;
                    ExtractRawBounties(helpers, cambionDriftRows, out cambionDriftBounties);

                    warframeDrops.CambionDriftBountyRewards = cambionDriftBounties;

                    break;

                case "Zariman Bounty Rewards:":
                    // Get all rows in the table
                    var zarimanRows = table.SelectNodes("tr");

                    // Store the bounties in this object
                    List<RawBountyRewards> zarimanBounties;
                    ExtractRawBounties(helpers, zarimanRows, out zarimanBounties);

                    warframeDrops.ZarimanBountyRewards = zarimanBounties;

                    break;

                case "Mod Drops by Source:":
                    // Get all rows in the table
                    var modBySourceRows = table.SelectNodes("tr");

                    // Store the mods in this object
                    List<RawDropsBySource> modsBySource;
                    ExtractDropsBySource(helpers, modBySourceRows, out modsBySource);

                    warframeDrops.ModDropsBySource = modsBySource;

                    break;

                case "Mod Drops by Mod:":
                    // Get all rows in the table
                    var modByModRows = table.SelectNodes("tr");

                    // Store the mods in this object
                    List<RawDropsByItem> modByMod;
                    ExtractDropsByItem(helpers, modByModRows, out modByMod);

                    warframeDrops.ModDropsByMod = modByMod;

                    break;

                case "Blueprint/Item Drops by Source:":
                    // Get all rows in the table
                    var partsBySourceRows = table.SelectNodes("tr");

                    // Store the parts in this object
                    List<RawDropsBySource> partsBySource;
                    ExtractDropsBySource(helpers, partsBySourceRows, out partsBySource);

                    warframeDrops.PartDropsBySource = partsBySource;

                    break;

                case "Blueprint/Item Drops by Blueprint/Item:":
                    // Get all rows in the table
                    var partsByItemRows = table.SelectNodes("tr");

                    // Store the mods in this object
                    List<RawDropsByItem> partsByItem;
                    ExtractDropsByItem(helpers, partsByItemRows, out partsByItem);

                    warframeDrops.PartDropsByItems = partsByItem;

                    break;

                case "Resource Drops by Source:":
                    // Get all rows in the table
                    var resourceBySourceRows = table.SelectNodes("tr");

                    // Store the resources in this object
                    List<RawDropsBySource> resourceBySource;
                    ExtractDropsBySource(helpers, resourceBySourceRows, out resourceBySource);

                    warframeDrops.ResourceDropsBySource = resourceBySource;

                    break;

                case "Resource Drops by Resource:":
                    // Get all rows in the table
                    var resourceByResourceRows = table.SelectNodes("tr");

                    // Store the resources in this object
                    List<RawDropsByItem> resourceByResource;
                    ExtractDropsByItem(helpers, resourceByResourceRows, out resourceByResource);

                    warframeDrops.ResourceDropsByResource = resourceByResource;

                    break;

                case "Sigil Drops by Source:":
                    // Get all rows in the table
                    var sigilsBySourceRows = table.SelectNodes("tr");

                    // Store the sigils in this object
                    List<RawDropsBySource> sigilsBySource;
                    ExtractDropsBySource(helpers, sigilsBySourceRows, out sigilsBySource);

                    warframeDrops.SigilDropsBySource = sigilsBySource;

                    break;

                case "Additional Item Drops by Source:":
                    // Get all rows in the table
                    var additionalItemsBySourceRows = table.SelectNodes("tr");

                    // Store the items in this object
                    List<RawDropsBySource> additionalItemsBySource;
                    ExtractDropsBySource(helpers, additionalItemsBySourceRows, out additionalItemsBySource);

                    warframeDrops.AdditionalItemDropsBySource = additionalItemsBySource;

                    break;

                case "Relic Drops by Source:":
                    // Get all rows in the table
                    var relicsBySourceRows = table.SelectNodes("tr");

                    // Store the relics in this object
                    List<RawDropsBySource> relicsBySource;
                    ExtractDropsBySource(helpers, relicsBySourceRows, out relicsBySource);

                    warframeDrops.RelicDropsBySource = relicsBySource;

                    break;

                default:
                    break;
            }
        }

        private static void ExtractRawMissions(DataHelpers helpers, HtmlNodeCollection missionRows, out List<RawMissions> rawMissions)
        {
            rawMissions = new List<RawMissions>();

            // Iterate through each row
            foreach (var row in missionRows)
            {
                // Get all cells in the row
                var cells = row.SelectNodes("td|th");

                foreach (var cell in cells)
                {
                    bool isEmptyRow = (cell.HasAttributes && cell.Attributes["class"]?.Value == "blank-row");
                    if (!isEmptyRow)
                    {

                        string text = cell.InnerText;

                        if (cell.Name.Equals("th"))
                        {
                            if (helpers.TryParseMission(text) || helpers.TryParseOddMission(text))
                            {
                                rawMissions.Add(new RawMissions
                                {
                                    Name = text
                                });
                            }

                            if (helpers.TryParseRotation(text))
                            {
                                rawMissions.Last().Rotations.Add(new RawRotation
                                {
                                    Name = text
                                });
                            }
                        }
                        if (cell.Name.Equals("td"))
                        {
                            if (helpers.TryParseReward(text))
                            {
                                if (!rawMissions.Last().Rotations.Any())
                                {
                                    rawMissions.Last().Rotations.Add(new RawRotation
                                    {
                                        Name = "Rotation A"
                                    });
                                }
                                rawMissions.Last().Rotations.Last().Rewards.Add(new RawReward
                                {
                                    Description = cell.PreviousSibling.InnerText,
                                    Chance = text
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void ExtractRawRelics(DataHelpers helpers, HtmlNodeCollection relicRows, out List<RawRelics> rawRelics)
        {
            rawRelics = new List<RawRelics>();

            // Iterate through each row
            foreach (var row in relicRows)
            {
                // Get all cells in the row
                var cells = row.SelectNodes("td|th");

                foreach (var cell in cells)
                {
                    bool isEmptyRow = (cell.HasAttributes && cell.Attributes["class"]?.Value == "blank-row");
                    if (!isEmptyRow)
                    {

                        string text = cell.InnerText;

                        if (cell.Name.Equals("th"))
                        {
                            if (helpers.TryParseRelic(text))
                            {
                                rawRelics.Add(new RawRelics
                                {
                                    Name = text
                                });
                            }
                        }
                        if (cell.Name.Equals("td"))
                        {
                            if (helpers.TryParseReward(text))
                            {
                                rawRelics.Last().Rewards.Add(new RawReward
                                {
                                    Description = cell.PreviousSibling.InnerText,
                                    Chance = text
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void ExtractRawBounties(DataHelpers helpers, HtmlNodeCollection bountyRows, out List<RawBountyRewards> rawBounties)
        {
            rawBounties = new List<RawBountyRewards>();

            // Iterate through each row
            foreach (var row in bountyRows)
            {
                // Get all cells in the row
                var cells = row.SelectNodes("td|th");

                foreach (var cell in cells)
                {
                    bool isEmptyRow = (cell.HasAttributes && cell.Attributes["class"]?.Value == "blank-row");
                    if (!isEmptyRow)
                    {

                        string text = cell.InnerText;

                        if (cell.Name.Equals("th"))
                        {
                            if (helpers.TryParseBounty(text))
                            {
                                rawBounties.Add(new RawBountyRewards
                                {
                                    Name = text
                                });
                            }

                            if (helpers.TryParseRotation(text))
                            {
                                rawBounties.Last().Rotations.Add(new RawBountyRotation
                                {
                                    Name = text
                                });
                            }

                            if (helpers.TryParseStage(text))
                            {
                                if (!rawBounties.Last().Rotations.Any())
                                {
                                    rawBounties.Last().Rotations.Add(new RawBountyRotation
                                    {
                                        Name = "Rotation A"
                                    });
                                }
                                rawBounties.Last().Rotations.Last().Stages.Add(new RawBountyStage
                                {
                                    Description = text
                                });
                            }
                        }
                        if (cell.Name.Equals("td"))
                        {
                            if (helpers.TryParseReward(text))
                            {
                                if (!rawBounties.Last().Rotations.Any())
                                {
                                    rawBounties.Last().Rotations.Add(new RawBountyRotation
                                    {
                                        Name = "Rotation A"
                                    });
                                }
                                if (!rawBounties.Last().Rotations.Last().Stages.Any())
                                {
                                    rawBounties.Last().Rotations.Last().Stages.Add(new RawBountyStage
                                    {
                                        Description = "Stage 1"
                                    });
                                }
                                rawBounties.Last().Rotations.Last().Stages.Last().Rewards.Add(new RawReward
                                {
                                    Description = cell.PreviousSibling.InnerText,
                                    Chance = text
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void ExtractDropsBySource(DataHelpers helpers, HtmlNodeCollection dropsBySourceRows, out List<RawDropsBySource> rawDropsBySource)
        {
            rawDropsBySource = new List<RawDropsBySource>();

            // Iterate through each row
            foreach (var row in dropsBySourceRows)
            {
                // Get all cells in the row
                var cells = row.SelectNodes("td|th");

                foreach (var cell in cells)
                {
                    bool isEmptyRow = (cell.HasAttributes && cell.Attributes["class"]?.Value == "blank-row");
                    if (!isEmptyRow)
                    {

                        string text = cell.InnerText;

                        if (cell.Name.Equals("th"))
                        {
                            if (helpers.TryParseSource(text))
                            {
                                rawDropsBySource.Add(new RawDropsBySource
                                {
                                    Source = text
                                });
                            }
                            if (helpers.TryParseChance(text))
                            {
                                rawDropsBySource.Last().Chance = text;
                            }
                        }
                        if (cell.Name.Equals("td"))
                        {
                            if (helpers.TryParseReward(text))
                            {
                                rawDropsBySource.Last().Rewards.Add(new RawReward
                                {
                                    Description = cell.PreviousSibling.InnerText,
                                    Chance = text
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void ExtractDropsByItem(DataHelpers helpers, HtmlNodeCollection dropsByItemRows, out List<RawDropsByItem> rawDropsByItem)
        {
            rawDropsByItem = new List<RawDropsByItem>();

            // Iterate through each row
            foreach (var row in dropsByItemRows)
            {
                // Get all cells in the row
                var cells = row.SelectNodes("td|th");

                foreach (var cell in cells)
                {
                    bool isEmptyRow = (cell.HasAttributes && cell.Attributes["class"]?.Value == "blank-row");
                    if (!isEmptyRow)
                    {

                        string text = cell.InnerText;

                        if (cell.Name.Equals("th"))
                        {
                            if (helpers.TryParseItemByItem(text))
                            {
                                rawDropsByItem.Add(new RawDropsByItem
                                {
                                    Name = text
                                });
                            }
                        }
                        if (cell.Name.Equals("td"))
                        {
                            if (helpers.TryParseDropSourceName(text))
                            {
                                rawDropsByItem.Last().Sources.Add(new RawDropSource
                                {
                                    Source = text
                                });
                            }
                            if (helpers.TryParseDropSourceDropChance(text))
                            {
                                rawDropsByItem.Last().Sources.Last().DropChance = text;
                            }
                            if (helpers.TryParseDropSourceChance(text))
                            {
                                rawDropsByItem.Last().Sources.Last().Chance = text;
                            }
                        }
                    }
                }
            }
        }
    }
}