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
        readonly bool[] subMenuActive;
        bool collapseNavMenu = true;
        ElementReference title;

        public NavMenu()
        {
            bgColor = new string[10];
            color = new string[10];
            subMenuActive = new bool[10];
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
            switch (menu)
            {
                case Menu.Home:
                    ResetMenus();
                    bgColor[(int)Menu.Home] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Home] = "white";
                    NavManager.NavigateTo("/");
                    break;

                case Menu.Tests:
                    if (subMenuActive[(int)Menu.Tests] == true)
                    {
                        // Tests menu click for a second time - close the dropdown list
                        subMenuActive[(int)Menu.Tests] = false;
                    }
                    else
                    {
                        // Close all dropdown lists
                        for (int i = 0; i < 10; i++)
                        {
                            subMenuActive[i] = false;
                        }

                        // Open dropdown list
                        subMenuActive[(int)Menu.Tests] = true;
                    }
                    
                    break;

                case Menu.GapTests:
                    ResetMenus();
                    bgColor[(int)Menu.Tests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Tests] = "white";
                    bgColor[(int)Menu.GapTests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.GapTests] = "white";
                    NavManager.NavigateTo("/gaptest");
                    break;

                case Menu.ABTests:
                    ResetMenus();
                    bgColor[(int)Menu.Tests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Tests] = "white";
                    bgColor[(int)Menu.ABTests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.ABTests] = "white";
                    NavManager.NavigateTo("/abtest");
                    break;

                case Menu.AVTests:
                    // Set all colors to unselected and close all dropdowns
                    ResetMenus();
                    bgColor[(int)Menu.Tests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Tests] = "white";
                    bgColor[(int)Menu.AVTests] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.AVTests] = "white";
                    NavManager.NavigateTo("/avtest");
                    break;

                case Menu.Data:
                    // Set all colors to unselected and close all dropdowns
                    ResetMenus();
                    bgColor[(int)Menu.Data] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Data] = "white";
                    NavManager.NavigateTo("/submissions");
                    break;

                case Menu.Admin:
                    ResetMenus();
                    bgColor[(int)Menu.Admin] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Admin] = "white";
                    NavManager.NavigateTo("/account");
                    break;

                default:
                    ResetMenus();
                    bgColor[(int)Menu.Home] = "rgba(255, 255, 255, 0.3)";
                    color[(int)Menu.Home] = "white";
                    NavManager.NavigateTo("/");
                    break;
            }

            StateHasChanged();
        }

        void ResetMenus()
        {
            // Set all colors to unselected and close all dropdowns
            for (int i = 0; i < 10; i++)
            {
                bgColor[i] = "transparent";
                color[i] = "#d7d7d7";
                subMenuActive[i] = false;
            }
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
