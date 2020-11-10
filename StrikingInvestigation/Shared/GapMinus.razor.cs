using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class GapMinus
    {
        private readonly string gapMinusStr = "-" + Constants.Rounding.ToString() + "ms";

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public bool GapDisabled { get; set; }

        [Parameter]
        public EventCallback<bool> Callback { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        protected override void OnParametersSet()
        {
            int midGap = Blow.GapCumulativeRow - Blow.Gap + Convert.ToInt32((Screen.GapMax - Screen.GapMin) /
                    (double)2) + Screen.GapMin;
            double xPos;

            if (Blow.IsHandstroke == true)
            {
                xPos = (midGap * Screen.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = (midGap * Screen.XScale) + (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }

            double yPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            double diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            double xAdj = -160;
            double yAdj = (diameter / 2) + (13 * Screen.DiameterScale) + 5;

            LeftPos = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            TopPos = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        protected async Task GapMinusClick()
        {
            int newGap = Blow.Gap - Constants.Rounding;

            if (newGap >= Screen.GapMin)
            {
                Blow.UpdateGap(newGap);
                await Callback.InvokeAsync(true);
            }
        }
    }
}
