﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Salvation.Core.ViewModel;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Salvation.Client.Shared.Components
{
    public partial class TalentViewer : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory? _httpClientFactory { get; set; }

        [Parameter]
        public Dictionary<int, int> SelectedTalents { get; set; } = new Dictionary<int, int>();

        [Parameter]
        public bool DisplayVertical { get; set; } = false;

        public TalentSpec? HolyPriest { get; set; }

        [Parameter]
        public Action<TalentSpec>? TalentDefinitionChanged { get; set; }

        private static int ClassPoints = 31;
        private static int SpecPoints = 30;

        private TalentTree? classTalentViewer;
        private TalentTree? specTalentViewer;

        private string wclImportText = String.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (_httpClientFactory == null)
                throw new NullReferenceException("Web client was not initialised");

            var client = _httpClientFactory.CreateClient("StaticData");
            HolyPriest = await client.GetFromJsonAsync<TalentSpec>("talent-tree.json");

            if(HolyPriest != null && TalentDefinitionChanged != null)
                TalentDefinitionChanged(HolyPriest);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("updateWowheadTooltips");
        }

        private void ProcessWclImport()
        {
            if (HolyPriest == null || classTalentViewer == null || specTalentViewer == null)
                return;

            // Loop through each line and look for potential talents.
            var lines = wclImportText.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            // First clear the tree
            classTalentViewer.ResetTalentTree();
            specTalentViewer.ResetTalentTree();

            foreach (var line in lines)
            {
                var inputLine = line.Trim();

                if(string.IsNullOrEmpty(inputLine))
                    continue;

                // If it's a talent, the row should end in a 1 or a 2.
                if(new[] { '1', '2' }.Contains(inputLine.Last()))
                {
                    // Get the rank
                    var rank = 0;
                    int.TryParse(inputLine.Last().ToString(), out rank);

                    var talentName = inputLine.Remove(inputLine.Length - 1, 1);
                    talentName = talentName.Trim();

                    var talent = HolyPriest.ClassNodes.Where(n => n.Name.Contains(talentName)).FirstOrDefault();

                    if (talent != null)
                    {
                        // If it's a choice node, select the right choice node.
                        bool? selectFirstChoiceOption = null;

                        if (talent.TalentEntries.Count == 2)
                            selectFirstChoiceOption = talent.TalentEntries[0].Name == talentName;

                        classTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                        if(rank == 2)
                            classTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);
                    }
                    else
                    {
                        talent = HolyPriest.SpecNodes.Where(n => n.Name.Contains(talentName)).FirstOrDefault();

                        if (talent == null)
                            continue;

                        // If it's a choice node, select the right choice node.
                        bool? selectFirstChoiceOption = null;

                        if (talent.TalentEntries.Count == 2)
                            selectFirstChoiceOption = talent.TalentEntries[0].Name == talentName;

                        specTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                        if (rank == 2)
                            specTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                    }
                }
            }
        }

        public void UpdateSelectedTalents(Dictionary<int, int> SelectedTalents)
        {
            if (HolyPriest == null || classTalentViewer == null || specTalentViewer == null)
                return;

            // First clear the tree
            classTalentViewer.ResetTalentTree();
            specTalentViewer.ResetTalentTree();

            foreach (var selectedTalent in SelectedTalents)
            {
                var spellId = selectedTalent.Key;
                var rank = selectedTalent.Value;

                // Find the talent in the list
                var talent = HolyPriest.ClassNodes.Where(n => n.TalentEntries.Where(te => te.SpellId == spellId).Any()).FirstOrDefault();

                if (talent != null)
                {
                    // If it's a choice node, select the right choice node.
                    bool? selectFirstChoiceOption = null;

                    if (talent.TalentEntries.Count == 2)
                        selectFirstChoiceOption = talent.TalentEntries[0].SpellId == spellId;

                    classTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                    if (rank == 2)
                        classTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);
                }
                else
                {
                    talent = HolyPriest.SpecNodes.Where(n => n.TalentEntries.Where(te => te.SpellId == spellId).Any()).FirstOrDefault();

                    if (talent == null)
                        continue;

                    // If it's a choice node, select the right choice node.
                    bool? selectFirstChoiceOption = null;

                    if (talent.TalentEntries.Count == 2)
                        selectFirstChoiceOption = talent.TalentEntries[0].SpellId == spellId;

                    specTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                    if (rank == 2)
                        specTalentViewer.TrySpendTalentPoint(talent, selectFirstChoiceOption);

                }
            }
        }
    }
}