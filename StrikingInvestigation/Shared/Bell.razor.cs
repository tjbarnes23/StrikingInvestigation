using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class Bell
    {
        // The first two fields below correspond to values in the bell CSS class
        readonly int borderSize = 10;
        readonly int fontSize = 14;
        readonly int paddingAdj = -3;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        string DiameterStr { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

        string Padding { get; set; }

        protected override void OnParametersSet()
        {
            double diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;
            DiameterStr = Convert.ToInt32(diameter).ToString() + "px";

            if (Blow.IsHandstroke)
            {
                LeftPos = Convert.ToInt32((Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        Screen.XMargin).ToString() + "px";
            }
            else
            {
                LeftPos = Convert.ToInt32((Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        (Screen.BaseGap * Screen.XScale) + Screen.XMargin).ToString() + "px";
            }

            TopPos = Convert.ToInt32((Blow.RowNum * Screen.YScale) - (diameter / 2) +
                    Screen.YMargin).ToString() + "px";

            double padding = ((diameter - (borderSize * 2) - fontSize) / 2) + paddingAdj;
            Padding = Convert.ToInt32(padding).ToString() + "px";
        }
    }
}
