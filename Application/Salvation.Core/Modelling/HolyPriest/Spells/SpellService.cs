using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class SpellService : ISpellService
    {
        protected readonly IGameStateService _gameStateService;

        public virtual int SpellId { get { return (int)Spell; } }
        public virtual Spell Spell { get; protected set; }

        public SpellService(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            Spell = Spell.None;
        }

        public virtual AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = new AveragedSpellCastResult
            {
                SpellName = spellData.Name,
                SpellId = SpellId,

                CastsPerMinute = GetActualCastsPerMinute(gameState, spellData),
                CastTime = GetHastedCastTime(gameState, spellData),
                Cooldown = GetHastedCooldown(gameState, spellData),
                Damage = GetAverageDamage(gameState, spellData),
                Duration = GetDuration(gameState, spellData),
                Gcd = GetHastedGcd(gameState, spellData),
                Healing = GetAverageHealing(gameState, spellData),
                ManaCost = GetActualManaCost(gameState, spellData),
                MaximumCastsPerMinute = GetMaximumCastsPerMinute(gameState, spellData),
                NumberOfDamageTargets = GetNumberOfDamageTargets(gameState, spellData),
                NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData),
                Overhealing = GetAverageOverhealing(gameState, spellData),
                RawHealing = GetAverageRawHealing(gameState, spellData),
            };

            if (TriggersMastery(gameState, spellData))
            {
                var echoResult = GetHolyPriestMasteryResult(gameState, spellData);
                if (echoResult != null)
                    result.AdditionalCasts.Add(echoResult);
            }

            return result;
        }

        public virtual double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            return 0;
        }

        public virtual double GetAverageHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalDirectHeal = GetAverageRawHealing(gameState, spellData)
                * (1 - castProfile.OverhealPercent);

            return totalDirectHeal;
        }

        public virtual double GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalOverheal = GetAverageRawHealing(gameState, spellData)
                * castProfile.OverhealPercent;

            return totalOverheal;
        }

        public virtual double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            double castsPerMinute = castProfile.Efficiency * GetMaximumCastsPerMinute(gameState, spellData);

            return castsPerMinute;
        }

        public virtual double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            return 0;
        }

        public virtual double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            throw new NotImplementedException();
        }

        public virtual double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the hasted cast time in seconds
            var hastedCastTime = spellData.BaseCastTime / 1000 / _gameStateService.GetHasteMultiplier(gameState);
            return hastedCastTime;
        }

        public virtual double GetHastedGcd(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Can't be lower than GCDFloor (0.75)
            return Math.Max(spellData.Gcd / _gameStateService.GetHasteMultiplier(gameState), _gameStateService.GetGCDFloor(gameState));
        }

        public virtual double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var baseCooldown = spellData.BaseCooldown / 1000d;

            return spellData.IsCooldownHasted
                ? baseCooldown / _gameStateService.GetHasteMultiplier(gameState)
                : baseCooldown;
        }

        public virtual double GetActualManaCost(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var baseMana = _gameStateService.GetBaseManaAmount(gameState);

            return baseMana * (spellData.ManaCost / 100);
        }

        public virtual double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Spells are stored with duration in milliseconds. We want seconds.
            return spellData.Duration / 1000;
        }

        /// <summary>
        /// Gets the number of healing targets the spell will hit by checking spelldata overrides
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="spellData"></param>
        /// <returns></returns>
        public virtual double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var profileData = _gameStateService.GetCastProfile(gameState, SpellId);

            var numTargets = profileData.AverageHealingTargets;

            if (spellData.Overrides.ContainsKey(Override.NumberOfHealingTargets))
                numTargets = spellData.Overrides[Override.NumberOfHealingTargets];

            return numTargets;
        }

        public virtual double GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var profileData = _gameStateService.GetCastProfile(gameState, SpellId);

            var numTargets = profileData.AverageDamageTargets;

            if (spellData.Overrides.ContainsKey(Override.NumberOfDamageTargets))
                numTargets = spellData.Overrides[Override.NumberOfDamageTargets];

            return numTargets;
        }

        public virtual double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        internal virtual BaseSpellData ValidateSpellData(GameState gameState, BaseSpellData spellData)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell);

            if (spellData == null)
                throw new ArgumentOutOfRangeException(nameof(SpellId),
                    $"Spelldata for SpellId ({SpellId}) not found: {Spell}");

            return spellData;
        }

        // This should probably be moved to another class/helper
        #region Holy Priest Specific

        /// <summary>
        /// This does NOT check to see if mastery applies to this spell
        /// </summary>
        public virtual AveragedSpellCastResult GetHolyPriestMasteryResult(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = new AveragedSpellCastResult();

            var averageMasteryHeal = GetAverageRawHealing(gameState, spellData)
                * (_gameStateService.GetMasteryMultiplier(gameState) - 1);

            var castProfile = _gameStateService.GetCastProfile(gameState, (int)Spell.EchoOfLight);

            result.SpellId = (int)Spell.EchoOfLight;
            result.SpellName = "Echo of Light";
            result.RawHealing = averageMasteryHeal;
            result.Healing = averageMasteryHeal * (1 - castProfile.OverhealPercent);

            return result;
        }

        public virtual bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            foreach (var effect in spellData.Effects)
            {
                if(effect.Type == 10)
                {
                    return true;
                }

                if (effect.TriggerSpell != null)
                {
                    foreach (var triggerSpellEffect in effect.TriggerSpell.Effects)
                    {
                        if (triggerSpellEffect.Type == 10)
                        {
                            return true;
                        }
                    }
                }
            }
                return false;
        }

        #endregion
    }
}
