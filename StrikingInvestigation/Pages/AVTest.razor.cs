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
    partial class AVTest
    {
        ElementReference mainDiv;

        internal AVTest()
        {
            Screen = new Screen();
            Screen.DiameterScale = 3;
            Screen.XScale = Constants.XScale;
            Screen.XMargin = Constants.XMargin;
            Screen.YScale = Constants.YScale;
            Screen.YMargin = 300;
            Screen.GapMin = 200;
            Screen.GapMax = 1200;
            Screen.BaseGap = 0;
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        HttpClient Http { get; set; }

        IEnumerable<AVTestData> AVTestsData { get; set; }

        int SelectedTest { get; set; } = -1;

        bool ShowGaps { get; set; }

        Blow Blow { get; set; }

        Screen Screen { get; set; }

        string SaveLabel { get; set; }

        string PlayLabel { get; set; }

        string SubmitLabel { get; set; }

        CancellationTokenSource CancellationTokenSource { get; set; }

        CancellationToken CancellationToken { get; set; }

        bool ControlsDisabled { get; set; }

        bool PlayDisabled { get; set; }

        bool Spinner1 { get; set; }

        bool Spinner2 { get; set; }

        bool SpinnerSubmit { get; set; }

        bool Saved { get; set; }

        bool Submitted { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AVTestsData = (await Http.GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();
            SaveLabel = "Save";
            SubmitLabel = "Submit";
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        void SetState(ScreenState screenState)
        {
            if (screenState == ScreenState.Play)
            {
                PlayLabel = "Play";
                ControlsDisabled = false;
            }
            else if (screenState == ScreenState.Stop)
            {
                PlayLabel = "Stop";
                ControlsDisabled = true;
            }
        }

        void TestChanged(int value)
        {
            SelectedTest = value;
            Blow = null;

            if (SelectedTest != 0 && SelectedTest != -1)
            {
                Load(SelectedTest);
            }
        }

        void ShowGapsChanged(bool value)
        {
            ShowGaps = value;
        }

        void Create()
        {
            Blow = new Blow();
            Blow.CreateRandomBlow();

            Blow.Gap = 700;
            Blow.GapCumulativeRow = Blow.Gap;
            Blow.GapCumulative = Blow.Gap;
            Blow.GapStr = Blow.Gap.ToString();

            Random rand = new Random();
            Blow.AltGap = rand.Next(400, 701);

            if (Blow.AltGap > 550)
            {
                Blow.AltGap += 300;
            }

            // Round
            Blow.AltGap = Convert.ToInt32((double)Blow.AltGap / Constants.Rounding) * Constants.Rounding;

            Blow.BellColor = Constants.UnstruckTestBellColor;

            ShowGaps = false;
            SetState(ScreenState.Play);
        }

        async void Load(int id)
        {
            // Get a test from the API
            AVTestData aVTestData = await Http.GetFromJsonAsync<AVTestData>("api/avtests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowCore object
            BlowCore blowCore = JsonSerializer.Deserialize<BlowCore>(aVTestData.AVTestSpec);

            // Now create a Blow object from the BlowCore object
            Blow = new Blow();
            Blow.LoadBlow(blowCore);
            
            Blow.BellColor = Constants.UnstruckTestBellColor;

            ShowGaps = false;
            SetState(ScreenState.Play);
            StateHasChanged();
        }

        async void Save()
        {
            Spinner1 = true;
            SaveLabel = "Wait";
            ControlsDisabled = true;
            PlayDisabled = true;
            Blow.BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Push the created test to the API in JSON format
            // Start by creating a BlowCore object, which just has the inherited properties from Blow
            // Note implicit cast from child to parent
            BlowCore blowCore = Blow;

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowCore object
            AVTestData aVTestData = new AVTestData();
            aVTestData.AVTestSpec = JsonSerializer.Serialize(blowCore);

            // Push the Json object to the API
            await Http.PostAsJsonAsync("api/avtests", aVTestData);

            // Refresh the contents of the Select Test dropdown 
            AVTestsData = (await Http.GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();

            Spinner1 = false;
            Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            Saved = false;
            SaveLabel = "Save";
            ControlsDisabled = false;
            PlayDisabled = false;
            Blow.BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        async Task Play()
        {
            if (PlayLabel == "Play")
            {
                // Change test bell color to disabled color - can't adjust gap during play
                Blow.BellColor = Constants.DisabledUnstruckTestBellColor;

                DateTime strikeTime = DateTime.Now;

                // This is the time to display the strike on the screen
                Blow.StrikeTime = strikeTime.AddMilliseconds(Blow.Gap);

                // This is the time to make the strike sound
                Blow.AltStrikeTime = strikeTime.AddMilliseconds(Blow.AltGap);

                CancellationTokenSource = new CancellationTokenSource();
                CancellationToken = CancellationTokenSource.Token;

                SetState(ScreenState.Stop);
                await Strike();
            }
            else if (PlayLabel == "Stop")
            {
                PlayLabel = "Wait";
                PlayDisabled = true;
                Spinner2 = true;

                CancellationTokenSource.Cancel();

                // Wait for 2.6 seconds for the sound to finish
                await Task.Delay(2600);

                Spinner2 = false;
                PlayLabel = "Play";
                PlayDisabled = false;
            }

            // Set color to unstruck
            Blow.BellColor = Constants.UnstruckTestBellColor;

            SetState(ScreenState.Play);
        }

        async Task Strike()
        {
            TimeSpan delay;
            int delayMs;

            if (Blow.AltStrikeTime <= Blow.StrikeTime)
            {
                // Process sound before display
                delay = Blow.AltStrikeTime - DateTime.Now;
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
                await JSRuntime.InvokeVoidAsync("PlayBellAudio", Blow.AudioId);

                // Change color of bell on screen
                delay = Blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs, CancellationToken);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Change bell color
                Blow.BellColor = Constants.StruckBellColor;
                StateHasChanged();
            }
            else
            {
                // Process display before sound
                delay = Blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs, CancellationToken);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Change bell color
                Blow.BellColor = Constants.StruckBellColor;
                StateHasChanged();

                // Bell sound
                delay = Blow.AltStrikeTime - DateTime.Now;
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
                await JSRuntime.InvokeVoidAsync("PlayBellAudio", Blow.AudioId);
            }

            // Wait for 2.6 seconds for the sound to finish
            await Task.Delay(1000, CancellationToken);

            Spinner2 = true;
            PlayLabel = "Wait";
            PlayDisabled = true;
            StateHasChanged();
            await Task.Delay(1600);
            Spinner2 = false;
            PlayLabel = "Play";
            PlayDisabled = false;
        }

        async Task Submit()
        {
            SpinnerSubmit = true;
            SubmitLabel = "Wait";
            ControlsDisabled = true;
            PlayDisabled = true;
            Blow.BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "A/V Test",
                TestId = SelectedTest,
                Gap = Blow.Gap,
                AB = string.Empty
            };

            // Push the testSubmission to the API in JSON format
            await Http.PostAsJsonAsync("api/testsubmissions", testSubmission);

            SpinnerSubmit = false;
            Submitted = true;
            StateHasChanged();

            await Task.Delay(1000);

            Submitted = false;
            SubmitLabel = "Submit";
            ControlsDisabled = false;
            PlayDisabled = false;
            Blow.BellColor = Constants.UnstruckTestBellColor;
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
                if (PlayLabel == "Play")
                {
                    // Call TestBellMouseMove to center the bell on where the mouse is clicked
                    TestBellMouseMove(e);
                }
            }
        }

        void TestBellMouseMove(MouseEventArgs e)
        {
            if (e.Buttons == 1 && PlayLabel == "Play")
            {
                int clientX = Convert.ToInt32(e.ClientX);

                int newGap = Convert.ToInt32((clientX - Screen.XMargin) / Screen.XScale);
                int newGapRounded = Convert.ToInt32((double)newGap / Constants.Rounding) * Constants.Rounding;

                if (newGapRounded >= Screen.GapMin && newGapRounded <= Screen.GapMax &&
                        newGapRounded != Blow.Gap)
                {
                    Blow.UpdateGap(newGapRounded);
                }
            }
        }

        void ArrowKeys(KeyboardEventArgs e)
        {
            // Keyboard arrows only active in Play mode
            if (PlayLabel == "Play")
            {
                if (e.Key == "ArrowLeft")
                {
                    int newGap = Blow.Gap - Constants.Rounding;

                    if (newGap >= Screen.GapMin)
                    {
                        Blow.UpdateGap(newGap);
                    }
                }
                else if (e.Key == "ArrowRight")
                {
                    int newGap = Blow.Gap + Constants.Rounding;

                    if (newGap <= Screen.GapMax)
                    {
                        Blow.UpdateGap(newGap);
                    }
                }
            }
        }
    }
}
