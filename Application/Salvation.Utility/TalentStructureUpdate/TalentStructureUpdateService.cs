using Microsoft.Extensions.Logging;
using Salvation.Core.Profile.Model;
using Salvation.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Salvation.Utility.TalentStructureUpdate
{
    public class TalentStructureUpdateService : ITalentStructureUpdateService
    {
        private readonly string remoteTalentDataUrl = "https://www.raidbots.com/static/data/beta/new-talent-trees.json";
        private readonly string localRawTalentDataFile = "new-talent-trees.json";
        /// <summary>
        /// This URL was from https://develop.battle.net/documentation/world-of-warcraft/game-data-apis
        /// If it expires you may need to get another one, not
        /// happening enough to warrant implementing oauth & secret handling here to call it from code.
        /// Icon filetype is .jpg
        /// </summary>
        private readonly string battletNetIconUrlBase = "https://render.worldofwarcraft.com/us/icons/56/";
        private readonly string staticDataRelativePath = "../../../../Salvation.Client/wwwroot/static-data/";
        private readonly string iconFolder = "icons";
        private readonly string localTalentDataFile = "talent-tree.json";
        /// <summary>
        /// Whitespace to the left of the talent tree
        /// </summary>
        private readonly static int talentLeftOffset = 1800;
        /// <summary>
        /// Whitespace above the talent tree
        /// </summary>
        private readonly static int talentTopOffset = 900;
        /// <summary>
        /// Whitespace to the left of the spec talent tree
        /// </summary>
        private readonly static int talentSpecLeftOffset = 7800;
        /// <summary>
        /// Width of a talent column
        /// </summary>
        private readonly static int talentColumnWidth = 300;
        /// <summary>
        /// Height of a talent row
        /// </summary>
        private readonly static int talentRowHeight = 600;
        private readonly ILogger<TalentStructureUpdateService> _logger;

        public TalentStructureUpdateService()
        {

        }
        public TalentStructureUpdateService(ILogger<TalentStructureUpdateService> logger)
        {
            _logger = logger;
        }

        public async Task UpdateTalentStructure()
        {
            // WARNING: Much of this process will fail if a step errors or provides unexpected data.
            // This is intended, fix the process if something has changed externally.

            // 1. Get data from https://www.raidbots.com/static/data/beta/new-talent-trees.json
            //var rawTalentData = await GetJsonTalentDataAsync();
            var rawTalentData = await GetJsonTalentDataFromFileAsync();

            // 2. Massage it into the format we want 
            var holyPriestData = MassageRawData(rawTalentData);
            // (optionally) generate snippets for code files
            GenerateCodeSnippets(holyPriestData.ClassNodes);
            GenerateCodeSnippets(holyPriestData.SpecNodes);

            // 3. Save it to file
            using FileStream processedDataFile = File.Create(Path.Combine(staticDataRelativePath, localTalentDataFile));
            await JsonSerializer.SerializeAsync<TalentSpec>(processedDataFile, holyPriestData, 
                new JsonSerializerOptions() { WriteIndented = true });

            // 4. Download icons
            await DownloadIcons(holyPriestData, Path.Combine(staticDataRelativePath, iconFolder));
        }

        private async Task<RawSpec[]> GetJsonTalentDataAsync()
        {
            using var client = new HttpClient();

            var jsonData = await client.GetFromJsonAsync<RawSpec[]>(remoteTalentDataUrl);

            return jsonData;
        }

        private async Task<RawSpec[]> GetJsonTalentDataFromFileAsync()
        {
            var jsonData = await JsonSerializer.DeserializeAsync<RawSpec[]>(File.OpenRead(localRawTalentDataFile));

            return jsonData;
        }


        private static TalentSpec MassageRawData(RawSpec[] rawTalentData)
        {
            var hpriest = rawTalentData.Where(t => t.specId == 257).FirstOrDefault();

            // Create new objects and assign all the relevant values to them.
            TalentSpec holyPriest = new TalentSpec()
            {
                ClassName = hpriest.className,
                ClassId = hpriest.classId,
                SpecName = hpriest.specName,
                SpecId = hpriest.specId,
            };

            holyPriest.ClassNodes = MassageTalentNodes(hpriest.classNodes, talentLeftOffset, talentTopOffset);
            holyPriest.SpecNodes = MassageTalentNodes(hpriest.specNodes, talentLeftOffset + talentSpecLeftOffset, talentTopOffset);

            return holyPriest;
        }

        private static List<TalentNode> MassageTalentNodes(List<RawTalent> nodes, int leftOffset, int topOffset)
        {
            List<TalentNode> talents = new List<TalentNode>();

            foreach (var node in nodes)
            {
                var talent = new TalentNode()
                {
                    Id = node.id,
                    Name = node.name,
                    Type = node.type,
                    TreePosX = node.posX,
                    TreePosY = node.posY,
                    MaxRanks = node.maxRanks,
                    EntryNode = node.entryNode,
                    NextNodes = node.next,
                    TalentEntries = MassageTalentOptions(node.entries),
                    IsFreeNode = node.freeNode,
                    RequiredPoints = node.reqPoints,

                    TreeRow = (node.posY - topOffset) / talentRowHeight,
                    TreeColumn = (node.posX - leftOffset) / talentColumnWidth
                };

                talents.Add(talent);               
            }

            return talents;
        }

        private static void GenerateCodeSnippets(List<TalentNode> talents)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            // produce talents for ProfileService::GetHolyPriestTalents
            foreach (var talent in talents)
            {
                foreach (var talentEntry in talent.TalentEntries)
                {
                    var name = textInfo.ToTitleCase(talentEntry.Name.ToLower()).Replace(",", "").Replace("'", "").Replace(" ", "").Replace(":", "");
                    Console.WriteLine($"new Talent(Spell.{name}, {(talent.IsFreeNode ? 1 : 0)}),");
                }
            }

            // produce spells for Spell enum
            foreach (var talent in talents)
            {
                foreach (var talentEntry in talent.TalentEntries)
                {
                    var name = textInfo.ToTitleCase(talentEntry.Name.ToLower()).Replace(",", "").Replace("'", "").Replace(" ", "").Replace(":", "");
                    Console.WriteLine($"{name} = {talentEntry.SpellId},");
                }
            }
        }

        private static List<TalentOption> MassageTalentOptions(List<RawTalentOption> nodes)
        {
            List<TalentOption> options = new List<TalentOption>();

            foreach (var node in nodes)
            {
                var talentOption = new TalentOption()
                {
                    Id = node.id,
                    MaxRanks= node.maxRanks,
                    Type = node.type,
                    Name = node.name,
                    SpellId = node.spellId,
                    Icon = node.icon,
                    DefinitionId = node.definitionId
                };

                // FIX: For spelldata having wrong icon name
                // The spell data is "spell_priest_void flay" and raidbots turns the space into an underscore
                // Battle.net remove the space. Wowhead replace it with a dash "-", raidbots replace it with an underscore "_"
                if (talentOption.Icon == "spell_priest_void_flay")
                    talentOption.Icon = "spell_priest_voidflay";

                // ability_priest_holywordlife doesn't exist on blizzard API at the moment. Obtaining instead from
                // https://wow.zamimg.com/images/wow/icons/large/ability_priest_holywordlife.jpg

                options.Add(talentOption);
            }

            return options;
        }


        private async Task DownloadIcons(TalentSpec holyPriestData, string iconPath)
        {
            using var client = new HttpClient();

            var allTalents = new List<TalentNode>();
            allTalents.AddRange(holyPriestData.ClassNodes);
            allTalents.AddRange(holyPriestData.SpecNodes);
            // Also get the "unknown" icon.
            allTalents.Add(new TalentNode()
            {
                TalentEntries = new List<TalentOption>()
                {
                    new TalentOption()
                    {
                        Icon = "inv_misc_questionmark"
                    }
                }
            });

            foreach (var talent in allTalents)
            {
                foreach(var entry in talent.TalentEntries)
                {
                    // FIX: For spelldata having wrong icon name
                    if (entry.Icon == "spell_priest_void_flay")
                        entry.Icon = "spell_priest_voidflay";

                    var iconName = entry.Icon + ".jpg";
                    var outputFile = Path.Combine(iconPath, iconName);
                    if (!File.Exists(outputFile))
                    {
                        await DownloadIcon(client, battletNetIconUrlBase + iconName, outputFile);
                        _logger?.LogInformation("New icon to download: {iconName}", iconName);
                    }
                }
            }
        }

        private async Task DownloadIcon(HttpClient client, string url, string saveLocation)
        {
            try
            {
                var fileData = await client.GetByteArrayAsync(url);

                File.WriteAllBytes(saveLocation, fileData);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error getting icon: {0}", url);
                Console.WriteLine(ex.Message);
            }

        }

        private class RawSpec
        {
            public string className { get; set; }
            public int classId { get; set; }
            public string specName { get; set; }
            public int specId { get; set; }
            public List<RawTalent> classNodes { get; set; }
            public List<RawTalent> specNodes { get; set; }
        }

        private class RawTalent
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public int posX { get; set; }
            public int posY { get; set; }
            public int maxRanks { get; set; }
            public bool entryNode { get; set; }
            public List<int> next { get; set; }
            public List<RawTalentOption> entries { get; set; }
            public bool freeNode { get; set; }
            public int? reqPoints { get; set; }
        }

        private class RawTalentOption
        {
            public int id { get; set; }
            public int maxRanks { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public int spellId { get; set; }
            public string icon { get; set; }
            public int definitionId { get; set; }
        }
    }
}
