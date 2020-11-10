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
        [Inject]
        public HttpClient Http { get; set; }

        public IEnumerable<TestSubmission> TestSubmissions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TestSubmissions = (await Http.GetFromJsonAsync<TestSubmission[]>("api/testsubmissions")).ToList();
        }
    }
}
