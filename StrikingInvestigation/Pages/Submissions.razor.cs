using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Submissions
    {
        IEnumerable<TestSubmission> testSubmissions;
        readonly TestSpec testSpec;

        public Submissions()
        {
            testSpec = new TestSpec();
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        Device Device { get; set; }

        [Inject]
        TJBarnesService TJBarnesService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            testSubmissions = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<TestSubmission[]>("api/testsubmissions")).ToList();
            
            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            testSpec.BrowserWidth = browserDimensions.Width;
            testSpec.BrowserHeight = browserDimensions.Height;
            testSpec.DeviceLoad = Device.DeviceLoad;
        }
    }
}
