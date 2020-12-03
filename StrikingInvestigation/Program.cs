using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton<Viewport>();

            builder.Services.AddSingleton<Device>();

            builder.Services.AddHttpClient<TJBarnesService>(client =>
            {
                client.BaseAddress = new Uri("https://tjbarnes.com/");
            });

            /* This is part of the Blazor WebAssembly template. Not needed for this project
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });
            */

            await builder.Build().RunAsync();
        }
    }
}
