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
        BlowSet blowSet;

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

            screen = new Screen();
            screen.SelectedTest = -1;
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
            screen.SaveLabel = "Save";
            screen.PlayLabel = "Play";
            screen.SubmitLabel = "Submit";
            
            await PopulateBrowserDimensions();
            
            screen.DiameterScale = ScreenSizing.DiameterScale(browserWidth);
            screen.XScale = ScreenSizing.XScale(browserWidth);
            screen.XMargin = ScreenSizing.XMargin(browserWidth);
            screen.YScale = ScreenSizing.YScale(browserWidth);
            screen.YMargin = ScreenSizing.YMargin(browserWidth);
            screen.BorderWidth = ScreenSizing.BorderWidth(browserWidth);
            screen.FontSize = ScreenSizing.FontSize(browserWidth);
            screen.StrokeLabelXOffset = ScreenSizing.StrokeLabelXOffset(browserWidth);
            screen.StrokeLabelYOffset = ScreenSizing.StrokeLabelYOffset(browserWidth);
            screen.RowStartLabelWidth = ScreenSizing.RowStartLabelWidth(browserWidth);
            screen.RowStartLabelHeight = ScreenSizing.RowStartLabelHeight(browserWidth);
            screen.ChangeLabelXOffset = ScreenSizing.ChangeLabelXOffset(browserWidth);
            screen.ChangeLabelYOffset = ScreenSizing.ChangeLabelYOffset(browserWidth);
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        async Task TestChanged(int value)
        {
            screen.SelectedTest = value;
            blowSet = null;

            if (screen.SelectedTest != 0 && screen.SelectedTest != -1)
            {
                await Load(screen.SelectedTest);
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

            screen.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);

            if (blowSet != null)
            {
                blowSet = null;
            }
        }

        void TenorWeightChanged(int value)
        {
            testSpec.TenorWeight = value;

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
            screen.ShowGaps = value;
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

            // Set up test spec-dependent elements of the screen object
            // When practicing in a Gap Test, gaps are rounded to the nearest 10ms so that bells will align
            // if zero gap error is selected
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 10);
            screen.BaseGap = baseGap;
            screen.GapMin = 20;

            // If test bell is 1st's place of a handstroke row, need to adjust GapMax to have a higher value
            // because of the handstroke gap
            if (testSpec.NumRows % 2 == 1 && testPlace == 1)
            {
                screen.GapMax = Convert.ToInt32(Math.Round(((double)screen.BaseGap * 3) / 50)) * 50;
            }
            else
            {
                screen.GapMax = Convert.ToInt32(Math.Round(((double)baseGap * 2) / 50)) * 50;
            }
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

            // Set up test spec-dependent elements of the screen object
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            screen.BaseGap = baseGap;
            screen.GapMin = 20;

            // If test bell is 1st's place of a handstroke row, need to adjust GapMax to have a higher value
            // because of the handstroke gap
            if (blowSet.Blows.Last().IsHandstroke == true && blowSet.Blows.Last().Place == 1)
            {
                screen.GapMax = Convert.ToInt32(Math.Round(((double)screen.BaseGap * 3) / 50)) * 50;
            }
            else
            {
                screen.GapMax = Convert.ToInt32(Math.Round(((double)baseGap * 2) / 50)) * 50;
            }

            screen.ShowGaps = false;
            StateHasChanged();
        }

        async Task Save()
        {
            screen.SpinnerSaving = true;
            screen.SaveLabel = "Wait";
            screen.ControlsDisabled = true;
            screen.TenorWeightDisabled = true;
            screen.PlayDisabled = true;
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

            screen.SpinnerSaving = false;
            screen.Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            screen.Saved = false;
            screen.SaveLabel = "Save";
            screen.ControlsDisabled = false;
            screen.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screen.PlayDisabled = false;
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        async Task ProcessCallback(CallbackParam cbp)
        {
            switch (cbp)
            {
                case CallbackParam.GapMinus:
                    StateHasChanged();
                    break;

                case CallbackParam.GapPlus:
                    StateHasChanged();
                    break;

                case CallbackParam.MouseMove:
                    StateHasChanged();
                    break;

                case CallbackParam.Play:
                    if (Device.DeviceLoad == DeviceLoad.Low)
                    {
                        Play();
                    }
                    else
                    {
                        await PlayAsync();
                    }

                    break;

                case CallbackParam.Submit:
                    await Submit();
                    break;

                default:
                    break;
            }
        }

        async Task PlayAsync()
        {
            int initialDelay = 1000;
            
            if (screen.PlayLabel == "Play")
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

                screen.PlayLabel = "Stop";
                screen.ControlsDisabled = true;
                screen.TenorWeightDisabled = true;

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
            else if (screen.PlayLabel == "Stop")
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
            screen.PlayDisabled = true;
            screen.PlayLabel = "Wait";
            screen.SpinnerPlaying = true;
            StateHasChanged();

            // Wait 2.6 or 1.6 further seconds for the sound to finish, depending on whether arriving here
            // on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset play button
            screen.SpinnerPlaying = false;
            screen.PlayLabel = "Play";
            screen.PlayDisabled = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High)
            {
                blowSet.SetUnstruck();
            }
            else
            {
                blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            }
            
            screen.ControlsDisabled = false;
            screen.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
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

            screen.PlayDisabled = true;
            screen.PlayLabel = "Wait";
            screen.ControlsDisabled = true;
            screen.TenorWeightDisabled = true;

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
            screen.SpinnerPlaying = true;
            StateHasChanged();

            // Wait further 1.6 seconds for the sound to finish
            await Task.Delay(1600);

            // Reset play button
            screen.SpinnerPlaying = false;
            screen.PlayLabel = "Play";
            screen.PlayDisabled = false;

            // Reset screen
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            screen.ControlsDisabled = false;
            screen.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            StateHasChanged();
        }

        async Task Submit()
        {
            screen.SpinnerSubmitting = true;
            screen.SubmitLabel = "Wait";
            screen.ControlsDisabled = true;
            screen.PlayDisabled = true;
            blowSet.Blows.Last().BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "Gap Test",
                TestId = screen.SelectedTest,
                Gap = blowSet.Blows.Last().Gap,
                AB = string.Empty
            };

            // Push the testSubmission to the API in JSON format
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/testsubmissions", testSubmission);

            screen.SpinnerSubmitting = false;
            screen.Submitted = true;
            StateHasChanged();

            await Task.Delay(1000);

            screen.Submitted = false;
            screen.SubmitLabel = "Submit";
            screen.ControlsDisabled = false;
            screen.PlayDisabled = false;
            blowSet.Blows.Last().BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        void ArrowKeys(KeyboardEventArgs e)
        {
            // Keyboard arrows only active when a blowset is populated and when in Play mode
            if (blowSet != null && screen.PlayLabel == "Play")
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
