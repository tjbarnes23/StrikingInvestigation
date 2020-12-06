using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Index
    {
        int browserWidth;
        int browserHeight;

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        Device Device { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await PopulateBrowserDimensions();
        }

        async Task PopulateBrowserDimensions()
        {
            BrowserDimensions browserDimensions = await JSRuntime.InvokeAsync<BrowserDimensions>("getDimensions");
            browserWidth = browserDimensions.Width;
            browserHeight = browserDimensions.Height;
        }
    }
}
