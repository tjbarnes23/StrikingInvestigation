using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    partial class PlayBtn
    {
        [Parameter]
        public Blow Blow { get; set; }

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

            double xAdj = 85;
            double yAdj = (diameter / 2) + (13 * Screen.DiameterScale) + 5;

            LeftPos = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            TopPos = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        async Task PlayClick()
        {
            await Callback.InvokeAsync(true);
        }
    }
}
