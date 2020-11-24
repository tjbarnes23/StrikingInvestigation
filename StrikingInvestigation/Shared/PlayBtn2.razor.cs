using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    partial class PlayBtn2
    {
        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public string PlayLabel { get; set; }

        [Parameter]
        public bool PlayDisabled { get; set; }

        [Parameter]
        public EventCallback<bool> Callback { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

        protected override void OnParametersSet()
        {
            int numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            int xPos = Convert.ToInt32(Screen.BaseGap * (numBells + 2) * Screen.XScale) + Screen.XMargin - 105;
            int yPos = Convert.ToInt32((TestSpec.NumRows + 1) * Screen.YScale) + Screen.YMargin - 45;

            LeftPos = xPos.ToString() + "px";
            TopPos = yPos.ToString() + "px";
        }

        async Task PlayClick()
        {
            await Callback.InvokeAsync(true);
        }
    }
}
