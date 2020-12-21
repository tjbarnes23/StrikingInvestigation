using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class NavMenu
    {
        readonly string[] bgColor;
        readonly string[] color;
        bool collapseNavMenu = true;
        ElementReference title;

        public NavMenu()
        {
            bgColor = new string[10];
            color = new string[10];
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        NavigationManager NavManager { get; set; }

        protected override void OnInitialized()
        {
            MenuClick(Menu.Home);
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", title);
        }

        void MenuClick(Menu menu)
        {
            // Set all colors to unselected
            for (int i = 0; i < 10; i++)
            {
                bgColor[i] = "transparent";
                color[i] = "#d7d7d7";
            }

            // Menu.Title is processed as Menu.Home
            if (menu == Menu.Title)
            {
                menu = Menu.Home;
            }

            // Set color for selected item and navigate to page
            bgColor[(int)menu] = "rgba(255, 255, 255, 0.3)";
            color[(int)menu] = "white";

            switch (menu)
            {
                case Menu.Home:
                    NavManager.NavigateTo("/");
                    break;

                case Menu.Tests:
                    NavManager.NavigateTo("/gaptest");
                    break;

                case Menu.Data:
                    NavManager.NavigateTo("/submissions");
                    break;

                case Menu.Admin:
                    NavManager.NavigateTo("/account");
                    break;

                default:
                    NavManager.NavigateTo("/");
                    break;
            }

            StateHasChanged();
        }

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        string NavMenuCssClass()
        {
            if (collapseNavMenu == true)
            {
                return "collapse";
            }
            else
            {
                return null;
            }
        }
    }
}
