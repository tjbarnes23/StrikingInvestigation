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
    public partial class AVTest
    {
        IEnumerable<AVTestData> aVTestsData;
        readonly Screen screen;
        int selectedTest;
        
        bool showGaps;

        Blow blow;

        bool controlsDisabled;
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

        public AVTest()
        {
            screen = new Screen
            {
                DiameterScale = 3,
                XScale = Constants.XScale,
                XMargin = Constants.XMargin,
                YScale = Constants.YScale,
                YMargin = Constants.YMargin + 75,
                GapMin = 200,
                GapMax = 1200,
                BaseGap = 0
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
            aVTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();
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
            blow = null;

            if (selectedTest != 0 && selectedTest != -1)
            {
                await Load(selectedTest);
            }
        }

        void ShowGapsChanged(bool value)
        {
            showGaps = value;
        }

        void Create()
        {
            blow = new Blow();
            blow.CreateRandomBlow();

            blow.Gap = 700;
            blow.GapCumulativeRow = blow.Gap;
            blow.GapCumulative = blow.Gap;
            blow.GapStr = blow.Gap.ToString();

            Random rand = new Random();
            blow.AltGap = rand.Next(400, 701);

            if (blow.AltGap > 550)
            {
                blow.AltGap += 300;
            }

            // Round
            blow.AltGap = Convert.ToInt32((double)blow.AltGap / Constants.Rounding) * Constants.Rounding;

            blow.BellColor = Constants.UnstruckTestBellColor;
        }

        async Task Load(int id)
        {
            // Get a test from the API
            AVTestData aVTestData = await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<AVTestData>("api/avtests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowCore object
            BlowCore blowCore = JsonSerializer.Deserialize<BlowCore>(aVTestData.AVTestSpec);

            // Now create a Blow object from the BlowCore object
            blow = new Blow();
            blow.LoadBlow(blowCore);
            
            blow.BellColor = Constants.UnstruckTestBellColor;

            showGaps = false;
            StateHasChanged();
        }

        async Task Save()
        {
            spinnerSaving = true;
            saveLabel = "Wait";
            controlsDisabled = true;
            playDisabled = true;
            blow.BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Push the created test to the API in JSON format
            // Start by creating a BlowCore object, which just has the inherited properties from Blow
            // Note implicit cast from child to parent
            BlowCore blowCore = blow;

            // Next use the Serializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a Json object from the BlowCore object
            AVTestData aVTestData = new AVTestData
            {
                AVTestSpec = JsonSerializer.Serialize(blowCore)
            };

            // Push the Json object to the API
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/avtests", aVTestData);

            // Refresh the contents of the Select Test dropdown 
            aVTestsData = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();

            spinnerSaving = false;
            saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            saved = false;
            saveLabel = "Save";
            controlsDisabled = false;
            playDisabled = false;
            blow.BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        async Task PlayAsync()
        {
            int initialDelay = 1000;

            if (playLabel == "Play")
            {
                // Change test bell color to disabled color - can't adjust gap during play
                blow.BellColor = Constants.DisabledUnstruckTestBellColor;

                DateTime strikeTime = DateTime.Now;

                // This is the time to display the strike on the screen
                blow.StrikeTime = strikeTime.AddMilliseconds(blow.Gap);

                // This is the time to make the strike sound
                blow.AltStrikeTime = strikeTime.AddMilliseconds(blow.AltGap);

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                playLabel = "Stop";
                controlsDisabled = true;

                TimeSpan delay;
                int delayMs;

                if (blow.AltStrikeTime <= blow.StrikeTime)
                {
                    // Process sound before display
                    delay = blow.AltStrikeTime - DateTime.Now;
                    delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // Strike bell
                    await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);

                    // Process display
                    delay = blow.StrikeTime - DateTime.Now;
                    delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // Change bell color
                    blow.BellColor = Constants.StruckBellColor;

                    // Confirmed this is needed here
                    StateHasChanged();
                }
                else
                {
                    // Process display before sound
                    delay = blow.StrikeTime - DateTime.Now;
                    delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // Change bell color
                    blow.BellColor = Constants.StruckBellColor;

                    // Confirmed this is needed here
                    StateHasChanged();

                    // Process sound
                    delay = blow.AltStrikeTime - DateTime.Now;
                    delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
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
            blow.BellColor = Constants.UnstruckTestBellColor;
            controlsDisabled = false;
            StateHasChanged();
        }

        async void Play()
        {
            // Change test bell color to disabled color - can't adjust gap during play
            blow.BellColor = Constants.DisabledUnstruckTestBellColor;

            DateTime strikeTime = DateTime.Now;

            // This is the time to display the strike on the screen
            blow.StrikeTime = strikeTime.AddMilliseconds(blow.Gap);

            // This is the time to make the strike sound
            blow.AltStrikeTime = strikeTime.AddMilliseconds(blow.AltGap);

            playDisabled = true;
            playLabel = "Wait";
            controlsDisabled = true;

            TimeSpan delay;
            int delayMs;

            if (blow.AltStrikeTime <= blow.StrikeTime)
            {
                // Process sound before display
                delay = blow.AltStrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }

                // Strike bell
                await JSRuntime.InvokeVoidAsync("PlayBellAudio", blow.AudioId);

                // Process display
                delay = blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }

                // Change bell color
                blow.BellColor = Constants.StruckBellColor;

                // Confirmed this is needed here
                StateHasChanged();
            }
            else
            {
                // Process display before sound
                delay = blow.StrikeTime - DateTime.Now;
                delayMs = Convert.ToInt32(delay.TotalMilliseconds);

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }

                // Change bell color
                blow.BellColor = Constants.StruckBellColor;

                // Confirmed this is needed here
                StateHasChanged();

                // Process sound
                delay = blow.AltStrikeTime - DateTime.Now;
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
            blow.BellColor = Constants.UnstruckTestBellColor;
            controlsDisabled = false;
            StateHasChanged();
        }

        async Task Submit()
        {
            spinnerSubmitting = true;
            submitLabel = "Wait";
            controlsDisabled = true;
            playDisabled = true;
            blow.BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "A/V Test",
                TestId = selectedTest,
                Gap = blow.Gap,
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
            blow.BellColor = Constants.UnstruckTestBellColor;
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

                int newGap = Convert.ToInt32((clientX - screen.XMargin) / screen.XScale);
                int newGapRounded = Convert.ToInt32((double)newGap / Constants.Rounding) * Constants.Rounding;

                if (newGapRounded >= screen.GapMin && newGapRounded <= screen.GapMax &&
                        newGapRounded != blow.Gap)
                {
                    blow.UpdateGap(newGapRounded);
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
                    int newGap = blow.Gap - Constants.Rounding;

                    if (newGap >= screen.GapMin)
                    {
                        blow.UpdateGap(newGap);
                    }
                }
                else if (e.Key == "ArrowRight")
                {
                    int newGap = blow.Gap + Constants.Rounding;

                    if (newGap <= screen.GapMax)
                    {
                        blow.UpdateGap(newGap);
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
