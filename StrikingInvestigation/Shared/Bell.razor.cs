using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class Bell
    {
        // The first two fields below correspond to values in the bell CSS class
        string borderWidthStr;
        string diameterStr;
        string leftPosStr;
        string topPosStr;
        string fontSizeStr;
        string paddingStr;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        protected override void OnParametersSet()
        {
            double diameter;
            double leftPos;
            double topPos;
            double padding;

            borderWidthStr = Screen.BorderWidth.ToString() + "px";

            diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;
            diameterStr = Convert.ToInt32(diameter).ToString() + "px";

            if (Blow.IsHandstroke)
            {
                leftPos = (Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        Screen.XMargin;
            }
            else
            {
                leftPos = (Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }

            leftPosStr = Convert.ToInt32(leftPos).ToString() + "px";

            topPos = (Blow.RowNum * Screen.YScale) - (diameter / 2) +
                    Screen.YMargin;
            topPosStr = Convert.ToInt32(topPos).ToString() + "px";

            fontSizeStr = Screen.FontSize.ToString() + "px";

            // Need logic here for centering the bell number inside the bell.
            // For now, use a default of -3
            // Check whether alignment is centerline - may need to change that
            padding = ((diameter - (Screen.BorderWidth * 2) - Screen.FontSize) / 2) - 3;
            paddingStr = Convert.ToInt32(padding).ToString() + "px";
        }
    }
}
