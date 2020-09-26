using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using System;
using System.Linq;

namespace Salvation.Core.State
{
    public class GameStateService : IGameStateService
    {
        public decimal GetBaseManaAmount(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return specData.ManaBase;
        }

        public CastProfile GetCastProfile(GameState state, int spellId)
        {
            var castProfile = state.Profile.Casts?
                .Where(c => c.SpellId == spellId)
                .FirstOrDefault();

            castProfile = JsonConvert.DeserializeObject<CastProfile>(JsonConvert.SerializeObject(castProfile));

            return castProfile;
        }

        public decimal GetCriticalStrikeMultiplier(GameState state)
        {
            // TODO: Add other sources of crit increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.CritBase + (state.Profile.CritRating / specData.CritCost / 100);
        }

        public decimal GetHasteMultiplier(GameState state)
        {
            // TODO: Add other sources of haste increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.HasteBase + (state.Profile.HasteRating / specData.HasteCost / 100);
        }

        public decimal GetVersatilityMultiplier(GameState state)
        {
            // TODO: Add other sources of vers increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.VersBase + (state.Profile.VersatilityRating / specData.VersCost / 100);
        }

        public decimal GetMasteryMultiplier(GameState state)
        {
            // TODO: Add other sources of mastery increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.MasteryBase + (state.Profile.MasteryRating / specData.MasteryCost / 100);
        }

        public decimal GetIntellect(GameState state)
        {
            // TODO: Add other sources of int increase here
            return state.Profile.Intellect;
        }

        public BaseModifier GetModifier(GameState state, string modifierName)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var modifier = specData.Modifiers.Where(s => s.Name == modifierName).FirstOrDefault();

            modifier = JsonConvert.DeserializeObject<BaseModifier>(JsonConvert.SerializeObject(modifier));

            return modifier;
        }

        public BaseSpellData GetSpellData(GameState state, SpellIds spellId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            BaseSpellData spell = specData.Spells.Where(s => s.Id == (int)spellId).FirstOrDefault();

            spell = JsonConvert.DeserializeObject<BaseSpellData>(JsonConvert.SerializeObject(spell));

            return spell;
        }

        public ConduitData GetConduitData(GameState state, Conduit conduitId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            ConduitData conduit = specData.Conduits
                .Where(c => c.Id == (int)conduitId).FirstOrDefault();

            conduit = JsonConvert.DeserializeObject<ConduitData>(
                JsonConvert.SerializeObject(conduit));

            return conduit;
        }

        public bool IsConduitActive(GameState state, Conduit conduit)
        {
            var exists = state.Profile.Conduits.Keys.Contains(conduit);

            return exists;
        }

        public int GetConduitRank(GameState state, Conduit conduit)
        {
            var rank = state.Profile.Conduits[conduit];

            return rank;
        }

        public Covenant GetActiveCovenant(GameState state)
        {
            return state.Profile.Covenant;
        }

        public void OverrideSpellData(GameState state, BaseSpellData newData)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var requestedData = specData.Spells.Where(s => s.Id == newData.Id).FirstOrDefault();

            if (requestedData == null)
                return;

            specData.Spells.Remove(requestedData);
            specData.Spells.Add(newData);
        }

        public void OverrideModifier(GameState state, BaseModifier newModifier)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var requestedModifier = specData.Modifiers.Where(s => s.Name == newModifier.Name).FirstOrDefault();

            if (requestedModifier == null)
                return;

            specData.Modifiers.Remove(requestedModifier);
            specData.Modifiers.Add(newModifier);
        }

        public GameState CloneGameState(GameState state)
        {
            var stateString = JsonConvert.SerializeObject(state);

            var newState = JsonConvert.DeserializeObject<GameState>(stateString);

            return newState;
        }
    }
}
