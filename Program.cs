using System;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SolarisUnited.Warframe.Armory.DataModels;
using SolarisUnited.Warframe.Armory.Helpers;
using SolarisUnited.Warframe.Armory.Logic;

namespace SolarisUnited.Warframe.Armory.CLI
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly DataHelpers dataHelpers = new DataHelpers();
        private static readonly string sourceUrl = "https://www.warframe.com/repos/hnfvc0o3jnfvc873njb03enrf56.html";
        private static readonly string filePath = "content/output/warframe.pc.drops.json";

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Starting Warframe PC Drops retrieval at: {0}", DateTime.Now.ToString("O"));
            Console.WriteLine("Retrieving from source URL: {0}", sourceUrl);
            // Retrieve the HTML content and convert it to an HtmlDocument for further processing
            string html;
            HtmlDocument doc;
            RawDataScrapers.RetrieveSourceContent(sourceUrl, client, out html, out doc);

            // Get the hash of the HTML document to determine the unique document seed
            string hash = RawDataScrapers.GetHtmlHash(html);

            // Done with the raw HTML content, dispose of it
            html = string.Empty;

            // Generate the HTML source content metadata
            DropsLastUpdated lastUpdateResponse = RawDataScrapers.DetermineSourceLastUpdate(sourceUrl, hash, doc);

            Console.WriteLine();
            Console.WriteLine("Warframe PC Drops current publish date: {0}", lastUpdateResponse.LastUpdated);
            Console.WriteLine("With hash: {0}", hash);

            // Initialize the object to store the data in for response/output
            WarframeDrops warframeDrops = new WarframeDrops
            {
                SourceData = lastUpdateResponse
            };

            // Get all tables in the HTML document
            HtmlNodeCollection tables = doc.DocumentNode.SelectNodes("//table");

            // Main document is no longer needed
            doc = null;

            // Iterate through each table
            foreach (HtmlNode table in tables)
            {
                RawDataScrapers.DetermineTableAction(warframeDrops, dataHelpers, table);
            }

            // Serialize the Warframe Drops data to a JSON string, indented to make comparisons for differences easier
            string jsonString = JsonConvert.SerializeObject(warframeDrops, Formatting.Indented);

            Console.WriteLine();
            Console.WriteLine("Saving file to: {0}", filePath);
            RawDataScrapers.SaveOutput(filePath, jsonString);

            Console.WriteLine();
            Console.WriteLine("Finished Warframe PC Drops retrieval at: {0}", DateTime.Now.ToString("O"));
            Console.WriteLine();
        }
    }
}