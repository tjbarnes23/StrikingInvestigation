using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class GapTest
    {
        protected ElementReference mainDiv;

        public GapTest()
        {
            TestSpec = new TestSpec();
            TestSpec.Stage = 8;
            TestSpec.TenorWeight = 23;
            TestSpec.NumRows = 4;
            TestSpec.ErrorType = 0;
            TestSpec.ErrorSize = 80;
            TestSpec.TestBellLoc = 1;

            // In a Gap Test, gaps are rounded to the nearest 10ms
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, Constants.Rounding);

            Screen = new Screen();
            Screen.DiameterScale = Constants.DiameterScale;
            Screen.XScale = Constants.XScale;
            Screen.XMargin = Constants.XMargin;
            Screen.YScale = Constants.YScale;
            Screen.YMargin = Constants.YMargin;
            Screen.GapMin = 20;
            Screen.GapMax = 700;
            Screen.BaseGap = baseGap;
        }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        public TestSpec TestSpec { get; set; }

        public IEnumerable<GapTestData> GapTestsData { get; set; }

        public string SelectedTest { get; set; } = "Select test";

        public bool ShowGaps { get; set; }

        public BlowSet BlowSet { get; set; }

        public Screen Screen { get; set; }

        public string PlayLabel { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool ControlsDisabled { get; set; }

        public bool PlayDisabled { get; set; }

        public bool SelectTenorWeightDisabled { get; set; }

        public bool CurrentTenorWeightDisabled { get; set; }

        public bool Saving { get; set; }

        public bool Saved { get; set; }

        protected override async Task OnInitializedAsync()
        {
            GapTestsData = (await Http.GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        protected void SetState(ScreenState screenState)
        {
            if (screenState == ScreenState.Play)
            {
                PlayLabel = "Play";
                ControlsDisabled = false;
                SelectTenorWeightDisabled = CurrentTenorWeightDisabled;
            }
            else if (screenState == ScreenState.Stop)
            {
                PlayLabel = "Stop";
                ControlsDisabled = true;
                SelectTenorWeightDisabled = true;
            }
        }

        protected void TestChanged(string value)
        {
            SelectedTest = value;
            BlowSet = null;

            if (SelectedTest != "Random" && SelectedTest != "Select test")
            {
                Load(SelectedTest);
            }
        }

        protected void StageChanged(int value)
        {
            TestSpec.Stage = value;

            if (TestSpec.Stage < 7)
            {
                TestSpec.TenorWeight = 8;
                CurrentTenorWeightDisabled = true;
            }
            else if (TestSpec.Stage > 8)
            {
                TestSpec.TenorWeight = 23;
                CurrentTenorWeightDisabled = true;
            }
            else
            {
                CurrentTenorWeightDisabled = false;
            }

            SelectTenorWeightDisabled = CurrentTenorWeightDisabled;

            // Need to recalculate BaseGap on a stage change
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, Constants.Rounding);
            Screen.BaseGap = baseGap;

            if (BlowSet != null)
            {
                BlowSet = null;
            }
        }

        protected void TenorWeightChanged(int value)
        {
            TestSpec.TenorWeight = value;

            // Need to recalculate BaseGap on a tenor change
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, Constants.Rounding);
            Screen.BaseGap = baseGap;

            if (BlowSet != null)
            {
                BlowSet = null;
            }
        }

        protected void NumRowsChanged(int value)
        {
            TestSpec.NumRows = value;

            if (BlowSet != null)
            {
                BlowSet = null;
            }
        }

        protected void ErrorSizeChanged(int value)
        {
            TestSpec.ErrorSize = value;

            if (BlowSet != null)
            {
                BlowSet = null;
            }
        }

        protected void TestBellLocChanged(int value)
        {
            TestSpec.TestBellLoc = value;

            if (BlowSet != null)
            {
                BlowSet = null;
            }
        }

        protected void ShowGapsChanged(bool value)
        {
            ShowGaps = value;
        }

        protected void Create()
        {
            Block testBlock = new Block(TestSpec.Stage, TestSpec.NumRows);
            testBlock.CreateRandomBlock();

            // Set place to be the test place
            int testPlace;
            testPlace = TestSpec.Stage + (TestSpec.Stage % 2);

            if (TestSpec.TestBellLoc != 1)
            {
                Random rand = new Random();
                testPlace = rand.Next(1, testPlace + 1);
            }

            BlowSet = new BlowSet(TestSpec.Stage, TestSpec.NumRows, TestSpec.TenorWeight, TestSpec.ErrorType, true);

            // No need for an audio suffix in a Gap test (this is used to distinguish A and B in an A/B test)
            BlowSet.PopulateBlows(testBlock, testPlace, string.Empty);
            BlowSet.CreateRandomSpacing(TestSpec.ErrorSize, Constants.Rounding);
            BlowSet.SetUnstruck();

            ShowGaps = false;
            SetState(ScreenState.Play);
        }

        protected async void Load(string id)
        {
            // Get a test from the API
            GapTestData gapTestData = await Http.GetFromJsonAsync<GapTestData>("api/gaptests/" + id);

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowSetCore object
            BlowSetCore blowSetCore = JsonSerializer.Deserialize<BlowSetCore>(gapTestData.GapTestSpec);

            // Now create a BlowSet object from the BlowSetCore object
            BlowSet = new BlowSet(blowSetCore.Stage, blowSetCore.NumRows, blowSetCore.TenorWeight,
                    blowSetCore.ErrorType, true);

            // No need for an audio suffix in a Gap test (this is used to distinguish A and B in an A/B test)
            BlowSet.LoadBlows(blowSetCore, string.Empty);
            BlowSet.SetUnstruck();

            // Update drop down boxes on screen
            TestSpec.Stage = BlowSet.Stage;
            TestSpec.TenorWeight = BlowSet.TenorWeight;
            TestSpec.NumRows = BlowSet.NumRows;

            // Update Screen.BaseGap - this varies by stage and tenorweight, and is used to shift backstroke rows
            // to the right to make the 1st blow of each row align vertically (when no striking errors)
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, 1);
            Screen.BaseGap = baseGap;

            ShowGaps = false;
            SetState(ScreenState.Play);
            StateHasChanged();
        }

        protected async void Save()
        {
            Saving = true;

            // Push the created test to the API in JSON format
            // Start by creating a BlowSetCore object, which just has the parent data BlowSet
            // Note implicit cast from child to parent
            BlowSetCore blowSetCore = BlowSet;

            // BlowSetCore has a BlowsCore list which is empty so far
            // Call the LoadBlowsCore method to populate it
            blowSetCore.LoadBlowsCore(BlowSet);

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowSetData object
            GapTestData gapTestData = new GapTestData();
            gapTestData.GapTestSpec = JsonSerializer.Serialize(blowSetCore);

            // Push the Json object to the API
            await Http.PostAsJsonAsync("api/gaptests", gapTestData);

            // Refresh the contents of the Select Test dropdown 
            GapTestsData = (await Http.GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();

            Saving = false;
            Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            Saved = false;
            StateHasChanged();
        }

        protected async Task Play()
        {
            if (PlayLabel == "Play")
            {
                // Change test bell color to disabled color - can't adjust gap during play
                BlowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;

                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in BlowSet.Blows)
                {
                    // Set sound times and display times
                    strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                    blow.StrikeTime = strikeTime;

                    // Add a delay after sound time to display the strike
                    DateTime altStrikeTime = strikeTime.AddMilliseconds(Constants.DisplayDelay);
                    blow.AltStrikeTime = altStrikeTime;
                }

                CancellationTokenSource = new CancellationTokenSource();
                CancellationToken = CancellationTokenSource.Token;

                SetState(ScreenState.Stop);
                await Strike();

            }
            else if (PlayLabel == "Stop")
            {
                PlayDisabled = true;
                
                CancellationTokenSource.Cancel();

                // Wait for 2.6 seconds for the sound to finish
                await Task.Delay(2600);

                PlayDisabled = false;
            }

            // Set colors to unstruck
            BlowSet.SetUnstruck();

            SetState(ScreenState.Play);
        }
        
        public async Task Strike()
        {
            foreach (Blow blow in BlowSet.Blows)
            {
                TimeSpan delay;
                int delayMs;

                delay = blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs, CancellationToken);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Strike bell
                await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);

                /*
                // Change color of bell on screen
                delay = blow.AltStrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs, CancellationToken);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }
                */

                // Change bell color
                blow.BellColor = Constants.StruckBellColor;
                StateHasChanged();
            }

            // Wait for 2.6 seconds for the sound to finish
            await Task.Delay(2600, CancellationToken);
        }

        protected void GapChangedWithButton(bool clicked)
        {
            if (clicked == true)
            {
                StateHasChanged();
            }
        }

        protected void TestBellMouseDown(MouseEventArgs e)
        {
            if (e.Buttons == 1)
            {
                // Mouse movement only active in Play mode
                if (PlayLabel == "Play")
                {
                    // Call TestBellMouseMove to center the bell on where the mouse is clicked
                    TestBellMouseMove(e);
                }
            }
        }

        protected void TestBellMouseMove(MouseEventArgs e)
        {
            if (e.Buttons == 1 && PlayLabel == "Play")
            {
                int clientX = Convert.ToInt32(e.ClientX);

                int newGapCumulativeRow;

                if (BlowSet.Blows.Last().IsHandstroke)
                {
                    newGapCumulativeRow = Convert.ToInt32((clientX - Screen.XMargin) / Screen.XScale);
                }
                else
                {
                    int baseGap = Screen.BaseGap;
                    newGapCumulativeRow = Convert.ToInt32((clientX - Screen.XMargin) / Screen.XScale) -
                             baseGap;
                }

                int newGap = BlowSet.Blows.Last().Gap + (newGapCumulativeRow - BlowSet.Blows.Last().GapCumulativeRow);
                int newGapRounded = Convert.ToInt32((double)newGap / Constants.Rounding) * Constants.Rounding;
                
                if (newGapRounded >= Screen.GapMin && newGapRounded <= Screen.GapMax &&
                        newGapRounded != BlowSet.Blows.Last().Gap)
                {
                    BlowSet.Blows.Last().UpdateGap(newGapRounded);
                }
            }
        }
        
        protected void ArrowKeys(KeyboardEventArgs e)
        {
            // Keyboard arrows only active in Play mode
            if (PlayLabel == "Play")
            {
                if (e.Key == "ArrowLeft")
                {
                    int newGap = BlowSet.Blows.Last().Gap - Constants.Rounding;

                    if (newGap >= Screen.GapMin)
                    {
                        BlowSet.Blows.Last().UpdateGap(newGap);
                    }
                }
                else if (e.Key == "ArrowRight")
                {
                    int newGap = BlowSet.Blows.Last().Gap + Constants.Rounding;

                    if (newGap <= Screen.GapMax)
                    {
                        BlowSet.Blows.Last().UpdateGap(newGap);
                    }
                }
            }
        }
    }
}
