using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    partial class GapLabel
    {
        private readonly int xOffset = -17;
        private readonly int yOffset = 32;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

        string BgColor { get; set; }

        string GapStr { get; set; }

        protected override void OnParametersSet()
        {
            double diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            if (Blow.IsHandstroke)
            {
                LeftPos = Convert.ToInt32((Blow.GapCumulativeRow * Screen.XScale) -
                        (diameter / 2) + xOffset + Screen.XMargin).ToString() + "px";
            }
            else
            {
                LeftPos = Convert.ToInt32((Blow.GapCumulativeRow * Screen.XScale) + (Screen.BaseGap * Screen.XScale) -
                        (diameter / 2) + xOffset + Screen.XMargin).ToString() + "px";
            }

            TopPos = Convert.ToInt32((Blow.RowNum * Screen.YScale) + yOffset + Screen.YMargin).ToString() + "px";
            
            if (Blow.IsHighlighted == true)
            {
                BgColor = "lightpink";
                    
            }
            else
            {
                BgColor = "white";
            }

            GapStr = Blow.Gap.ToString();
        }
    }
}
