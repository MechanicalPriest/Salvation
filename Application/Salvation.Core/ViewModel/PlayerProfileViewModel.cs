using Salvation.Core.Constants.Data;
using Salvation.Core.Profile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.ViewModel
{
    public class PlayerProfileViewModel
    {
        public int Spec { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string Region { get; set; }
        public int Level { get; set; }
        public int Race { get; set; }
        public int Class { get; set; }
        public List<CastProfileViewModel> Casts { get; set; } = new List<CastProfileViewModel>();
        // TODO: Convert this to viewmodel if needed.
        public List<Item> Items { get; set; } = new List<Item>();
        // TODO: Convert this to viewmodel if needed.
        public List<TalentViewModel> Talents { get; set; } = new List<TalentViewModel>();
        public int FightLengthSeconds { get; set; }
        // TODO: Convert this to viewmodel if needed.
        public List<AdvancedSettingsViewModel> AdvancedSettings { get; set; } = new List<AdvancedSettingsViewModel>();
    }

    public class CastProfileViewModel
    {
        public int SpellId { get; set; }
        public string Description { get; set; } = "";
        public double Efficiency { get; set; }
        public double OverhealPercent { get; set; }
        public double AverageHealingTargets { get; set; }
        public double AverageDamageTargets { get; set; }

        public double EfficiencyPercentValue
        {
            get => ((int)(Efficiency * 10000)) / 100d;
            set => Efficiency = value / 100;
        }

        public double OverhealingPercentValue
        {
            get => ((int)(OverhealPercent * 10000)) / 100d;
            set => OverhealPercent = value / 100;
        }
    }

    public class AdvancedSettingsViewModel
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public int SpellId { get; set; }

    }

    public class TalentViewModel
    {
        public Spell Spell { get; set; }
        public int SpellId { get; set; }
        public int Rank { get; set; }
    }

    public static class PlayerProfileExtensions
    {
        public static PlayerProfileViewModel ToViewModel(this PlayerProfile profile)
        {
            var profileVM = new PlayerProfileViewModel();

            profileVM.Spec = (int)profile.Spec;
            profileVM.Name = profile.Name;
            profileVM.Server = profile.Server;
            profileVM.Region = profile.Region;
            profileVM.Level = profile.Level;
            profileVM.Race = (int)profile.Race;
            profileVM.Class = (int)profile.Class;
            profileVM.Items = profile.Items;
            profileVM.Casts = profile.Casts.Select(c => c.ToViewModel()).ToList();
            profileVM.Talents = profile.Talents.Select(c => c.ToViewModel()).ToList();
            profileVM.FightLengthSeconds = profile.FightLengthSeconds;
            profileVM.AdvancedSettings = profile.PlaystyleEntries.Select(c => c.ToViewModel()).ToList();

            return profileVM;
        }

        public static PlayerProfile ToModel(this PlayerProfileViewModel profileVM)
        {
            var profile = new PlayerProfile();

            profile.Spec = (Spec)profileVM.Spec;
            profile.Name = profileVM.Name;
            profile.Server = profileVM.Server;
            profile.Region = profileVM.Region;
            profile.Level = profileVM.Level;
            profile.Race = (Race)profileVM.Race;
            profile.Class = (Class)profileVM.Class;
            profile.Items = profileVM.Items;
            profile.Casts = profileVM.Casts.Select(c => c.ToModel()).ToList();
            profile.Talents = profileVM.Talents.Select(c => c.ToModel()).ToList();
            profile.FightLengthSeconds = profileVM.FightLengthSeconds;
            profile.PlaystyleEntries = profileVM.AdvancedSettings.Select(c => c.ToModel()).ToList();

            return profile;
        }
    }

    public static class CastProfileExtensions
    {
        public static CastProfileViewModel ToViewModel(this CastProfile profile)
        {
            var profileVM = new CastProfileViewModel();

            profileVM.SpellId = profile.SpellId;
            profileVM.Description = profile.Description;
            profileVM.Efficiency = profile.Efficiency;
            profileVM.OverhealPercent = profile.OverhealPercent;
            profileVM.AverageHealingTargets = profile.AverageHealingTargets;
            profileVM.AverageDamageTargets = profile.AverageDamageTargets;

            return profileVM;
        }

        public static CastProfile ToModel(this CastProfileViewModel profileVM)
        {
            var profile = new CastProfile();

            profile.SpellId = profileVM.SpellId;
            profile.Description = profileVM.Description;
            profile.Efficiency = profileVM.Efficiency;
            profile.OverhealPercent = profileVM.OverhealPercent;
            profile.AverageHealingTargets = profileVM.AverageHealingTargets;
            profile.AverageDamageTargets = profileVM.AverageDamageTargets;

            return profile;
        }
    }
    public static class AdvancedSettingsExtensions
    {
        public static AdvancedSettingsViewModel ToViewModel(this PlaystyleEntry profile)
        {
            var profileVM = new AdvancedSettingsViewModel();

            profileVM.SpellId = (int)profile.SpellId;
            profileVM.Name = profile.Name;
            profileVM.Value = profile.Value;

            return profileVM;
        }

        public static PlaystyleEntry ToModel(this AdvancedSettingsViewModel profileVM)
        {
            var profile = new PlaystyleEntry();

            profile.SpellId = (uint)profileVM.SpellId;
            profile.Name = profileVM.Name;
            profile.Value = profileVM.Value;

            return profile;
        }
    }
    public static class TalentExtensions
    {
        public static TalentViewModel ToViewModel(this Profile.Model.Talent talent)
        {
            var talentVM = new TalentViewModel
            {
                Rank = talent.Rank,
                Spell = talent.Spell,
                SpellId = talent.SpellId,
            };

            return talentVM;
        }

        public static Profile.Model.Talent ToModel(this TalentViewModel talentVM)
        {
            var talent = new Profile.Model.Talent
            {
                Rank = talentVM.Rank,
                Spell = talentVM.Spell,
                SpellId = talentVM.SpellId,
            };

            return talent;
        }
    }
}
