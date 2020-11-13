using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class SpinnerPlay2
    {
        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        protected override void OnParametersSet()
        {
            int numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            int xPos = Convert.ToInt32(Screen.BaseGap * (numBells + 2) * Screen.XScale) + Screen.XMargin - 18;
            int yPos = Convert.ToInt32((TestSpec.NumRows + 1) * Screen.YScale) + Screen.YMargin - 38;

            LeftPos = xPos.ToString() + "px";
            TopPos = yPos.ToString() + "px";
        }
    }
}
