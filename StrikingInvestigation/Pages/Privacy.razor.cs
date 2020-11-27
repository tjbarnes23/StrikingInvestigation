using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Privacy
    {
        int width;

        [Inject]
        Device Device { get; set; }

        [Inject]
        Viewport Viewport { get; set; }

        async Task GetWidth()
        {
            BrowserDimensions browserDimensions = await Viewport.GetDimensions();
            width = browserDimensions.Width;
        }
    }
}
