using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class ResonantWordsTests : BaseTest
    {
        GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void RW_Increases_Heal_Amount()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService(new ProfileService(), new ConstantsService(), new RWTestSpellFactory());
            var profileService = new ProfileService();
            var spellService = new Heal(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            var spellList = new List<Core.Profile.Model.RegisteredSpell>()
            {
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordSerenity),
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordSanctify),
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordChastise)
            };

            gameStateService.RegisterSpells(gamestate1, spellList);
            gameStateService.RegisterSpells(gamestate2, spellList);
            gameStateService.OverridePlaystyle(gamestate1, new Core.Profile.Model.PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.90));
            gameStateService.OverridePlaystyle(gamestate1, new Core.Profile.Model.PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.80));

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.ResonantWords, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(5257.9023084315022d, resultWithout);
            Assert.AreEqual(7255.9051856354736d, resultWith);
        }

        [Test]
        public void RW_Increases_FlashHeal_Amount()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService(new ProfileService(), new ConstantsService(), new RWTestSpellFactory());
            var profileService = new ProfileService();
            var spellService = new FlashHeal(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            var spellList = new List<Core.Profile.Model.RegisteredSpell>()
            {
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordSerenity),
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordSanctify),
                new Core.Profile.Model.RegisteredSpell(Spell.HolyWordChastise)
            };

            gameStateService.RegisterSpells(gamestate1, spellList);
            gameStateService.RegisterSpells(gamestate2, spellList);
            gameStateService.OverridePlaystyle(gamestate1, new Core.Profile.Model.PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.90));
            gameStateService.OverridePlaystyle(gamestate1, new Core.Profile.Model.PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.80));

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.ResonantWords, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(3618.1497241071006d, resultWithout);
            Assert.AreEqual(4251.2316550826417d, resultWith);
        }
    }

    class RWTestSpellFactory : ISpellServiceFactory
    {
        public ISpellService GetSpellService(Spell spell)
        {
            IGameStateService gss = new GameStateService();
            switch(spell)
            {
                case Spell.HolyWordSerenity:
                    return new HolyWordSerenity(gss, new FlashHeal(gss), new Heal(gss), new BindingHeal(gss), new PrayerOfMending(gss));
                case Spell.HolyWordSanctify:
                    return new HolyWordSanctify(gss, new PrayerOfHealing(gss), new Renew(gss), new BindingHeal(gss), new CircleOfHealing(gss));
                case Spell.HolyWordChastise:
                    return new HolyWordChastise(gss, new Smite(gss), new HolyFire(gss));
                default:
                    return null;
            }
        }
    }
}
