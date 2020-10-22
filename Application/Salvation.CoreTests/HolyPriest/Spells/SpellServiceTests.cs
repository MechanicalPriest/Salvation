using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    /// <summary>
    /// A mixture of integration / unit tests to ensure the methods return expected results 
    /// regardless of different configuration values
    /// </summary>
    [TestFixture]
    public class SpellServiceTests
    {
        private List<SpellService> Spells { get; set; }

        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            Spells = new List<SpellService>();

            IGameStateService gameStateService = new GameStateService();
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var basePath = @"HolyPriest\TestData";
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_profile.json")));

            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new BindingHeal(gameStateService));
            Spells.Add(new BoonOfTheAscended(gameStateService,
                new AscendedBlast(gameStateService),
                new AscendedNova(gameStateService),
                new AscendedEruption(gameStateService)));
            Spells.Add(new CircleOfHealing(gameStateService));
            Spells.Add(new DivineHymn(gameStateService));
            Spells.Add(new DivineStar(gameStateService));
            Spells.Add(new FaeGuardians(gameStateService,
                new DivineHymn(gameStateService)));
            Spells.Add(new FlashHeal(gameStateService));
            Spells.Add(new Halo(gameStateService));
            Spells.Add(new Heal(gameStateService));
            Spells.Add(new HolyNova(gameStateService));
            Spells.Add(new HolyWordSalvation(gameStateService,
                new HolyWordSerenity(gameStateService,
                    new FlashHeal(gameStateService),
                    new Heal(gameStateService),
                    new BindingHeal(gameStateService),
                    new PrayerOfMending(gameStateService)),
                new HolyWordSanctify(gameStateService,
                    new PrayerOfHealing(gameStateService),
                    new Renew(gameStateService),
                    new BindingHeal(gameStateService),
                    new CircleOfHealing(gameStateService)),
                new Renew(gameStateService),
                new PrayerOfMending(gameStateService)));
            Spells.Add(new HolyWordSanctify(gameStateService,
                new PrayerOfHealing(gameStateService),
                new Renew(gameStateService),
                new BindingHeal(gameStateService),
                new CircleOfHealing(gameStateService)));
            Spells.Add(new HolyWordSerenity(gameStateService,
                new FlashHeal(gameStateService),
                new Heal(gameStateService),
                new BindingHeal(gameStateService),
                new PrayerOfMending(gameStateService)));
            Spells.Add(new Mindgames(gameStateService));
            Spells.Add(new PowerWordShield(gameStateService));
            Spells.Add(new PrayerOfHealing(gameStateService));
            Spells.Add(new PrayerOfMending(gameStateService));
            Spells.Add(new Renew(gameStateService));
            Spells.Add(new SpellService(gameStateService));
            Spells.Add(new UnholyNova(gameStateService,
                new UnholyTransfusion(gameStateService)));
            Spells.Add(new UnholyTransfusion(gameStateService));

            _gameState = new GameState(profile, constants);
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMaximumHealTargets))]
        public double GetMaximumHealTargets(Type t)
        {
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            var result = spellService.GetMaximumHealTargets(_gameState, null);

            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMinimumHealTargets))]
        public double GetMinimumHealTargets(Type t)
        {
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            var result = spellService.GetMinimumHealTargets(_gameState, null);

            return result;
        }
    }

    public class SpellServiceTestsData
    {
        public static IEnumerable GetMaximumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(6);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(3);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(DivineStar)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1);
                yield return new TestCaseData(typeof(Halo)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(Heal)).Returns(1);
                yield return new TestCaseData(typeof(HolyNova)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(6);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1);
                yield return new TestCaseData(typeof(Renew)).Returns(1);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(6);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
            }
        }
        public static IEnumerable GetMinimumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(2);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(1);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(1);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1);
                yield return new TestCaseData(typeof(Halo)).Returns(1);
                yield return new TestCaseData(typeof(Heal)).Returns(1);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1);
                yield return new TestCaseData(typeof(Mindgames)).Returns(0);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1);
                yield return new TestCaseData(typeof(Renew)).Returns(1);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
            }
        }
    }
}
