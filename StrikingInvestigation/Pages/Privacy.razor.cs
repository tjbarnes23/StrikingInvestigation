using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Privacy
    {
        readonly TestSpec testSpec;

        public Privacy()
        {
            testSpec = new TestSpec();
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        Device Device { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            testSpec.BrowserWidth = browserDimensions.Width;
            testSpec.BrowserHeight = browserDimensions.Height;
            testSpec.DeviceLoad = Device.DeviceLoad;
        }
    }
}
