using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    partial class GapTest2
    {
        bool controlsDisabled;
        bool playDisabled;
        bool selectTenorWeightDisabled;
        bool currentTenorWeightDisabled;
        bool showGaps;
        BlowSet blowSet;
        Screen screen;
        bool spinnerSaving;
        string saveLabel;
        string submitLabel;
        bool saved;
        string playLabel;
        TestSpec testSpec;
        IEnumerable<GapTestData> gapTestsData;
        int selectedTest = -1;

        public GapTest2()
        {
            testSpec = new TestSpec
            {
                Stage = 8,
                TenorWeight = 23,
                NumRows = 4,
                ErrorType = 0,
                ErrorSize = 80,
                TestBellLoc = 1
            };

            // In a Gap Test, gaps are rounded to the nearest 10ms
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, Constants.Rounding);

            screen = new Screen
            {
                DiameterScale = Constants.DiameterScale,
                XScale = Constants.XScale,
                XMargin = Constants.XMargin,
                YScale = Constants.YScale,
                YMargin = Constants.YMargin,
                GapMin = 20,
                GapMax = 700,
                BaseGap = baseGap
            };
        }

        [Inject]
        HttpClient Http { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            gapTestsData = (await Http.GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();
            saveLabel = "Save";
            submitLabel = "Submit";
        }

        void TestChanged(int value)
        {
            selectedTest = value;
            blowSet = null;

            if (selectedTest != 0 && selectedTest != -1)
            {
                Load(selectedTest);
            }
        }

        void StageChanged(int value)
        {
            testSpec.Stage = value;

            if (testSpec.Stage < 7)
            {
                testSpec.TenorWeight = 8;
                currentTenorWeightDisabled = true;
            }
            else if (testSpec.Stage > 8)
            {
                testSpec.TenorWeight = 23;
                currentTenorWeightDisabled = true;
            }
            else
            {
                currentTenorWeightDisabled = false;
            }

            selectTenorWeightDisabled = currentTenorWeightDisabled;

            // Need to recalculate BaseGap on a stage change
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, Constants.Rounding);
            screen.BaseGap = baseGap;

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void TenorWeightChanged(int value)
        {
            testSpec.TenorWeight = value;

            // Need to recalculate BaseGap on a tenor change
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, Constants.Rounding);
            screen.BaseGap = baseGap;

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void NumRowsChanged(int value)
        {
            testSpec.NumRows = value;

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void ErrorSizeChanged(int value)
        {
            testSpec.ErrorSize = value;

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void TestBellLocChanged(int value)
        {
            testSpec.TestBellLoc = value;

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void ShowGapsChanged(bool value)
        {
            showGaps = value;
        }

        void Create()
        {
            Block testBlock = new Block(testSpec.Stage, testSpec.NumRows);
            testBlock.CreateRandomBlock();

            // Set place to be the test place
            int testPlace;
            testPlace = testSpec.Stage + (testSpec.Stage % 2);

            if (testSpec.TestBellLoc != 1)
            {
                Random rand = new Random();
                testPlace = rand.Next(1, testPlace + 1);
            }

            blowSet = new BlowSet(testSpec.Stage, testSpec.NumRows, testSpec.TenorWeight, testSpec.ErrorType, true);

            // No need for an audio suffix in a Gap test (this is used to distinguish A and B in an A/B test)
            blowSet.PopulateBlows(testBlock, testPlace, string.Empty);
            blowSet.CreateRandomSpacing(testSpec.ErrorSize, Constants.Rounding);
            blowSet.SetUnstruck();

            showGaps = false;
            playLabel = "Play";
            controlsDisabled = false;
            selectTenorWeightDisabled = currentTenorWeightDisabled;
        }

        async void Load(int id)
        {
            // Get a test from the API
            GapTestData gapTestData = await Http.GetFromJsonAsync<GapTestData>("api/gaptests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowSetCore object
            BlowSetCore blowSetCore = JsonSerializer.Deserialize<BlowSetCore>(gapTestData.GapTestSpec);

            // Now create a BlowSet object from the BlowSetCore object
            blowSet = new BlowSet(blowSetCore.Stage, blowSetCore.NumRows, blowSetCore.TenorWeight,
                    blowSetCore.ErrorType, true);

            // No need for an audio suffix in a Gap test (this is used to distinguish A and B in an A/B test)
            blowSet.LoadBlows(blowSetCore, string.Empty);
            blowSet.SetUnstruck();

            // Update drop down boxes on screen
            testSpec.Stage = blowSet.Stage;
            testSpec.TenorWeight = blowSet.TenorWeight;
            testSpec.NumRows = blowSet.NumRows;

            // Update Screen.BaseGap - this varies by stage and tenorweight, and is used to shift backstroke rows
            // to the right to make the 1st blow of each row align vertically (when no striking errors)
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            screen.BaseGap = baseGap;

            showGaps = false;
            playLabel = "Play";
            controlsDisabled = false;
            selectTenorWeightDisabled = currentTenorWeightDisabled;
            StateHasChanged();
        }

        async void Save()
        {
            spinnerSaving = true;
            saveLabel = "Wait";
            controlsDisabled = true;
            playDisabled = true;
            blowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Push the created test to the API in JSON format
            // Start by creating a BlowSetCore object, which just has the parent data BlowSet
            // Note implicit cast from child to parent
            BlowSetCore blowSetCore = blowSet;

            // BlowSetCore has a BlowsCore list which is empty so far
            // Call the LoadBlowsCore method to populate it
            blowSetCore.LoadBlowsCore(blowSet);

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowSetData object
            var gapTestData = new GapTestData
            {
                GapTestSpec = JsonSerializer.Serialize(blowSetCore)
            };

            // Push the Json object to the API
            await Http.PostAsJsonAsync("api/gaptests", gapTestData);

            // Refresh the contents of the Select Test dropdown 
            gapTestsData = (await Http.GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();

            spinnerSaving = false;
            saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            saved = false;
            saveLabel = "Save";
            controlsDisabled = false;
            playDisabled = false;
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }
    }
}
