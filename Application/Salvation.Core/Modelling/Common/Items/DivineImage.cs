﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageSpellService : ISpellService { }
    class DivineImage : SpellService, ISpellService<IDivineImageSpellService>
    {
        private readonly ISpellService<IHolyWordSerenitySpellService> _serenitySpellService;
        private readonly ISpellService<IHolyWordSanctifySpellService> _sanctifySpellService;
        private readonly ISpellService<IHolyWordChastiseSpellService> _chastiseSpellService;
        private readonly ISpellService<IDivineImageHealingLightSpellService> _healingLightSpellSevice;
        private readonly ISpellService<IDivineImageTranquilLightSpellService> _tranquilLightSpellService;
        private readonly ISpellService<IDivineImageDazzlingsLightSpellService> _dazzlingLightsSpellService;
        private readonly ISpellService<IDivineImageBlessedLightSpellService> _blessedLightSpellService;

        public DivineImage(IGameStateService gameStateService,
            ISpellService<IHolyWordSerenitySpellService> serenitySpellService,
            ISpellService<IHolyWordSanctifySpellService> sanctifySpellService,
            ISpellService<IHolyWordChastiseSpellService> chastiseSpellService,
            ISpellService<IDivineImageHealingLightSpellService> healingLightSpellSevice,
            ISpellService<IDivineImageTranquilLightSpellService> tranquilLightSpellService,
            ISpellService<IDivineImageDazzlingsLightSpellService> dazzlingLightsSpellService,
            ISpellService<IDivineImageBlessedLightSpellService> blessedLightSpellService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImage;
            _serenitySpellService = serenitySpellService;
            _sanctifySpellService = sanctifySpellService;
            _chastiseSpellService = chastiseSpellService;
            _healingLightSpellSevice = healingLightSpellSevice;
            _tranquilLightSpellService = tranquilLightSpellService;
            _dazzlingLightsSpellService = dazzlingLightsSpellService;
            _blessedLightSpellService = blessedLightSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var castResult = base.GetCastResults(gameState, spellData);

            // Healing is all of the spells the little windchime casts based on what you cast
            // 2 of them are broken and don't scale how they should and haven't been fixed still
            // Duration is garbage
            // Proc rate is atrocious and based on HW casts.
            var windchimeUptime = GetUptime(gameState, spellData) * 60;

            // Healing Light
            var healingLightSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineImageHealingLight);
            healingLightSpellData.Overrides.Add(Override.AllowedDuration, windchimeUptime);
            castResult.AdditionalCasts.Add(_healingLightSpellSevice.GetCastResults(gameState, healingLightSpellData));

            // Tranquil Light
            var tranquilLightSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineImageTranquilLight);
            tranquilLightSpellData.Overrides.Add(Override.AllowedDuration, windchimeUptime);
            castResult.AdditionalCasts.Add(_tranquilLightSpellService.GetCastResults(gameState, tranquilLightSpellData));

            //  Dazzling Lights
            var dazzlingLightsSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineImageDazzlingLight);
            dazzlingLightsSpellData.Overrides.Add(Override.AllowedDuration, windchimeUptime);
            castResult.AdditionalCasts.Add(_dazzlingLightsSpellService.GetCastResults(gameState, dazzlingLightsSpellData));

            //  Blessed Light
            var blessedLightSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineImageBlessedLight);
            blessedLightSpellData.Overrides.Add(Override.AllowedDuration, windchimeUptime);
            castResult.AdditionalCasts.Add(_blessedLightSpellService.GetCastResults(gameState, blessedLightSpellData));

            return castResult;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = spellData.GetEffect(833842).TriggerSpell.Duration / 1000;

            return duration;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // To factor in overlapping casts;
            // hw_per_proc = (1 / proc_chance) + (hwpm / (60 / duration))
            // ppm = hwpm / hw_per_proc 
            // On average it procs every 5 holy words: (1 / proc_chance)
            // We then add on the number of holy words the ICD eats: (hwpm / (60 / duration)) = (hwpm / (60 / 15)) = hwpm / 4
            // Procs per minute is: hwpm / hw_per_proc 

            var procChance = spellData.ProcChance / 100;

            var hwCpm = _serenitySpellService.GetActualCastsPerMinute(gameState, null);
            hwCpm += _sanctifySpellService.GetActualCastsPerMinute(gameState, null);
            hwCpm += _chastiseSpellService.GetActualCastsPerMinute(gameState, null);

            var hwPerProc = (1 / procChance) + (hwCpm / (60 / GetDuration(gameState, spellData)));

            return hwCpm / hwPerProc;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            return GetDuration(gameState, spellData) * GetActualCastsPerMinute(gameState, spellData) / 60;
        }
    }
}
