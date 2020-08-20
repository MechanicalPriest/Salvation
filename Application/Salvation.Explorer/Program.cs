using Salvation.Core;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.IO;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHolyPriestModel();
        }

        private static void TestHolyPriestModel()
        {
            var data = File.ReadAllText(@"constants.json");

            var globalConstants = ConstantsManager.ParseConstants(data);

            var basicProfile = new BaseProfile()
            {
                Intellect = 978,
                MasteryRating = 202,
                VersatilityRating = 139,
                HasteRating = 282,
            };

            var hpriest = new HolyPriestModel(globalConstants, basicProfile);

            foreach(var spell in hpriest.Spells)
            {
                if(spell is BaseHealingSpell healingSpell)
                {
                    Console.WriteLine(healingSpell.AverageHeal);
                }
            }

            Console.WriteLine(hpriest.Spells);
        }
    }
}
