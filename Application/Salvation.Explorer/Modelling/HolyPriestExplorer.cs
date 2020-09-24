﻿using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    public interface IHolyPriestExplorer
    {
        public void TestNewHolyPriestModel();
        public void TestHolyPriestModel();
        public void CompareCovenants();
    }

    class HolyPriestExplorer : IHolyPriestExplorer
    {
        private readonly IConstantsService constantsService;
        private readonly IModellingService modellingService;

        public HolyPriestExplorer(IConstantsService constantsService, IModellingService modellingService)
        {
            this.constantsService = constantsService;
            this.modellingService = modellingService;
        }

        public void TestHolyPriestModel()
        {
            var globalConstants = constantsService.LoadConstantsFromFile();

            GenerateStatWeights(constantsService);
        }

        public void GenerateStatWeights(IConstantsService constantsManager)
        {
            var basicProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);

            StatWeightGenerator sw = new StatWeightGenerator(constantsManager);
            var results = sw.Generate(basicProfile, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }

        public void CompareCovenants()
        {
            var cc = new CovenantComparisons();

            var results = new List<BaseModelResults>();

            //results.Add(cc.GetBaseResult()); // Maybe use this if we start using the full resultset
            results.Add(cc.GetMindgamesResults());
            results.Add(cc.GetFaeGuardiansDROnlyResults());
            results.Add(cc.GetFaeGuardiansHymnCDRResults());

            foreach (var result in results)
            {
                var covSpellResult = result.SpellCastResults.Last() as AveragedSpellCastResult;
                Console.WriteLine($"{result.Profile.Name} - " +
                    $"{covSpellResult.RawHealing} ({covSpellResult.RawHPS})");
            }
        }

        public void TestNewHolyPriestModel()
        {
            GameState state = new GameState();
            state.Constants = constantsService.LoadConstantsFromFile();
            state.Profile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);

            var results = modellingService.GetResults(state);
        }
    }
}