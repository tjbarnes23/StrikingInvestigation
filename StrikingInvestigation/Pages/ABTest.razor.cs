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
        int selectedTest;

        bool showGaps;

        BlowSet blowSetA;
        BlowSet blowSetB;

        bool controlsDisabled;
        bool tenorWeightDisabled;
        bool playDisabledA;
        bool playDisabledB;

        string saveLabel;
        string playLabelA;
        string playLabelB;
        string submitLabel1;
        string submitLabel2;
        string submitLabel3;

        bool spinnerSaving;
        bool spinnerPlayingA;
        bool spinnerPlayingB;
        bool spinnerSubmitting1;
        bool spinnerSubmitting2;
        bool spinnerSubmitting3;

        bool saved;
        bool submitted1;
        bool submitted2;
        bool submitted3;

        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        bool isA;
        bool resultSound;
        string resultSource;
        bool resultEntered;

        int width;

        public ABTest()
        {
            testSpec = new TestSpec
            {
                Stage = 8,
                TenorWeight = 23,
                NumRows = 4,
                ErrorType = 1,
                ErrorSize = 60,
                TestBellLoc = 0 // Indicates no test bell
            };

            // No rounding in A/B test
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);

            screenA = new Screen
            {
                DiameterScale = Constants.DiameterScale,
                XScale = Constants.XScale,
                XMargin = Constants.XMargin,
                YScale = Constants.YScale,
                YMargin = Constants.YMargin,
                GapMin = 0,
                GapMax = 0,
                BaseGap = baseGap,
                RunAnimation = false,
                AnimationDuration = 0
            };

            screenB = new Screen
            {
                DiameterScale = Constants.DiameterScale,
                XScale = Constants.XScale,
                XMargin = Constants.XMargin,
                YScale = Constants.YScale,
                YMargin = Constants.YMargin + 475,
                GapMin = 0,
                GapMax = 0,
                BaseGap = baseGap,
                RunAnimation = false,
                AnimationDuration = 0
            };

            selectedTest = -1;
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        TJBarnesService TJBarnesService { get; set; }

        [Inject]
        Device Device { get; set; }

        [Inject]
        Viewport Viewport { get; set; }

        protected override async Task OnInitializedAsync()
        {
            aBTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<ABTestData[]>("api/abtests")).ToList();
            saveLabel = "Save";
            playLabelA = "Play A";
            playLabelB = "Play B";
            submitLabel1 = "A has errors";
            submitLabel2 = "B has errors";
            submitLabel3 = "I can't tell which has errors";
        }

        async Task TestChanged(int value)
        {
            selectedTest = value;
            blowSetA = null;
            blowSetB = null;

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
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            screenA.BaseGap = baseGap;
            screenB.BaseGap = baseGap;

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

            // Need to recalculate BaseGap on a tenor change
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            screenA.BaseGap = baseGap;
            screenB.BaseGap = baseGap;

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
            showGaps = value;
        }

        void CreateABTest()
        {
            // Choose whether A or B will have the errors
            Random rand = new Random();
            isA = rand.Next(0, 2) == 0;

            // Create the test block
            Block testBlock = new Block(testSpec.Stage, testSpec.NumRows);
            testBlock.CreateRandomBlock();

            blowSetA = new BlowSet(testSpec.Stage, testSpec.NumRows, testSpec.TenorWeight,
                    testSpec.ErrorType, isA);
            blowSetA.PopulateBlows(testBlock, testSpec.TestBellLoc, "a");

            // No rounding in an A/B test
            blowSetA.CreateEvenSpacing(1);
            blowSetA.SetUnstruck();

            blowSetB = new BlowSet(testSpec.Stage, testSpec.NumRows, testSpec.TenorWeight,
                    testSpec.ErrorType, !isA);
            blowSetB.PopulateBlows(testBlock, testSpec.TestBellLoc, "b");

            // No rounding in an A/B test
            blowSetB.CreateEvenSpacing(1);
            blowSetB.SetUnstruck();

            if (isA == true)
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

            screenA.AnimationDuration = blowSetA.Blows.Last().GapCumulative + 600;
            screenB.AnimationDuration = blowSetB.Blows.Last().GapCumulative + 600;
            resultEntered = false;

            showGaps = false;
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

            isA = blowSetCoreA.HasErrors;

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

            // Update Screen.BaseGap - this varies by stage and tenorweight, and is used to shift backstroke rows
            // to the right to make the 1st blow of each row align vertically (when no striking errors)
            // No rounding in an A/B test
            int baseGap = BaseGaps.BaseGap(testSpec.Stage, testSpec.TenorWeight, 1);
            screenA.BaseGap = baseGap;
            screenB.BaseGap = baseGap;

            screenA.AnimationDuration = blowSetA.Blows.Last().GapCumulative + 600;
            screenB.AnimationDuration = blowSetB.Blows.Last().GapCumulative + 600;
            resultEntered = false;

            showGaps = false;
            StateHasChanged();
        }

        async Task Save()
        {
            spinnerSaving = true;
            saveLabel = "Wait";
            controlsDisabled = true;
            tenorWeightDisabled = true;
            playDisabledA = true;
            playDisabledB = true;
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

            spinnerSaving = false;
            saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            saved = false;
            saveLabel = "Save";
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledA = false;
            playDisabledB = false;
            StateHasChanged();
        }

        async Task PlayAsyncA()
        {
            int initialDelay = 600;

            if (playLabelA == "Play A")
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

                playLabelA = "Stop A";
                controlsDisabled = true;
                tenorWeightDisabled = true;
                playDisabledB = true;

                // Start the animation if not showing the bells
                if (showGaps == false)
                {
                    screenA.RunAnimation = true;
                }

                foreach (Blow blow in blowSetA.Blows)
                {
                    TimeSpan delay;
                    int delayMs;

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
                    if (Device.DeviceLoad == DeviceLoad.High && showGaps == true)
                    {
                        blow.BellColor = Constants.StruckBellColor;

                        // Confirmed this is needed here
                        StateHasChanged();
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
                }
            }
            else if (playLabelA == "Stop A")
            {
                cancellationTokenSource.Cancel();
                initialDelay = 0;
            }

            // Initial delay
            if (initialDelay > 0)
            {
                await Task.Delay(initialDelay);
            }

            // Reset animation
            if (showGaps == false)
            {
                screenA.RunAnimation = false;
            }

            // Start spinner
            playDisabledA = true;
            playLabelA = "Wait";
            spinnerPlayingA = true;
            StateHasChanged();

            // Wait 2.6 or 2 seconds depending on whether arriving here on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset play button
            spinnerPlayingA = false;
            playLabelA = "Play A";
            playDisabledA = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High && showGaps == true)
            {
                blowSetA.SetUnstruck();
            }
                        
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledB = false;
            StateHasChanged();
        }

        async Task PlayAsyncB()
        {
            int initialDelay = 600;

            if (playLabelB == "Play B")
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

                playLabelB = "Stop B";
                controlsDisabled = true;
                tenorWeightDisabled = true;
                playDisabledA = true;

                // Start the animation if not showing the bells
                if (showGaps == false)
                {
                    screenB.RunAnimation = true;
                }

                foreach (Blow blow in blowSetB.Blows)
                {
                    TimeSpan delay;
                    int delayMs;

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
                    if (Device.DeviceLoad == DeviceLoad.High && showGaps == true)
                    {
                        blow.BellColor = Constants.StruckBellColor;

                        // Confirmed this is needed here
                        StateHasChanged();
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);
                }
            }
            else if (playLabelB == "Stop B")
            {
                cancellationTokenSource.Cancel();
                initialDelay = 0;
            }

            // Initial delay
            if (initialDelay > 0)
            {
                await Task.Delay(initialDelay);
            }

            // Reset animation
            if (showGaps == false)
            {
                screenB.RunAnimation = false;
            }

            // Start spinner
            playDisabledB = true;
            playLabelB = "Wait";
            spinnerPlayingB = true;
            StateHasChanged();

            // Wait 2.6 or 2 seconds depending on whether arriving here on stop or end of play
            await Task.Delay(2600 - initialDelay);

            // Reset play button
            spinnerPlayingB = false;
            playLabelB = "Play B";
            playDisabledB = false;

            // Reset screen
            if (Device.DeviceLoad == DeviceLoad.High && showGaps == true)
            {
                blowSetB.SetUnstruck();
            }

            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledA = false;
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

            playDisabledA = true;
            playLabelA = "Wait";
            controlsDisabled = true;
            tenorWeightDisabled = true;
            playDisabledB = true;

            // Start the animation if not showing the bells
            if (showGaps == false)
            {
                screenA.RunAnimation = true;
            }

            foreach (Blow blow in blowSetA.Blows)
            {
                TimeSpan delay;
                int delayMs;

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
            await Task.Delay(600);

            // Reset animation
            if (showGaps == false)
            {
                screenA.RunAnimation = false;
            }

            // Start spinner
            spinnerPlayingA = true;
            StateHasChanged();

            // Wait 2 seconds
            await Task.Delay(2000);

            // Reset play button
            spinnerPlayingA = false;
            playLabelA = "Play A";
            playDisabledA = false;

            // Reset screen
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledB = false;
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

            playDisabledB = true;
            playLabelB = "Wait";
            controlsDisabled = true;
            tenorWeightDisabled = true;
            playDisabledA = true;

            // Start the animation if not showing the bells
            if (showGaps == false)
            {
                screenB.RunAnimation = true;
            }

            foreach (Blow blow in blowSetB.Blows)
            {
                TimeSpan delay;
                int delayMs;

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
            await Task.Delay(600);

            // Reset animation
            if (showGaps == false)
            {
                screenB.RunAnimation = false;
            }

            // Start spinner
            spinnerPlayingB = true;
            StateHasChanged();

            // Wait 2 seconds
            await Task.Delay(2000);

            // Reset play button
            spinnerPlayingB = false;
            playLabelB = "Play B";
            playDisabledB = false;

            // Reset screen
            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledA = false;
            StateHasChanged();
        }

        async Task AHasErrors()
        {
            if (selectedTest != 0 && selectedTest != -1)
            {
                await Submit("A has errors");
            }
            else
            {
                showGaps = true;
                resultEntered = true;

                if (isA == true)
                {
                    resultSource = "/audio/right.mp3";
                }
                else
                {
                    resultSource = "/audio/wrong.mp3";
                }

                resultSound = true;

                // Wait for 1.5 seconds for the sound to finish
                await Task.Delay(1500);

                resultSound = false;
            }
        }

        async Task BHasErrors()
        {
            if (selectedTest != 0 && selectedTest != -1)
            {
                await Submit("B has errors");
            }
            else
            {
                showGaps = true;
                resultEntered = true;

                if (isA == true)
                {
                    resultSource = "/audio/wrong.mp3";
                }
                else
                {
                    resultSource = "/audio/right.mp3";
                }

                resultSound = true;

                // Wait for 1.5 seconds for the sound to finish
                await Task.Delay(1500);

                resultSound = false;
            }
        }

        async Task DontKnow()
        {
            if (selectedTest != 0 && selectedTest != -1)
            {
                await Submit("Don't know");
            }
            else
            {
                showGaps = true;
                resultEntered = true;
            }
        }

        async Task Submit(string result)
        {
            switch (result)
            {
                case "A has errors":
                    spinnerSubmitting1 = true;
                    submitLabel1 = "Wait";
                    break;

                case "B has errors":
                    spinnerSubmitting2 = true;
                    submitLabel2 = "Wait";
                    break;

                case "Don't know":
                    spinnerSubmitting3 = true;
                    submitLabel3 = "Wait";
                    break;

                default:
                    break;
            }

            controlsDisabled = true;
            tenorWeightDisabled = true;
            playDisabledA = true;
            playDisabledB = true;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "A/B Test",
                TestId = selectedTest,
                AB = result
            };

            // Push the testSubmission to the API in JSON format
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/testsubmissions", testSubmission);

            switch (result)
            {
                case "A has errors":
                    spinnerSubmitting1 = false;
                    submitted1 = true;
                    break;

                case "B has errors":
                    spinnerSubmitting2 = false;
                    submitted2 = true;
                    break;

                case "Don't know":
                    spinnerSubmitting3 = false;
                    submitted3 = true;
                    break;

                default:
                    break;
            }
            
            StateHasChanged();

            await Task.Delay(1000);

            switch (result)
            {
                case "A has errors":
                    submitted1 = false;
                    submitLabel1 = "A has errors";
                    break;

                case "B has errors":
                    submitted2 = false;
                    submitLabel2 = "B has errors";
                    break;

                case "Don't know":
                    submitted3 = false;
                    submitLabel3 = "I can't tell which has errors";
                    break;

                default:
                    break;
            }

            controlsDisabled = false;
            tenorWeightDisabled = TenorWeightSelect.TenorWeightDisabled(testSpec.Stage);
            playDisabledA = false;
            playDisabledB = false;
            StateHasChanged();
        }

        async Task GetWidth()
        {
            BrowserDimensions browserDimensions = await Viewport.GetDimensions();
            width = browserDimensions.Width;
        }
    }
}
