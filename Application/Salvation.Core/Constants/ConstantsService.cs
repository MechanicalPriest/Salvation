using Newtonsoft.Json;
using Salvation.Core.Interfaces.Constants;
using System;
using System.IO;

namespace Salvation.Core.Constants
{
    public class ConstantsService : IConstantsService
    {
        public string DefaultDirectory { get; private set; }
        public string DefaultFilename { get; private set; }

        public ConstantsService()
        {
            DefaultFilename = @"constants.json";
            DefaultDirectory = "";
        }

        public GlobalConstants ParseConstants(string rawConstants)
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

        public void SetDefaultFilename(string defaultFilename)
        {
            DefaultFilename = defaultFilename;
        }

        public GlobalConstants LoadConstantsFromFile()
        {
            string filePath = Path.Combine(DefaultDirectory, DefaultFilename);

            var data = File.ReadAllText(filePath);

            return ParseConstants(data);
        }
    }
}
