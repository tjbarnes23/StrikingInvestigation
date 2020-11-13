using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class GapAudioLabel
    {
        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; set; }

        public string TopPos { get; set; }

        public string AltGapStr { get; set; }

        protected override void OnParametersSet()
        {
            double diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            double xPos = Screen.XMargin;
            double xAdj = (Screen.GapMin * Screen.XScale) - (diameter / 2) - 3 - 80;

            double yPos = Screen.YScale + Screen.YMargin;
            double yAdj = - 20;

            LeftPos = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            TopPos = Convert.ToInt32(yPos + yAdj).ToString() + "px";

            AltGapStr = Blow.AltGap.ToString();
        }
    }
}
