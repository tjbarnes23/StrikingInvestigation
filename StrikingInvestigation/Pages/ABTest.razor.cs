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
    public partial class ABTest
    {
        public ABTest()
        {
            TestSpec = new TestSpec();
            TestSpec.Stage = 8;
            TestSpec.TenorWeight = 23;
            TestSpec.NumRows = 4;
            TestSpec.ErrorType = 1;
            TestSpec.ErrorSize = 60;
            TestSpec.TestBellLoc = 0; // Indicates no test bell

            // No rounding in A/B test
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, 1);

            ScreenA = new Screen();
            ScreenA.DiameterScale = Constants.DiameterScale;
            ScreenA.XScale = Constants.XScale;
            ScreenA.XMargin = Constants.XMargin;
            ScreenA.YScale = Constants.YScale;
            ScreenA.YMargin = Constants.YMargin;
            ScreenA.GapMin = 0;
            ScreenA.GapMax = 0;
            ScreenA.BaseGap = baseGap;
            ScreenA.RunAnimation = false;

            ScreenB = new Screen();
            ScreenB.DiameterScale = Constants.DiameterScale;
            ScreenB.XScale = Constants.XScale;
            ScreenB.XMargin = Constants.XMargin;
            ScreenB.YScale = Constants.YScale;
            ScreenB.YMargin = Constants.YMargin + 475;
            ScreenB.GapMin = 0;
            ScreenB.GapMax = 0;
            ScreenB.BaseGap = baseGap;
            ScreenB.RunAnimation = false;
        }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        public TestSpec TestSpec { get; set; }

        public IEnumerable<ABTestData> ABTestsData { get; set; }

        public int SelectedTest { get; set; } = -1;

        public bool ShowGaps { get; set; }

        public bool IsA { get; set; }

        public BlowSet BlowSetA { get; set; }

        public BlowSet BlowSetB { get; set; }

        public Screen ScreenA { get; set; }

        public Screen ScreenB { get; set; }

        public int DurationA { get; set; }

        public int DurationB { get; set; }

        public string PlayLabelA { get; set; }

        public string PlayLabelB { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool ResultSound { get; set; }

        public string ResultSource { get; set; }

        public bool ResultEntered { get; set; }

        public bool ControlsDisabled { get; set; }

        public bool PlayDisabledA { get; set; }

        public bool PlayDisabledB { get; set; }

        public bool SelectTenorWeightDisabled { get; set; }

        public bool CurrentTenorWeightDisabled { get; set; }

        public bool Spinner1 { get; set; }

        public bool Spinner2 { get; set; }

        public bool Spinner3 { get; set; }

        public bool SpinnerSubmit1 { get; set; }

        public bool SpinnerSubmit2 { get; set; }

        public bool SpinnerSubmit3 { get; set; }

        public bool Saved { get; set; }

        public bool Submitted1 { get; set; }

        public bool Submitted2 { get; set; }

        public bool Submitted3 { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ABTestsData = (await Http.GetFromJsonAsync<ABTestData[]>("api/abtests")).ToList();
        }

        protected void SetState(ScreenState screenState)
        {
            if (screenState == ScreenState.Play)
            {
                PlayLabelA = "Play A";
                PlayLabelB = "Play B";
                ControlsDisabled = false;
                SelectTenorWeightDisabled = CurrentTenorWeightDisabled;
                PlayDisabledA = false;
                PlayDisabledB = false;
            }
            else if (screenState == ScreenState.StopA)
            {
                PlayLabelA = "Stop A";
                PlayLabelB = "Play B";
                ControlsDisabled = true;
                SelectTenorWeightDisabled = true;
                PlayDisabledA = false;
                PlayDisabledB = true;
            }
            else if (screenState == ScreenState.StopB)
            {
                PlayLabelA = "Play A";
                PlayLabelB = "Stop B";
                ControlsDisabled = true;
                SelectTenorWeightDisabled = true;
                PlayDisabledA = true;
                PlayDisabledB = false;
            }
        }

        protected void TestChanged(int value)
        {
            SelectedTest = value;
            BlowSetA = null;
            BlowSetB = null;

            if (SelectedTest != 0 && SelectedTest != -1)
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
            // No rounding in A/B test
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, 1);
            ScreenA.BaseGap = baseGap;
            ScreenB.BaseGap = baseGap;

            if (BlowSetA != null)
            {
                BlowSetA = null;
            }

            if (BlowSetB != null)
            {
                BlowSetB = null;
            }
        }

        protected void TenorWeightChanged(int value)
        {
            TestSpec.TenorWeight = value;

            // Need to recalculate BaseGap on a tenor change
            // No rounding in A/B test
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, 1);
            ScreenA.BaseGap = baseGap;
            ScreenB.BaseGap = baseGap;

            if (BlowSetA != null)
            {
                BlowSetA = null;
            }

            if (BlowSetB != null)
            {
                BlowSetB = null;
            }
        }

        protected void ErrorTypeChanged(int value)
        {
            TestSpec.ErrorType = value;

            if (BlowSetA != null)
            {
                BlowSetA = null;
            }

            if (BlowSetB != null)
            {
                BlowSetB = null;
            }
        }

        protected void ErrorSizeChanged(int value)
        {
            TestSpec.ErrorSize = value;

            if (BlowSetA != null)
            {
                BlowSetA = null;
            }

            if (BlowSetB != null)
            {
                BlowSetB = null;
            }
        }

        protected void ShowGapsChanged(bool value)
        {
            ShowGaps = value;
        }

        protected void CreateABTest()
        {
            // Choose whether A or B will have the errors
            Random rand = new Random();
            IsA = rand.Next(0, 2) == 0;

            // Create the test block
            Block testBlock = new Block(TestSpec.Stage, TestSpec.NumRows);
            testBlock.CreateRandomBlock();

            BlowSetA = new BlowSet(TestSpec.Stage, TestSpec.NumRows, TestSpec.TenorWeight,
                    TestSpec.ErrorType, IsA);
            BlowSetA.PopulateBlows(testBlock, TestSpec.TestBellLoc, "a");

            // No rounding in an A/B test
            BlowSetA.CreateEvenSpacing(1);
            BlowSetA.SetUnstruck();

            BlowSetB = new BlowSet(TestSpec.Stage, TestSpec.NumRows, TestSpec.TenorWeight,
                    TestSpec.ErrorType, !IsA);
            BlowSetB.PopulateBlows(testBlock, TestSpec.TestBellLoc, "b");

            // No rounding in an A/B test
            BlowSetB.CreateEvenSpacing(1);
            BlowSetB.SetUnstruck();

            if (IsA == true)
            {
                if (TestSpec.ErrorType == 1)
                {
                    BlowSetA.CreateStrikingError(TestSpec.ErrorSize);
                }
                else
                {
                    BlowSetA.CreateCompassError(TestSpec.ErrorSize);
                }
            }
            else
            {
                if (TestSpec.ErrorType == 1)
                {
                    BlowSetB.CreateStrikingError(TestSpec.ErrorSize);
                }
                else
                {
                    BlowSetB.CreateCompassError(TestSpec.ErrorSize);
                }
            }

            DurationA = BlowSetA.Blows.Last().GapCumulative + 1500;
            DurationB = BlowSetB.Blows.Last().GapCumulative + 1500;
            ResultEntered = false;

            ShowGaps = false;
            SetState(ScreenState.Play);
        }

        protected async void Load(int id)
        {
            // Get a test from the API
            ABTestData aBTestData = await Http.GetFromJsonAsync<ABTestData>("api/abtests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowSetCore object for each of A and B
            BlowSetCore blowSetCoreA = JsonSerializer.Deserialize<BlowSetCore>(aBTestData.ABTestSpecA);
            BlowSetCore blowSetCoreB = JsonSerializer.Deserialize<BlowSetCore>(aBTestData.ABTestSpecB);

            IsA = blowSetCoreA.HasErrors;

            // Create a BlowSet object from the BlowSetCore object for A
            BlowSetA = new BlowSet(blowSetCoreA.Stage, blowSetCoreA.NumRows, blowSetCoreA.TenorWeight,
                    blowSetCoreA.ErrorType, blowSetCoreA.HasErrors);

            // Use an audioIdSuffix of "b" for this blowset
            BlowSetA.LoadBlows(blowSetCoreA, "a");
            
            BlowSetA.SetUnstruck();

            // Create a BlowSet object from the BlowSetCore object for B
            BlowSetB = new BlowSet(blowSetCoreB.Stage, blowSetCoreB.NumRows, blowSetCoreB.TenorWeight,
                    blowSetCoreB.ErrorType, blowSetCoreB.HasErrors);
            
            // Use an audioIdSuffix of "b" for this blowset
            BlowSetB.LoadBlows(blowSetCoreB, "b");

            BlowSetB.SetUnstruck();

            // Update drop down boxes on screen
            // Use BlowSetA - by definition the following properties for BlowSetA and BlowSetB will be the same
            TestSpec.Stage = BlowSetA.Stage;
            TestSpec.TenorWeight = BlowSetA.TenorWeight;
            TestSpec.ErrorType = BlowSetA.ErrorType;

            // Update Screen.BaseGap - this varies by stage and tenorweight, and is used to shift backstroke rows
            // to the right to make the 1st blow of each row align vertically (when no striking errors)
            // No rounding in an A/B test
            int baseGap = BaseGaps.BaseGap(TestSpec.Stage, TestSpec.TenorWeight, 1);
            ScreenA.BaseGap = baseGap;
            ScreenB.BaseGap = baseGap;

            DurationA = BlowSetA.Blows.Last().GapCumulative + 1500;
            DurationB = BlowSetB.Blows.Last().GapCumulative + 1500;
            ResultEntered = false;

            ShowGaps = false;
            SetState(ScreenState.Play);
            StateHasChanged();
        }

        protected async void Save()
        {
            Spinner1 = true;
            ControlsDisabled = true;
            PlayLabelA = "Wait";
            PlayLabelB = "Wait";
            PlayDisabledA = true;
            PlayDisabledB = true;
            StateHasChanged();

            // Push the created test to the API in JSON format
            // Start by creating a BlowSetCore object, which just has the parent data BlowSet, for each of A and B
            // Note implicit cast from child to parent
            BlowSetCore blowSetCoreA = BlowSetA;
            BlowSetCore blowSetCoreB = BlowSetB;

            // BlowSetCore has a BlowsCore list which is empty so far
            // Call the LoadBlowsCore method to populate it for each of A and B
            blowSetCoreA.LoadBlowsCore(BlowSetA);
            blowSetCoreB.LoadBlowsCore(BlowSetB);

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowSetData object for each of A and B
            ABTestData aBTestData = new ABTestData();
            aBTestData.ABTestSpecA = JsonSerializer.Serialize(blowSetCoreA);
            aBTestData.ABTestSpecB = JsonSerializer.Serialize(blowSetCoreB);

            // Push the Json object to the API
            await Http.PostAsJsonAsync("api/abtests", aBTestData);

            // Refresh the contents of the Select Test dropdown 
            ABTestsData = (await Http.GetFromJsonAsync<ABTestData[]>("api/abtests")).ToList();

            Spinner1 = false;
            Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            Saved = false;
            ControlsDisabled = false;
            PlayLabelA = "Play";
            PlayLabelB = "Play";
            PlayDisabledA = false;
            PlayDisabledB = false;
            StateHasChanged();
        }

        protected async Task PlayA()
        {
            if (PlayLabelA == "Play A")
            {
                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in BlowSetA.Blows)
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

                SetState(ScreenState.StopA);
                await StrikeA();

                if (ShowGaps == false)
                {
                    ScreenA.RunAnimation = false;
                }
            }
            else if (PlayLabelA == "Stop A")
            {
                PlayDisabledA = true;
                Spinner2 = true;

                CancellationTokenSource.Cancel();

                if (ShowGaps == false)
                {
                    ScreenA.RunAnimation = false;
                }

                // Wait for 2.6 seconds for the sound to finish
                await Task.Delay(2600);

                Spinner2 = false;
                PlayDisabledA = false;
            }

            if (ShowGaps == true)
            {
                BlowSetA.SetUnstruck();
            }
                
            SetState(ScreenState.Play);
        }

        protected async Task PlayB()
        {
            if (PlayLabelB == "Play B")
            {
                DateTime strikeTime = DateTime.Now;

                foreach (Blow blow in BlowSetB.Blows)
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

                SetState(ScreenState.StopB);
                await StrikeB();

                if (ShowGaps == false)
                {
                    ScreenB.RunAnimation = false;
                }
            }
            else if (PlayLabelB == "Stop B")
            {
                PlayDisabledB = true;
                Spinner3 = true;

                CancellationTokenSource.Cancel();

                if (ShowGaps == false)
                {
                    ScreenB.RunAnimation = false;
                }

                // Wait for 2.6 seconds for the sound to finish
                await Task.Delay(2600);

                Spinner3 = false;
                PlayDisabledB = false;
            }

            if (ShowGaps == true)
            {
                BlowSetB.SetUnstruck();
            }

            SetState(ScreenState.Play);
        }

        public async Task StrikeA()
        {
            // If ShowGaps is false, start the animation
            if (ShowGaps == false)
            {
                ScreenA.RunAnimation = true;
            }

            foreach (Blow blow in BlowSetA.Blows)
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
                if (ShowGaps == true)
                {
                    blow.BellColor = Constants.StruckBellColor;
                    StateHasChanged();
                }
            }

            // Wait for 2.6 seconds for the sound to finish
            await Task.Delay(1000, CancellationToken);

            Spinner2 = true;
            PlayDisabledA = true;
            StateHasChanged();
            await Task.Delay(1600);
            Spinner2 = false;
            PlayDisabledA = false;
        }

        public async Task StrikeB()
        {
            // If ShowGaps is false, start the animation
            if (ShowGaps == false)
            {
                ScreenB.RunAnimation = true;
            }

            foreach (Blow blow in BlowSetB.Blows)
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
                if (ShowGaps == true)
                {
                    blow.BellColor = Constants.StruckBellColor;
                    StateHasChanged();
                }
            }

            // Wait for 2.6 seconds for the sound to finish
            await Task.Delay(1000, CancellationToken);

            Spinner3 = true;
            PlayDisabledB = true;
            StateHasChanged();
            await Task.Delay(1600);
            Spinner3 = false;
            PlayDisabledB = false;
        }

        protected async Task AHasErrors()
        {
            if (SelectedTest != 0 && SelectedTest != -1)
            {
                await Submit("A has errors");
            }
            else
            {
                ShowGaps = true;
                ResultEntered = true;

                if (IsA == true)
                {
                    ResultSource = "/audio/right.mp3";
                }
                else
                {
                    ResultSource = "/audio/wrong.mp3";
                }

                ResultSound = true;

                // Wait for 1.5 seconds for the sound to finish
                await Task.Delay(1500);

                ResultSound = false;
            }
        }

        protected async Task BHasErrors()
        {
            if (SelectedTest != 0 && SelectedTest != -1)
            {
                await Submit("B has errors");
            }
            else
            {
                ShowGaps = true;
                ResultEntered = true;

                if (IsA == true)
                {
                    ResultSource = "/audio/wrong.mp3";
                }
                else
                {
                    ResultSource = "/audio/right.mp3";
                }

                ResultSound = true;

                // Wait for 1.5 seconds for the sound to finish
                await Task.Delay(1500);

                ResultSound = false;
            }
        }

        protected async Task DontKnow()
        {
            if (SelectedTest != 0 && SelectedTest != -1)
            {
                await Submit("Don't know");
            }
            else
            {
                ShowGaps = true;
                ResultEntered = true;
            }
        }

        protected async Task Submit(string result)
        {
            switch (result)
            {
                case "A has errors":
                    SpinnerSubmit1 = true;
                    break;

                case "B has errors":
                    SpinnerSubmit2 = true;
                    break;

                case "Don't know":
                    SpinnerSubmit3 = true;
                    break;

                default:
                    break;
            }

            ControlsDisabled = true;
            PlayLabelA = "Wait";
            PlayLabelB = "Wait";
            PlayDisabledA = true;
            PlayDisabledB = true;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "A/B Test",
                TestId = SelectedTest,
                AB = result
            };

            // Push the testSubmission to the API in JSON format
            await Http.PostAsJsonAsync("api/testsubmissions", testSubmission);

            switch (result)
            {
                case "A has errors":
                    SpinnerSubmit1 = false;
                    Submitted1 = true;
                    break;

                case "B has errors":
                    SpinnerSubmit2 = false;
                    Submitted2 = true;
                    break;

                case "Don't know":
                    SpinnerSubmit3 = false;
                    Submitted3 = true;
                    break;

                default:
                    break;
            }
            
            StateHasChanged();

            await Task.Delay(1000);

            switch (result)
            {
                case "A has errors":
                    Submitted1 = false;
                    break;

                case "B has errors":
                    Submitted2 = false;
                    break;

                case "Don't know":
                    Submitted3 = false;
                    break;

                default:
                    break;
            }

            ControlsDisabled = false;
            PlayLabelA = "Play";
            PlayLabelB = "Play";
            PlayDisabledA = false;
            PlayDisabledB = false;
            StateHasChanged();
        }
    }
}
