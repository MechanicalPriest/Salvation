using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.ViewModel
{
    public class Spec
    {
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        public string SpecName { get; set; }
        public int SpecId { get; set; }
        public List<Talent> ClassNodes { get; set; }
        public List<Talent> SpecNodes { get; set; }
    }

    public class Talent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TreePosX { get; set; }
        public int TreePosY { get; set; }
        public int MaxRanks { get; set; }
        public bool EntryNode { get; set; }
        public List<int> NextNodes { get; set; }
        public List<TalentOption> TalentEntries { get; set; }
        public bool IsFreeNode { get; set; }
        public int? RequiredPoints { get; set; }

        // Manual fields
        public int TreeRow { get; set; }
        public int TreeColumn { get; set; }
    }

    public class TalentOption
    {
        public int Id { get; set; }
        public int MaxRanks { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int SpellId { get; set; }
        public string Icon { get; set; }
    }
}
