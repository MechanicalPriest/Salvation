using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Salvation.Utility.SpellDataDump
{
    class SpellDataParser
    {
        static void Parse(string spellData)
        {
            string firstLine = new StringReader(spellData).ReadLine();

            SpellDataVersion version = new SpellDataVersion(firstLine);

            Console.WriteLine(version.WowVersion);

            Dictionary<int, Dictionary<string, string>> spells = ParseSpells(spellData);

            spells[17].Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Console.WriteLine);
        }

        static Dictionary<int, Dictionary<string, string>> ParseSpells(string spellData)
        {
            List<string> spells = new List<string>();
            Dictionary<int, Dictionary<string, string>> spellsOut = new Dictionary<int, Dictionary<string, string>>();


            int startIndex = spellData.IndexOf("Name             :");
            int nextIndex = spellData.IndexOf("Name             :", startIndex + 18);

            while (nextIndex != -1)
            {
                spells.Add(spellData[startIndex..nextIndex]);

                startIndex = nextIndex;
                nextIndex = spellData.IndexOf("Name             :", startIndex + 18);
            }

            spells.Add(spellData.Substring(startIndex));


            foreach (string spell in spells)
            {
                StringReader reader = new StringReader(spell);

                Match m = Regex.Match(reader.ReadLine(), @"^Name\s+: (.+?\(id=(\d+)\).+$)");

                if (m.Success)
                {
                    int id = int.Parse(m.Groups[2].Value);

                    spellsOut.Add(id, new Dictionary<string, string>());
                    spellsOut[id]["Name"] = m.Groups[1].Value;
                    string lastKey = "";
                    while (true)
                    {
                        string line = reader.ReadLine();

                        if (line != null)
                        {
                            string[] kvp;
                            string key;

                            // Blank line
                            if (line.Length == 0)
                            {
                                continue;
                            }

                            // KVP
                            if (line.Length > 17 && line[17] == ':')
                            {
                                kvp = line.Split(":", 2);
                            }
                            // Not a KVP but might have a :
                            else
                            {
                                kvp = new string[1];
                                kvp[0] = line.Trim();
                            }

                            // We were able to split by : so likely a KVP
                            if (kvp.Length == 2)
                            {
                                key = kvp[0].Trim();

                                // Key exists
                                if (key.Length > 0)
                                {
                                    Match idMatch = Regex.Match(key, @"(#\d+) (\(id=\d+\))");
                                    if (idMatch.Success)
                                    {
                                        key = idMatch.Groups[1].Value;
                                        kvp[1] = idMatch.Groups[2].Value + " " + kvp[1];
                                    }
                                    spellsOut[id][key] = kvp[1].Trim();
                                    lastKey = key;

                                }
                                // Blank key, append to previous key
                                else
                                {
                                    spellsOut[id][lastKey] += kvp[1].Trim();
                                }
                            }
                            // Not a KVP, append to previous key
                            else
                            {
                                spellsOut[id][lastKey] += " " + line.Trim();
                            }

                        }
                        else
                        {
                            break;
                        }

                    };
                }


            }

            return spellsOut;

        }
    }
}
