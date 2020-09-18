using Newtonsoft.Json;
using Salvation.Core.Constants;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

[assembly: InternalsVisibleTo("Salvation.CoreTests")]
namespace Salvation.Core
{
    public class ConstantsManager
    {
        public string DefaultDirectory { get; private set; }
        public string DefaultFileName { get; private set; }

        public ConstantsManager()
        {
            DefaultFileName = @"constants.json";
            DefaultDirectory = "";
        }

        public static GlobalConstants ParseConstants(string rawConstants)
        {
            GlobalConstants constants = default(GlobalConstants);

            try
            {
                constants = JsonConvert.DeserializeObject<GlobalConstants>(rawConstants);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error deserialising constants: " + ex.Message);
            }

            return constants;
        }

        public void SetDefaultDirectory(string defaultFilePath)
        {
            DefaultDirectory = defaultFilePath;
        }

        public void SetDefaultFileName(string defaultFileName)
        {
            DefaultFileName = defaultFileName;
        }

        public GlobalConstants LoadConstantsFromFile()
        {
            string filePath = Path.Combine(DefaultDirectory, DefaultFileName);

            var data = File.ReadAllText(filePath);

            return ParseConstants(data);
        }
    }
}
