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
        readonly TestSpec testSpec;
        readonly Screen screen;
        Blow blow;
                
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;
        ElementReference mainDiv;

        public AVTest()
        {
            testSpec = new TestSpec
            {
                SelectedTest = -1
            };

            screen = new Screen();
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

            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            testSpec.BrowserWidth = browserDimensions.Width;
            testSpec.BrowserHeight = browserDimensions.Height;
            testSpec.DeviceLoad = Device.DeviceLoad;

            testSpec.SaveLabel = "Save";

            testSpec.DiameterScale = ScreenSizing.DiameterScale(testSpec.BrowserWidth) * 1.5;
            testSpec.XScale = ScreenSizing.XScale(testSpec.BrowserWidth);
            testSpec.YScale = ScreenSizing.YScale(testSpec.BrowserWidth);
            testSpec.BorderWidth = ScreenSizing.BorderWidth(testSpec.BrowserWidth);
            testSpec.FontSize = ScreenSizing.FontSize(testSpec.BrowserWidth);
            testSpec.GapMin = 200;
            testSpec.GapMax = 1200;

            testSpec.ButtonsCentered = true;
            testSpec.SubmitLabel1 = "Submit";

            screen.XMargin = ScreenSizing.XMargin(testSpec.BrowserWidth);
            screen.YMargin = ScreenSizing.YMargin(testSpec.BrowserWidth) + 50;
            screen.PlayLabel = "Play";
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", mainDiv);
        }

        async Task TestChanged(int value)
        {
            testSpec.SelectedTest = value;
            blow = null;

            if (testSpec.SelectedTest != 0 && testSpec.SelectedTest != -1)
            {
                await Load(testSpec.SelectedTest);
            }
        }

        void ShowGapsChanged(bool value)
        {
            testSpec.ShowGaps = value;
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

            testSpec.ShowGaps = false;
            StateHasChanged();
        }

        async Task Save()
        {
            testSpec.SpinnerSaving = true;
            testSpec.SaveLabel = "Wait";
            testSpec.ControlsDisabled = true;
            screen.PlayDisabled = true;
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

            testSpec.SpinnerSaving = false;
            testSpec.Saved = true;
            StateHasChanged();

            await Task.Delay(1000);

            testSpec.Saved = false;
            testSpec.SaveLabel = "Save";
            testSpec.ControlsDisabled = false;
            screen.PlayDisabled = false;
            blow.BellColor = Constants.UnstruckTestBellColor;
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

                case CallbackParam.PlayAsync:
                    await PlayAsync();
                    break;

                case CallbackParam.Play:
                    Play();
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
                blow.BellColor = Constants.DisabledUnstruckTestBellColor;

                DateTime strikeTime = DateTime.Now;

                // This is the time to display the strike on the screen
                blow.StrikeTime = strikeTime.AddMilliseconds(blow.Gap);

                // This is the time to make the strike sound
                blow.AltStrikeTime = strikeTime.AddMilliseconds(blow.AltGap);

                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                screen.PlayLabel = "Stop";
                testSpec.ControlsDisabled = true;

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
            blow.BellColor = Constants.UnstruckTestBellColor;
            testSpec.ControlsDisabled = false;
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

            screen.PlayDisabled = true;
            screen.PlayLabel = "Wait";
            testSpec.ControlsDisabled = true;

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
            screen.SpinnerPlaying = true;
            StateHasChanged();

            // Wait further 1.6 seconds for the sound to finish
            await Task.Delay(1600);

            // Reset play button
            screen.SpinnerPlaying = false;
            screen.PlayLabel = "Play";
            screen.PlayDisabled = false;

            // Reset screen
            blow.BellColor = Constants.UnstruckTestBellColor;
            testSpec.ControlsDisabled = false;
            StateHasChanged();
        }

        async Task Submit()
        {
            testSpec.SpinnerSubmitting1 = true;
            testSpec.SubmitLabel1 = "Wait";
            testSpec.ControlsDisabled = true;
            screen.PlayDisabled = true;
            blow.BellColor = Constants.DisabledUnstruckTestBellColor;
            StateHasChanged();

            // Create a TestSubmission object
            TestSubmission testSubmission = new TestSubmission()
            {
                UserId = string.Empty,
                TestDate = DateTime.Now,
                TestType = "A/V Test",
                TestId = testSpec.SelectedTest,
                Gap = blow.Gap,
                ABResult = ABResult.NA
            };

            // Push the testSubmission to the API in JSON format
            await TJBarnesService.GetHttpClient().PostAsJsonAsync("api/testsubmissions", testSubmission);

            testSpec.SpinnerSubmitting1 = false;
            testSpec.Submitted1 = true;
            StateHasChanged();

            await Task.Delay(1000);

            testSpec.Submitted1 = false;
            testSpec.SubmitLabel1 = "Submit";
            testSpec.ControlsDisabled = false;
            screen.PlayDisabled = false;
            blow.BellColor = Constants.UnstruckTestBellColor;
            StateHasChanged();
        }

        void ArrowKeys(KeyboardEventArgs e)
        {
            // Keyboard arrows only active when a blow is populated, the play button says Play (as opposed to
            // Stop or Wait), and the Play button is not disabled
            // The last test is needed because when submitting, the play button says Play
            if (blow != null && screen.PlayLabel == "Play" && screen.PlayDisabled == false)
            {
                if (e.Key == "ArrowLeft")
                {
                    int newGap = blow.Gap - Constants.Rounding;

                    if (newGap >= testSpec.GapMin)
                    {
                        blow.UpdateGap(newGap);
                    }
                }
                else if (e.Key == "ArrowRight")
                {
                    int newGap = blow.Gap + Constants.Rounding;

                    if (newGap <= testSpec.GapMax)
                    {
                        blow.UpdateGap(newGap);
                    }
                }
            }
        }
    }
}
