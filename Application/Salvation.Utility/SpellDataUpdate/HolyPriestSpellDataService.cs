using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public class HolyPriestSpellDataService : SpellDataService,
        ISpellDataService<HolyPriestSpellDataService>
    {
        private readonly ISimcGenerationService _simcGenerationService;
        private readonly IList<uint> _spells;

        public HolyPriestSpellDataService(ISimcGenerationService simcGenerationService)
        {
            _simcGenerationService = simcGenerationService;
            _spells = new List<uint>()
            {
                // Talents
                (uint)Spell.Benediction
            };
        }

        public async Task<BaseSpec> Generate()
        {
            var spec = new BaseSpec();
            var spells = new List<BaseSpellData>();

            foreach(var spell in _spells)
            {
                var newSpell = new BaseSpellData();

                // TODO: feed up level 60 from somewhere else
                var spellOptions = new SimcSpellOptions()
                {
                    SpellId = spell,
                    PlayerLevel = 60
                };

                var spellData = await _simcGenerationService.GenerateSpellAsync(spellOptions);

                newSpell.Id = spellData.SpellId;
                newSpell.Name = spellData.Name;

                // Bene's proc chance is effect[0].basevalue

                spells.Add(newSpell);
            }

            spec.Spells = spells;

            return spec;
        }
    }
}
