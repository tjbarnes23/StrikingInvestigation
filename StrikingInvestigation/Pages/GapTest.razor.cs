using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<GapTestData> gapTestsData;
        readonly TestSpec testSpec;
        readonly Screen screen;
        int selectedTest;

        bool showGaps;

        BlowSet blowSet;

        bool controlsDisabled;
        bool tenorWeightDisabled;
        bool playDisabled;

        string saveLabel;
        string playLabel;
        string submitLabel;

        bool spinnerSaving;
        bool spinnerPlaying;
        bool spinnerSubmitting;

        bool saved;
        bool submitted;

        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        ElementReference mainDiv;

        int browserWidth;
        int browserHeight;

        public GapTest()
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

            selectedTest = -1;
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        Device Device { get; set; }

        [Inject]
        TJBarnesService TJBarnesService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            gapTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();
            saveLabel = "Save";
            playLabel = "Play";
            submitLabel = "Submit";
            await PopulateBrowserDimensions();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        async Task TestChanged(int value)
        {
            selectedTest = value;
            blowSet = null;

            if (selectedTest != 0 && selectedTest != -1)
            {
                await Load(selectedTest);
            }
        }

        void StageChanged(int value)
        {
            testSpec.Stage = value;

            if (testSpec.Stage < 7)
            {
                testSpec.TenorWeight = 8;
            }
            else if (testSpec.Stage > 8)
            {
                testSpec.TenorWeight = 23;
            }

            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);

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
        }

        async Task Load(int id)
        {
            // Get a test from the API
            GapTestData gapTestData = await TJBarnesService.GetHttpClient().
                    GetFromJsonAsync<GapTestData>("api/gaptests/" + id.ToString());

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
            StateHasChanged();
        }

        async Task Save()
        {
            spinnerSaving = true;
            saveLabel = "Wait";
            controlsDisabled = true;
            tenorWeightDisabled = true;
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
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/gaptests", gapTestData);

            // Refresh the contents of the Select Test dropdown 
            gapTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<GapTestData[]>("api/gaptests")).ToList();

            spinnerSaving = false;
            saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            saved = false;
            saveLabel = "Save";
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabled = false;
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        async Task PlayAsync()
        {
            int initialDelay = 1000;
            
            if (playLabel == "Play")
            {
                // Change test bell color to disabled color - can't adjust gap during play
                blowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;

                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in blowSet.Blows)
                {
                    // Set strike times
                    strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                    blow.StrikeTime = strikeTime;
                }

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                playLabel = "Stop";
                controlsDisabled = true;
                tenorWeightDisabled = true;

                TimeSpan delay;
                int delayMs;

                foreach (Blow blow in blowSet.Blows)
                {
                    delay = blow.StrikeTime - DateTime.Now;
                    delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // Change bell color
                    if (Device.DeviceLoad == DeviceLoad.High)
                    {
                        blow.BellColor = Constants.StruckBellColor;

                        // Confirmed this is needed here
                        StateHasChanged();
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
                }
            }
            else if (playLabel == "Stop")
            {
                cancellationTokenSource.Cancel();
                initialDelay = 0;
            }

            // Initial delay
            if (initialDelay > 0)
            {
                await Task.Delay(initialDelay);
            }

            // Start spinner
            playDisabled = true;
            playLabel = "Wait";
            spinnerPlaying = true;
            StateHasChanged();

            // Wait 2.6 or 1.6 further seconds for the sound to finish, depending on whether arriving here
            // on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset play button
            spinnerPlaying = false;
            playLabel = "Play";
            playDisabled = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High)
            {
                blowSet.SetUnstruck();
            }
            else
            {
                blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            }
            
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            StateHasChanged();
        }

        async void Play()
        {
            // Change test bell color to disabled color - can't adjust gap during play
            blowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;

            DateTime strikeTime = DateTime.Now;

            foreach (Blow blow in blowSet.Blows)
            {
                // Set strike times
                strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                blow.StrikeTime = strikeTime;
            }

            playDisabled = true;
            playLabel = "Wait";
            controlsDisabled = true;
            tenorWeightDisabled = true;

            TimeSpan delay;
            int delayMs;

            foreach (Blow blow in blowSet.Blows)
            {
                delay = blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }

                // Strike bell
                await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
            }
            
            // Initial delay
            await Task.Delay(1000);

            // Start spinner
            spinnerPlaying = true;
            StateHasChanged();

            // Wait further 1.6 seconds for the sound to finish
            await Task.Delay(1600);

            // Reset play button
            spinnerPlaying = false;
            playLabel = "Play";
            playDisabled = false;

            // Reset screen
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            StateHasChanged();
        }

        async Task Submit()
        {
            spinnerSubmitting = true;
            submitLabel = "Wait";
            controlsDisabled = true;
            playDisabled = true;
            blowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "Gap Test",
                TestId = selectedTest,
                Gap = blowSet.Blows.Last().Gap,
                AB = string.Empty
            };

            // Push the testSubmission to the API in JSON format
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/testsubmissions", testSubmission);

            spinnerSubmitting = false;
            submitted = true;
            StateHasChanged();

            await Task.Delay(1000);

            submitted = false;
            submitLabel = "Submit";
            controlsDisabled = false;
            playDisabled = false;
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        void GapChangedWithButton(bool clicked)
        {
            if (clicked == true)
            {
                StateHasChanged();
            }
        }

        void TestBellMouseDown(MouseEventArgs e)
        {
            if (e.Buttons == 1)
            {
                // Mouse movement only active in Play mode
                if (playLabel == "Play")
                {
                    // Call TestBellMouseMove to center the bell on where the mouse is clicked
                    TestBellMouseMove(e);
                }
            }
        }

        void TestBellMouseMove(MouseEventArgs e)
        {
            if (e.Buttons == 1 && playLabel == "Play")
            {
                int clientX = Convert.ToInt32(e.ClientX);

                int newGapCumulativeRow;

                if (blowSet.Blows.Last().IsHandstroke)
                {
                    newGapCumulativeRow = Convert.ToInt32((clientX - screen.XMargin) / screen.XScale);
                }
                else
                {
                    int baseGap = screen.BaseGap;
                    newGapCumulativeRow = Convert.ToInt32((clientX - screen.XMargin) / screen.XScale) -
                             baseGap;
                }

                int newGap = blowSet.Blows.Last().Gap + (newGapCumulativeRow - blowSet.Blows.Last().GapCumulativeRow);
                int newGapRounded = Convert.ToInt32((double)newGap / Constants.Rounding) * Constants.Rounding;

                if (newGapRounded >= screen.GapMin && newGapRounded <= screen.GapMax &&
                        newGapRounded != blowSet.Blows.Last().Gap)
                {
                    blowSet.Blows.Last().UpdateGap(newGapRounded);
                }
            }
        }

        void ArrowKeys(KeyboardEventArgs e)
        {
            // Keyboard arrows only active in Play mode
            if (playLabel == "Play")
            {
                if (e.Key == "ArrowLeft")
                {
                    int newGap = blowSet.Blows.Last().Gap - Constants.Rounding;

                    if (newGap >= screen.GapMin)
                    {
                        blowSet.Blows.Last().UpdateGap(newGap);
                    }
                }
                else if (e.Key == "ArrowRight")
                {
                    int newGap = blowSet.Blows.Last().Gap + Constants.Rounding;

                    if (newGap <= screen.GapMax)
                    {
                        blowSet.Blows.Last().UpdateGap(newGap);
                    }
                }
            }
        }

        async Task PopulateBrowserDimensions()
        {
            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            browserWidth = browserDimensions.Width;
            browserHeight = browserDimensions.Height;
        }
    }
}
