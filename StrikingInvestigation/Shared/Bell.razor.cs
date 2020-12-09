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
        string bellLeftPosStr;
        string bellTopPosStr;
        string bellFontSizeStr;
        string paddingStr;

        string gapLabelLeftPosStr;
        string gapLabelTopPosStr;
        string gapLabelFontSizeStr;
        string bgColor;
        string gapStr;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        protected override void OnParametersSet()
        {
            // Calculations for bell
            double diameter;
            double bellLeftPos;
            double bellTopPos;
            double padding;
            
            borderWidthStr = Screen.BorderWidth.ToString() + "px";

            diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;
            diameterStr = Convert.ToInt32(diameter).ToString() + "px";

            if (Blow.IsHandstroke)
            {
                bellLeftPos = (Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        Screen.XMargin;
            }
            else
            {
                bellLeftPos = (Blow.GapCumulativeRow * Screen.XScale) - (diameter / 2) +
                        (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }

            bellLeftPosStr = Convert.ToInt32(bellLeftPos).ToString() + "px";

            bellTopPos = (Blow.RowNum * Screen.YScale) - (diameter / 2) +
                    Screen.YMargin;
            bellTopPosStr = Convert.ToInt32(bellTopPos).ToString() + "px";

            bellFontSizeStr = Screen.FontSize.ToString() + "px";

            // Need logic here for centering the bell number inside the bell.
            // For now, use a default of -3
            // Check whether alignment is centerline - may need to change that
            padding = ((diameter - (Screen.BorderWidth * 2) - Screen.FontSize) / 2) - 3;
            paddingStr = Convert.ToInt32(padding).ToString() + "px";

            // Calculations for gap label
            double tenorDiameter;
            double xPos;
            double yPos;
            double gapLabelLeftPos;
            double gapLabelTopPos;

            tenorDiameter = Diam.Diameter("T") * Screen.DiameterScale;

            if (Blow.IsHandstroke)
            {
                xPos = (Blow.GapCumulativeRow * Screen.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((Blow.GapCumulativeRow + Screen.BaseGap) * Screen.XScale) + Screen.XMargin;
            }

            gapLabelLeftPos = xPos - (tenorDiameter / 2) - 11;

            gapLabelLeftPosStr = Convert.ToInt32(gapLabelLeftPos).ToString() + "px";

            yPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            gapLabelTopPos = yPos + (tenorDiameter / 2) + 1;
            gapLabelTopPosStr = Convert.ToInt32(gapLabelTopPos).ToString() + "px";

            gapLabelFontSizeStr = (Screen.FontSize - 2).ToString() + "px";

            if (Blow.IsHighlighted == true)
            {
                bgColor = "lightpink";

            }
            else
            {
                bgColor = "white";
            }

            gapStr = Blow.Gap.ToString();
        }
    }
}
