using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IEchoOfEonarSpellSevice : ISpellService { }
    class EchoOfEonar : SpellService, ISpellService<IEchoOfEonarSpellSevice>
    {
        public EchoOfEonar(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.EchoOfEonar;
        }

        public override double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingBonus = 0;


            return healingBonus;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = spellData.GetEffect(824536).TriggerSpell.Duration / 1000;

            return duration;
        }
    }
}
