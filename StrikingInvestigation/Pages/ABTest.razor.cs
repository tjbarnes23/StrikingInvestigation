using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class ABTest
    {
        IEnumerable<ABTestData> aBTestsData;
        readonly TestSpec testSpec;
        readonly Screen screenA;
        readonly Screen screenB;
        BlowSet blowSetA;
        BlowSet blowSetB;

        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        public ABTest()
        {
            testSpec = new TestSpec
            {
                Stage = 8,
                TenorWeight = 23,
                NumRows = 4,
                ErrorType = 1,
                ErrorSize = 60,
                TestBellLoc = 0, // Indicates no test bell
                SelectedTest = -1
            };

            screenA = new Screen();
            screenB = new Screen();
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        Device Device { get; set; }

        [Inject]
        TJBarnesService TJBarnesService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            aBTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<ABTestData[]>("api/abtests")).ToList();

            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            testSpec.BrowserWidth = browserDimensions.Width;
            testSpec.BrowserHeight = browserDimensions.Height;
            testSpec.DeviceLoad = Device.DeviceLoad;

            testSpec.SaveLabel = "Save";

            testSpec.DiameterScale = ScreenSizing.DiameterScale(testSpec.BrowserWidth);
            testSpec.XScale = ScreenSizing.XScale(testSpec.BrowserWidth);
            testSpec.YScale = ScreenSizing.YScale(testSpec.BrowserWidth);
            testSpec.BorderWidth = ScreenSizing.BorderWidth(testSpec.BrowserWidth);
            testSpec.FontSize = ScreenSizing.FontSize(testSpec.BrowserWidth);
            testSpec.StrokeLabelXOffset = ScreenSizing.StrokeLabelXOffset(testSpec.BrowserWidth);
            testSpec.StrokeLabelYOffset = ScreenSizing.StrokeLabelYOffset(testSpec.BrowserWidth);
            testSpec.RowStartLabelWidth = ScreenSizing.RowStartLabelWidth(testSpec.BrowserWidth);
            testSpec.RowStartLabelHeight = ScreenSizing.RowStartLabelHeight(testSpec.BrowserWidth);
            testSpec.ChangeLabelXOffset = ScreenSizing.ChangeLabelXOffset(testSpec.BrowserWidth);
            testSpec.ChangeLabelYOffset = ScreenSizing.ChangeLabelYOffset(testSpec.BrowserWidth);

            testSpec.SubmitLabel1 = "A has errors";
            testSpec.SubmitLabel2 = "B has errors";
            testSpec.SubmitLabel3 = "I can't tell which has errors";

            screenA.IsA = true;
            screenA.XMargin = ScreenSizing.XMargin(testSpec.BrowserWidth);
            screenA.YMargin = ScreenSizing.YMargin(testSpec.BrowserWidth);
            screenA.PlayLabel = "Play A";

            screenB.IsA = false;
            screenB.XMargin = ScreenSizing.XMargin(testSpec.BrowserWidth);
            screenB.YMargin = ScreenSizing.YMarginB(testSpec.BrowserWidth);
            screenB.PlayLabel = "Play B";
        }

        async Task TestChanged(int value)
        {
            testSpec.SelectedTest = value;
            blowSetA = null;
            blowSetB = null;

            if (testSpec.SelectedTest != 0 && testSpec.SelectedTest != -1)
            {
                await Load(testSpec.SelectedTest);
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

            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);

            if (blowSetA != null)
            {
                blowSetA = null;
            }

            if (blowSetB != null)
            {
                blowSetB = null;
            }
        }

        void TenorWeightChanged(int value)
        {
            testSpec.TenorWeight = value;

            if (blowSetA != null)
            {
                blowSetA = null;
            }

            if (blowSetB != null)
            {
                blowSetB = null;
            }
        }

        void ErrorTypeChanged(int value)
        {
            testSpec.ErrorType = value;

            if (blowSetA != null)
            {
                blowSetA = null;
            }

            if (blowSetB != null)
            {
                blowSetB = null;
            }
        }

        void ErrorSizeChanged(int value)
        {
            testSpec.ErrorSize = value;

            if (blowSetA != null)
            {
                blowSetA = null;
            }

            if (blowSetB != null)
            {
                blowSetB = null;
            }
        }

        void ShowGapsChanged(bool value)
        {
            testSpec.ShowGaps = value;
        }

        void Create()
        {
            // Choose whether A or B will have the errors
            Random rand = new Random();
            testSpec.AHasErrors = rand.Next(0, 2) == 0;

            // Create the test block
            Block testBlock = new Block(testSpec.Stage, testSpec.NumRows);
            testBlock.CreateRandomBlock();

            blowSetA = new BlowSet(testSpec.Stage, testSpec.NumRows, testSpec.TenorWeight,
                    testSpec.ErrorType, testSpec.AHasErrors);
            blowSetA.PopulateBlows(testBlock, testSpec.TestBellLoc, "a");

            // No rounding in an A/B test
            blowSetA.CreateEvenSpacing(1);
            blowSetA.SetUnstruck();

            blowSetB = new BlowSet(testSpec.Stage, testSpec.NumRows, testSpec.TenorWeight,
                    testSpec.ErrorType, !testSpec.AHasErrors);
            blowSetB.PopulateBlows(testBlock, testSpec.TestBellLoc, "b");

            // No rounding in an A/B test
            blowSetB.CreateEvenSpacing(1);
            blowSetB.SetUnstruck();

            if (testSpec.AHasErrors == true)
            {
                if (testSpec.ErrorType == 1)
                {
                    blowSetA.CreateStrikingError(testSpec.ErrorSize);
                }
                else
                {
                    blowSetA.CreateCompassError(testSpec.ErrorSize);
                }
            }
            else
            {
                if (testSpec.ErrorType == 1)
                {
                    blowSetB.CreateStrikingError(testSpec.ErrorSize);
                }
                else
                {
                    blowSetB.CreateCompassError(testSpec.ErrorSize);
                }
            }

            // Set up test spec-dependent elements of the screen object
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            testSpec.BaseGap = baseGap;

            // Set the timing for the animation (when not showing the bells)
            screenA.AnimationDuration = blowSetA.Blows.Last().GapCumulative + 1000;
            screenB.AnimationDuration = blowSetB.Blows.Last().GapCumulative + 1000;
            testSpec.ResultEntered = false;

            testSpec.ShowGaps = false;
        }

        async Task Load(int id)
        {
            // Get a test from the API
            ABTestData aBTestData = await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<ABTestData>("api/abtests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowSetCore object for each of A and B
            BlowSetCore blowSetCoreA = JsonSerializer.Deserialize<BlowSetCore>(aBTestData.ABTestSpecA);
            BlowSetCore blowSetCoreB = JsonSerializer.Deserialize<BlowSetCore>(aBTestData.ABTestSpecB);

            testSpec.AHasErrors = blowSetCoreA.HasErrors;

            // Create a BlowSet object from the BlowSetCore object for A
            blowSetA = new BlowSet(blowSetCoreA.Stage, blowSetCoreA.NumRows, blowSetCoreA.TenorWeight,
                    blowSetCoreA.ErrorType, blowSetCoreA.HasErrors);

            // Use an audioIdSuffix of "b" for this blowset
            blowSetA.LoadBlows(blowSetCoreA, "a");
            
            blowSetA.SetUnstruck();

            // Create a BlowSet object from the BlowSetCore object for B
            blowSetB = new BlowSet(blowSetCoreB.Stage, blowSetCoreB.NumRows, blowSetCoreB.TenorWeight,
                    blowSetCoreB.ErrorType, blowSetCoreB.HasErrors);
            
            // Use an audioIdSuffix of "b" for this blowset
            blowSetB.LoadBlows(blowSetCoreB, "b");

            blowSetB.SetUnstruck();

            // Update drop down boxes on screen
            // Use BlowSetA - by definition the following properties for BlowSetA and BlowSetB will be the same
            testSpec.Stage = blowSetA.Stage;
            testSpec.TenorWeight = blowSetA.TenorWeight;
            testSpec.ErrorType = blowSetA.ErrorType;

            // Set up test spec-dependent elements of the screen object
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            testSpec.BaseGap = baseGap;

            // Set the timing for the animation (when not showing the bells)
            screenA.AnimationDuration = blowSetA.Blows.Last().GapCumulative + 1000;
            screenB.AnimationDuration = blowSetB.Blows.Last().GapCumulative + 1000;
            testSpec.ResultEntered = false;

            testSpec.ShowGaps = false;
            StateHasChanged();
        }

        async Task Save()
        {
            testSpec.SpinnerSaving = true;
            testSpec.SaveLabel = "Wait";
            testSpec.ControlsDisabled = true;
            testSpec.TenorWeightDisabled = true;
            screenA.PlayDisabled = true;
            screenB.PlayDisabled = true;
            StateHasChanged();

            // Push the created test to the API in JSON format
            // Start by creating a BlowSetCore object, which just has the parent data BlowSet, for each of A and B
            // Note implicit cast from child to parent
            BlowSetCore blowSetCoreA = blowSetA;
            BlowSetCore blowSetCoreB = blowSetB;

            // BlowSetCore has a BlowsCore list which is empty so far
            // Call the LoadBlowsCore method to populate it for each of A and B
            blowSetCoreA.LoadBlowsCore(blowSetA);
            blowSetCoreB.LoadBlowsCore(blowSetB);

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowSetData object for each of A and B
            ABTestData aBTestData = new ABTestData
            {
                ABTestSpecA = JsonSerializer.Serialize(blowSetCoreA),
                ABTestSpecB = JsonSerializer.Serialize(blowSetCoreB)
            };

            // Push the Json object to the API
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/abtests", aBTestData);

            // Refresh the contents of the Select Test dropdown 
            aBTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<ABTestData[]>("api/abtests")).ToList();

            testSpec.SpinnerSaving = false;
            testSpec.Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            testSpec.Saved = false;
            testSpec.SaveLabel = "Save";
            testSpec.ControlsDisabled = false;
            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screenA.PlayDisabled = false;
            screenB.PlayDisabled = false;
            StateHasChanged();
        }

        async Task ProcessCallback(CallbackParam callbackParam)
        {
            switch (callbackParam)
            {
                case CallbackParam.PlayAsyncA:
                    await PlayAsyncA();
                    break;

                case CallbackParam.PlayA:
                    PlayA();
                    break;

                case CallbackParam.PlayAsyncB:
                    await PlayAsyncB();
                    break;

                case CallbackParam.PlayB:
                    PlayB();
                    break;

                case CallbackParam.AHasErrors:
                case CallbackParam.BHasErrors:
                case CallbackParam.DontKnow:
                    await Submit(callbackParam);
                    break;

                default:
                    break;
            }
        }

        async Task PlayAsyncA()
        {
            int initialDelay = 1000;

            if (screenA.PlayLabel == "Play A")
            {
                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in blowSetA.Blows)
                {
                    // Set strike times
                    strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                    blow.StrikeTime = strikeTime;
                }

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                screenA.PlayLabel = "Stop A";
                testSpec.ControlsDisabled = true;
                testSpec.TenorWeightDisabled = true;
                screenB.PlayDisabled = true;

                // Start the animation if not showing the bells
                if (testSpec.ShowGaps == false)
                {
                    screenA.RunAnimation = true;
                }

                TimeSpan delay;
                int delayMs;

                foreach (Blow blow in blowSetA.Blows)
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
                    if (Device.DeviceLoad == DeviceLoad.High && testSpec.ShowGaps == true)
                    {
                        blow.BellColor = Constants.StruckBellColor;

                        // Confirmed this is needed here
                        StateHasChanged();
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
                }
            }
            else if (screenA.PlayLabel == "Stop A")
            {
                cancellationTokenSource.Cancel();
                initialDelay = 0;
            }

            // Initial delay
            if (initialDelay > 0)
            {
                // Test was not stopped
                await Task.Delay(initialDelay);
            }
            else
            {
                // Reset animation immediately when test is stopped
                if (testSpec.ShowGaps == false)
                {
                    screenA.RunAnimation = false;
                }
            }

            // Start spinner
            screenA.PlayDisabled = true;
            screenA.PlayLabel = "Wait";
            screenA.SpinnerPlaying = true;
            StateHasChanged();

            // Wait 2.6 or 1.6 further seconds for the sound to finish, depending on whether arriving here
            // on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset animation, unless it was already reset earlier after a stop
            if (initialDelay > 0 && testSpec.ShowGaps == false)
            {
                screenA.RunAnimation = false;
            }

            // Reset play button
            screenA.SpinnerPlaying = false;
            screenA.PlayLabel = "Play A";
            screenA.PlayDisabled = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High && testSpec.ShowGaps == true)
            {
                blowSetA.SetUnstruck();
            }

            testSpec.ControlsDisabled = false;
            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screenB.PlayDisabled = false;
            StateHasChanged();
        }

        async Task PlayAsyncB()
        {
            int initialDelay = 1000;

            if (screenB.PlayLabel == "Play B")
            {
                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in blowSetB.Blows)
                {
                    // Set strike times
                    strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                    blow.StrikeTime = strikeTime;
                }

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                screenB.PlayLabel = "Stop B";
                testSpec.ControlsDisabled = true;
                testSpec.TenorWeightDisabled = true;
                screenA.PlayDisabled = true;

                // Start the animation if not showing the bells
                if (testSpec.ShowGaps == false)
                {
                    screenB.RunAnimation = true;
                }

                TimeSpan delay;
                int delayMs;

                foreach (Blow blow in blowSetB.Blows)
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
                    if (Device.DeviceLoad == DeviceLoad.High && testSpec.ShowGaps == true)
                    {
                        blow.BellColor = Constants.StruckBellColor;

                        // Confirmed this is needed here
                        StateHasChanged();
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
                }
            }
            else if (screenB.PlayLabel == "Stop B")
            {
                cancellationTokenSource.Cancel();
                initialDelay = 0;
            }

            // Initial delay
            if (initialDelay > 0)
            {
                // Test was not stopped
                await Task.Delay(initialDelay);
            }
            else
            {
                // Reset animation immediately when test is stopped
                if (testSpec.ShowGaps == false)
                {
                    screenB.RunAnimation = false;
                }
            }

            // Start spinner
            screenB.PlayDisabled = true;
            screenB.PlayLabel = "Wait";
            screenB.SpinnerPlaying = true;
            StateHasChanged();

            // Wait 2.6 or 1.6 further seconds for the sound to finish, depending on whether arriving here
            // on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset animation, unless it was already reset earlier after a stop
            if (initialDelay > 0 && testSpec.ShowGaps == false)
            {
                screenB.RunAnimation = false;
            }

            // Reset play button
            screenB.SpinnerPlaying = false;
            screenB.PlayLabel = "Play B";
            screenB.PlayDisabled = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High && testSpec.ShowGaps == true)
            {
                blowSetB.SetUnstruck();
            }

            testSpec.ControlsDisabled = false;
            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screenA.PlayDisabled = false;
            StateHasChanged();
        }

        async void PlayA()
        {
            DateTime strikeTime = DateTime.Now;

            foreach (Blow blow in blowSetA.Blows)
            {
                // Set strike times
                strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                blow.StrikeTime = strikeTime;
            }

            screenA.PlayDisabled = true;
            screenA.PlayLabel = "Wait";
            testSpec.ControlsDisabled = true;
            testSpec.TenorWeightDisabled = true;
            screenB.PlayDisabled = true;

            // Start the animation if not showing the bells
            if (testSpec.ShowGaps == false)
            {
                screenA.RunAnimation = true;
            }

            TimeSpan delay;
            int delayMs;

            foreach (Blow blow in blowSetA.Blows)
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
            screenA.SpinnerPlaying = true;
            StateHasChanged();

            // Wait 1.6 further seconds for the sound to finish
            await Task.Delay(1600);

            // Reset animation
            if (testSpec.ShowGaps == false)
            {
                screenA.RunAnimation = false;
            }

            // Reset play button
            screenA.SpinnerPlaying = false;
            screenA.PlayLabel = "Play A";
            screenA.PlayDisabled = false;

            // Reset screen
            testSpec.ControlsDisabled = false;
            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screenB.PlayDisabled = false;
            StateHasChanged();
        }

        async void PlayB()
        {
            DateTime strikeTime = DateTime.Now;

            foreach (Blow blow in blowSetB.Blows)
            {
                // Set strike times
                strikeTime = strikeTime.AddMilliseconds(blow.Gap);
                blow.StrikeTime = strikeTime;
            }

            screenB.PlayDisabled = true;
            screenB.PlayLabel = "Wait";
            testSpec.ControlsDisabled = true;
            testSpec.TenorWeightDisabled = true;
            screenA.PlayDisabled = true;

            // Start the animation if not showing the bells
            if (testSpec.ShowGaps == false)
            {
                screenB.RunAnimation = true;
            }

            TimeSpan delay;
            int delayMs;

            foreach (Blow blow in blowSetB.Blows)
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
            screenB.SpinnerPlaying = true;
            StateHasChanged();

            // Wait 1.6 further seconds for the sound to finish
            await Task.Delay(1600);

            // Reset animation
            if (testSpec.ShowGaps == false)
            {
                screenB.RunAnimation = false;
            }

            // Reset play button
            screenB.SpinnerPlaying = false;
            screenB.PlayLabel = "Play B";
            screenB.PlayDisabled = false;

            // Reset screen
            testSpec.ControlsDisabled = false;
            testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            screenA.PlayDisabled = false;
            StateHasChanged();
        }

        async Task Submit(CallbackParam callbackParam)
        {
            if (testSpec.SelectedTest != 0 && testSpec.SelectedTest != -1)
            {
                ABResult aBResult;

                // Set the applicable submit button to Wait with a spinner
                switch (callbackParam)
                {
                    case CallbackParam.AHasErrors:
                        testSpec.SpinnerSubmitting1 = true;
                        testSpec.SubmitLabel1 = "Wait";
                        aBResult = ABResult.AHasErrors;
                        break;

                    case CallbackParam.BHasErrors:
                        testSpec.SpinnerSubmitting2 = true;
                        testSpec.SubmitLabel2 = "Wait";
                        aBResult = ABResult.BHasErrors;
                        break;

                    case CallbackParam.DontKnow:
                        testSpec.SpinnerSubmitting3 = true;
                        testSpec.SubmitLabel3 = "Wait";
                        aBResult = ABResult.DontKnow;
                        break;

                    default:
                        aBResult = ABResult.DontKnow;
                        break;
                }

                testSpec.ControlsDisabled = true;
                testSpec.TenorWeightDisabled = true;
                screenA.PlayDisabled = true;
                screenB.PlayDisabled = true;
                StateHasChanged();

                // Create a TestSubmission object
                TestSubmission testSubmission = new TestSubmission()
                {
                    UserId = string.Empty,
                    TestDate = DateTime.Now,
                    TestType = "A/B Test",
                    TestId = testSpec.SelectedTest,
                    ABResult = aBResult
                };

                // Push the testSubmission to the API in JSON format
                await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/testsubmissions", testSubmission);

                switch (callbackParam)
                {
                    case CallbackParam.AHasErrors:
                        testSpec.SpinnerSubmitting1 = false;
                        testSpec.Submitted1 = true;
                        break;

                    case CallbackParam.BHasErrors:
                        testSpec.SpinnerSubmitting2 = false;
                        testSpec.Submitted2 = true;
                        break;

                    case CallbackParam.DontKnow:
                        testSpec.SpinnerSubmitting3 = false;
                        testSpec.Submitted3 = true;
                        break;

                    default:
                        break;
                }

                StateHasChanged();

                await Task.Delay(1000);

                switch (callbackParam)
                {
                    case CallbackParam.AHasErrors:
                        testSpec.Submitted1 = false;
                        testSpec.SubmitLabel1 = "A has errors";
                        break;

                    case CallbackParam.BHasErrors:
                        testSpec.Submitted2 = false;
                        testSpec.SubmitLabel2 = "B has errors";
                        break;

                    case CallbackParam.DontKnow:
                        testSpec.Submitted3 = false;
                        testSpec.SubmitLabel3 = "I can't tell which has errors";
                        break;

                    default:
                        break;
                }

                testSpec.ControlsDisabled = false;
                testSpec.TenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
                screenA.PlayDisabled = false;
                screenB.PlayDisabled = false;
                StateHasChanged();
            }
            else
            {
                testSpec.ShowGaps = true;
                testSpec.ResultEntered = true;
                
                if (callbackParam == CallbackParam.AHasErrors || callbackParam == CallbackParam.BHasErrors)
                {
                    switch (callbackParam)
                    {
                        case CallbackParam.AHasErrors:
                            if (testSpec.AHasErrors == true)
                            {
                                testSpec.ResultSource = "/audio/right.mp3";
                            }
                            else
                            {
                                testSpec.ResultSource = "/audio/wrong.mp3";
                            }
                        
                            break;

                        case CallbackParam.BHasErrors:
                            if (testSpec.AHasErrors == true)
                            {
                                testSpec.ResultSource = "/audio/wrong.mp3";
                            }
                            else
                            {
                                testSpec.ResultSource = "/audio/right.mp3";
                            }
                        
                            break;

                        default:
                            break;
                    }

                    testSpec.ResultSound = true;

                    // Wait for 1.5 seconds for the sound to finish
                    await Task.Delay(1500);

                    testSpec.ResultSound = false;
                }
            }
        }
    }
}
