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

        public AVTest()
        {
            screen = new Screen
            {
                DiameterScale = 3,
                XScale = Constants.XScale,
                XMargin = Constants.XMargin,
                YScale = Constants.YScale,
                YMargin = 300,
                GapMin = 200,
                GapMax = 1200,
                BaseGap = 0
            };

            selectedTest = -1;
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        HttpClient Http { get; set; }
                
        protected override async Task OnInitializedAsync()
        {
            aVTestsData = (await Http.GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();
            saveLabel = "Save";
            playLabel = "Play";
            submitLabel = "Submit";
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        void TestChanged(int value)
        {
            selectedTest = value;
            blow = null;

            if (selectedTest != 0 && selectedTest != -1)
            {
                Load(selectedTest);
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

        async void Load(int id)
        {
            // Get a test from the API
            AVTestData aVTestData = await Http.GetFromJsonAsync<AVTestData>("api/avtests/" + id.ToString());

            // Use the Deserializer method of the JsonSerializer class (in the System.Text.Json namespace) to create
            // a BlowCore object
            BlowCore blowCore = JsonSerializer.Deserialize<BlowCore>(aVTestData.AVTestSpec);

            // Now create a Blow object from the BlowCore object
            blow = new Blow();
            blow.LoadBlow(blowCore);
            
            blow.BellColor = Constants.UnstruckTestBellColor;

            StateHasChanged();
        }

        async void Save()
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
            await Http.PostAsJsonAsync("api/avtests", aVTestData);

            // Refresh the contents of the Select Test dropdown 
            aVTestsData = (await Http.GetFromJsonAsync<AVTestData[]>("api/avtests")).ToList();

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

        async Task Play()
        {
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

                    // Change color of bell on screen
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
                    StateHasChanged();

                    // Bell sound
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

                // Wait for 0.6 seconds
                await Task.Delay(600);

                // Start spinner
                spinnerPlaying = true;
                playLabel = "Wait";
                playDisabled = true;
                StateHasChanged();

                // Wait a further 2 seconds for the bells to finish sounding (each bell sample is 2.5 seconds)
                await Task.Delay(2000);

                // Reset play button
                spinnerPlaying = false;
                playLabel = "Play";
                playDisabled = false;
            }
            else if (playLabel == "Stop")
            {
                spinnerPlaying = true;
                playLabel = "Wait";
                playDisabled = true;

                cancellationTokenSource.Cancel();

                // Wait for 2.6 seconds for the sound to finish
                await Task.Delay(2600);

                // Reset play button
                spinnerPlaying = false;
                playLabel = "Play";
                playDisabled = false;
            }

            // Reset screen
            blow.BellColor = Constants.UnstruckTestBellColor;
            controlsDisabled = false;
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
            await Http.PostAsJsonAsync("api/testsubmissions", testSubmission);

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
    }
}
