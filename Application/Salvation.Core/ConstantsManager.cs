using Newtonsoft.Json;
using Salvation.Core.Constants;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Salvation.CoreTests")]
namespace Salvation.Core
{
    public class ConstantsManager
    {
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
    }
}
