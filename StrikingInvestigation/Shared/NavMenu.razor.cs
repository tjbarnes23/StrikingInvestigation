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
        bool expandNavMenu;
        ElementReference title;

        public NavMenu()
        {
            bgColor = new string[12];
            color = new string[12];
            subMenuActive = new bool[12];
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
                    SetMenus(menu);
                    NavManager.NavigateTo("/");
                    break;

                case Menu.Tests:
                    if (subMenuActive[(int)Menu.Tests] == true)
                    {
                        // Menu clicked for a second time - close the dropdown list
                        subMenuActive[(int)Menu.Tests] = false;
                    }
                    else
                    {
                        // Close all dropdown lists
                        for (int i = 0; i < 12; i++)
                        {
                            subMenuActive[i] = false;
                        }

                        // Open dropdown list
                        subMenuActive[(int)Menu.Tests] = true;
                    }
                    
                    break;

                case Menu.GapTests:
                    SetMenus(menu);
                    NavManager.NavigateTo("/gaptest");
                    break;

                case Menu.ABTests:
                    SetMenus(menu);
                    NavManager.NavigateTo("/abtest");
                    break;

                case Menu.AVTests:
                    // Set all colors to unselected and close all dropdowns
                    SetMenus(menu);
                    NavManager.NavigateTo("/avtest");
                    break;

                case Menu.Data:
                    if (subMenuActive[(int)Menu.Data] == true)
                    {
                        // Menu clicked for a second time - close the dropdown list
                        subMenuActive[(int)Menu.Data] = false;
                    }
                    else
                    {
                        // Close all dropdown lists
                        for (int i = 0; i < 12; i++)
                        {
                            subMenuActive[i] = false;
                        }

                        // Open dropdown list
                        subMenuActive[(int)Menu.Data] = true;
                    }

                    break;

                case Menu.Submissions:
                    // Set all colors to unselected and close all dropdowns
                    SetMenus(menu);
                    NavManager.NavigateTo("/submissions");
                    break;

                case Menu.Results:
                    // Set all colors to unselected and close all dropdowns
                    SetMenus(menu);
                    NavManager.NavigateTo("/results");
                    break;

                case Menu.Admin:
                    if (subMenuActive[(int)Menu.Admin] == true)
                    {
                        // Menu clicked for a second time - close the dropdown list
                        subMenuActive[(int)Menu.Admin] = false;
                    }
                    else
                    {
                        // Close all dropdown lists
                        for (int i = 0; i < 12; i++)
                        {
                            subMenuActive[i] = false;
                        }

                        // Open dropdown list
                        subMenuActive[(int)Menu.Admin] = true;
                    }

                    break;

                case Menu.Account:
                    SetMenus(menu);
                    NavManager.NavigateTo("/account");
                    break;

                case Menu.Settings:
                    SetMenus(menu);
                    NavManager.NavigateTo("/settings");
                    break;

                case Menu.Privacy:
                    SetMenus(menu);
                    NavManager.NavigateTo("/privacy");
                    break;

                default:
                    SetMenus(Menu.Home);
                    NavManager.NavigateTo("/");
                    break;
            }
        }

        void SetMenus(Menu menu)
        {
            // Set all colors to unselected and close all dropdowns
            for (int i = 0; i < 12; i++)
            {
                bgColor[i] = "transparent";
                color[i] = "#d7d7d7";
                subMenuActive[i] = false;
            }

            // Set colors for selected menu item
            bgColor[(int)menu] = "rgba(255, 255, 255, 0.3)";
            color[(int)menu] = "white";

            // Set menu group colors if applicable
            if (menu == Menu.GapTests || menu == Menu.ABTests || menu == Menu.AVTests)
            {
                bgColor[(int)Menu.Tests] = "rgba(255, 255, 255, 0.3)";
                color[(int)Menu.Tests] = "white";
            }
            else if (menu == Menu.Submissions || menu == Menu.Results)
            {
                bgColor[(int)Menu.Data] = "rgba(255, 255, 255, 0.3)";
                color[(int)Menu.Data] = "white";
            }
            else if (menu == Menu.Account || menu == Menu.Settings || menu == Menu.Privacy)
            {
                bgColor[(int)Menu.Admin] = "rgba(255, 255, 255, 0.3)";
                color[(int)Menu.Admin] = "white";
            }

            // Set the menu togger to closed
            expandNavMenu = false;
        }

        void ToggleNavMenu()
        {
            expandNavMenu = !expandNavMenu;
        }
    }
}
