using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IAnsweredPrayersSpellService : ISpellService { }
    public class AnsweredPrayers : SpellService, ISpellService<IAnsweredPrayersSpellService>
    {
        private readonly ISpellService<IPrayerOfMendingSpellService> _prayerOfMendingSpellService;

        public AnsweredPrayers(IGameStateService gameStateService,
            ISpellService<IPrayerOfMendingSpellService> prayerOfMendingSpellService)
            : base(gameStateService)
        {
            Spell = Spell.AnsweredPrayers;
            _prayerOfMendingSpellService = prayerOfMendingSpellService;
        }

        /// <summary>
        /// The duration of the Apotheosis buff when it procs
        /// </summary>
        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = spellData.GetEffect(1028870).BaseValue;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            var actualCpm = GetActualCastsPerMinute(gameState, spellData);

            var uptime = duration * actualCpm / 60;

            return uptime;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // This will depend on PoM casts, bounces and how many PoM's expire.
            var cpmPoM = _prayerOfMendingSpellService.GetActualCastsPerMinute(gameState);

            var numPoMBounces = ((IPrayerOfMendingExtensions)_prayerOfMendingSpellService).GetAverageBounces(gameState, null);

            var rank = _gameStateService.GetTalent(gameState, Spell.AnsweredPrayers).Rank;

            var bouncesPerBuff = spellData.GetEffect(1028869).BaseValue * 2 / rank;

            double maximumPotentialCasts = (cpmPoM * numPoMBounces) / bouncesPerBuff;

            return maximumPotentialCasts;
        }
    }
}
