using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests
{
    public class ConstantManagerTests
    {
        [Test]
        public void SaveConstants()
        {
            GlobalConstants gc = new GlobalConstants();

            gc.GameVersion = "9.0.0.1";
            gc.Specs.Add(new BaseSpec()
            {
                Class = "Priest",
                Spec = "Holy",
                SpecId = 257,
                
                CritBase = .05M,
                HasteBase = 0,
                VersBase = 0,
                MasteryBase = 0.1M,
                IntBase = 450,
                StamBase = 416,
                ManaBase = 100000,

                Spells = new List<BaseSpellData>()
                {
                    new BaseSpellData()
                    {
                        Id = 2060,
                        Name = "Heal",
                        ManaCost = 0.024M,
                        Range = 40,
                        BaseCastTime = 2.5M,
                        IsCastTimeHasted = true,
                        Gcd = 1.5M,
                        Coeff1 = 2.1M
                    }
                },

                Modifiers = new List<BaseModifier>()
                {
                    new BaseModifier()
                    {
                        Name = "BeneRenewChance",
                        Value = 0.25M
                    }
                }
            });

            var gcText = JsonConvert.SerializeObject(gc);
            File.WriteAllText("constants.json", gcText);
        }

        [Test]
        public void ParseConstants()
        {
            var data = File.ReadAllText(@"constants.json");

            var result = ConstantsManager.ParseConstants(data);

            Console.Write(result);
        }
    }
}
