using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Pages
{
    public partial class Submissions
    {
        IEnumerable<TestSubmission> testSubmissions;

        int width;

        [Inject]
        TJBarnesService TJBarnesService { get; set; }

        [Inject]
        Device Device { get; set; }

        [Inject]
        Viewport Viewport { get; set; }

        protected override async Task OnInitializedAsync()
        {
            testSubmissions = (await TJBarnesService.GetHttpClient()
                    .GetFromJsonAsync<TestSubmission[]>("api/testsubmissions")).ToList();
            width = await GetWidth();
        }

        async Task<int> GetWidth()
        {
            BrowserDimensions browserDimensions = await Viewport.GetDimensions();
            return browserDimensions.Width;
        }
    }
}
