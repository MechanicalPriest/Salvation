using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface ICombatMeditation : ISpellService { }

    internal class CombatMeditation : SpellService, ISpellService<ICombatMeditation>
    {
        private readonly ISpellService<IBoonOfTheAscendedSpellService> _boonOfTheAscendedSpellService;

        public CombatMeditation(IGameStateService gameStateService,
            ISpellService<IBoonOfTheAscendedSpellService> boonOfTheAscendedSpellService)
            : base(gameStateService)
        {
            Spell = Spell.CombatMeditation;
            _boonOfTheAscendedSpellService = boonOfTheAscendedSpellService;
        }

        public override double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // +mastery for duration extended up to duration * 3 times.
            // 328908e1 for 328908d extended up to 328913e2 * 3 times.
            var masteryBuffSpell = _gameStateService.GetSpellData(gameState, Spell.CombatMeditationBuff);

            if (!masteryBuffSpell.ScaleValues.ContainsKey(PlayerLevel))
                throw new ArgumentOutOfRangeException("PlayerLevel", $"masteryBuffSpell.ScaleValues does not contain player level: {PlayerLevel}");

            var scaleBudget = masteryBuffSpell.ScaleValues[PlayerLevel];

            // Mastery amount: 328908 effect 1
            var masteryAmount = scaleBudget * masteryBuffSpell.GetEffect(821722).Coefficient;

            return masteryAmount * GetUptime(gameState, spellData) / 60;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the base duration: 328908d
            var masteryBuffSpell = _gameStateService.GetSpellData(gameState, Spell.CombatMeditationBuff);

            var baseDuration = masteryBuffSpell.Duration / 1000;

            // Get the added duration per orb: 328913 e2 * 3
            // 3 is a constant from 328266 "Variables".
            var orbExtensionSpell = _gameStateService.GetSpellData(gameState, Spell.CombatMeditationExtension);
            var orbExtensionDuration = orbExtensionSpell.GetEffect(829213).BaseValue * 3d;

            // Get the number of orbs picked up on average
            var orbPickupMulti = _gameStateService.GetPlaystyle(gameState, "CombatMeditationOrbPickups");

            if(orbPickupMulti == null)
                throw new ArgumentOutOfRangeException("CombatMeditationOrbPickups", $"CombatMeditationOrbPickups needs to be set.");

            var numOrbsPickedUp = 3d * orbPickupMulti.Value;

            return baseDuration + (orbExtensionDuration * numOrbsPickedUp);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            var cpm = _boonOfTheAscendedSpellService.GetActualCastsPerMinute(gameState, null);

            return duration * cpm;
        }
    }
}
