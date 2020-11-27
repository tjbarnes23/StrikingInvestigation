using System.Net.Http;

namespace StrikingInvestigation.Utilities
{
    public class TJBarnesService
    {
        readonly HttpClient httpClient;

        public TJBarnesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return httpClient;
        }
    }
}
