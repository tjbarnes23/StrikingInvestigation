using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Pages
{
    public partial class Submissions
    {
        IEnumerable<TestSubmission> testSubmissions;

        [Inject]
        HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            testSubmissions = (await Http.GetFromJsonAsync<TestSubmission[]>("api/testsubmissions")).ToList();
        }
    }
}
