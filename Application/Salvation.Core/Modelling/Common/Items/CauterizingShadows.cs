using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ICauterizingShadowsSpellSevice : ISpellService { }
    class CauterizingShadows : SpellService, ISpellService<ICauterizingShadowsSpellSevice>
    {
        private readonly IShadowWordPainSpellService _shadowWordPainSpellService;

        public CauterizingShadows(IGameStateService gameStateService,
            IShadowWordPainSpellService shadowWordPainSpellService)
            : base(gameStateService)
        {
            Spell = Spell.CauterizingShadows;
            _shadowWordPainSpellService = shadowWordPainSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            // grab healing data, use the other methods to calc
            return base.GetAverageRawHealing(gameState, spellData);
        }

        public override double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            // max targets
            return base.GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            // 3 from spelldata
            return base.GetMaximumHealTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // Maximum with the casts that don't do healing (target dies / sw:p is refreshed
            return base.GetActualCastsPerMinute(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // sw:p casts
            return base.GetMaximumCastsPerMinute(gameState, spellData);
        }
    }
}
