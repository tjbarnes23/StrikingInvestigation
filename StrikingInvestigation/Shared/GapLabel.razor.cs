using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class GapLabel
    {
        private readonly int xOffset = -17;
        private readonly int yOffset = 32;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; set; }

        public string TopPos { get; set; }

        public string BgColor { get; set; }

        public string GapStr { get; set; }

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
                BgColor = "yellow";
                    
            }
            else
            {
                BgColor = "white";
            }

            GapStr = Blow.Gap.ToString();
        }
    }
}
