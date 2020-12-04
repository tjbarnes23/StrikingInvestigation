using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Index
    {
        int width;

        [Inject]
        Device Device { get; set; }

        [Inject]
        Viewport Viewport { get; set; }

        protected override async Task OnInitializedAsync()
        {
            width = await GetWidth();
        }

        async Task<int> GetWidth()
        {
            BrowserDimensions browserDimensions = await Viewport.GetDimensions();
            return browserDimensions.Width;
        }
    }
}
