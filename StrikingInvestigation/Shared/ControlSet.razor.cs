using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class ControlSet
    {
        string boundaryRowLeftPosStr;
        string boundaryRowTopPosStr;
        
        string boundaryLabelWidthStr;
        string boundaryLabelHeightStr;

        string altGapLabelStr;
        string altGapLabelFontSizeStr;
        double altGapLabelMarginTop;
        string altGapLabelMarginTopStr;

        string buttonRowLeftPosStr;
        string buttonRowTopPosStr;

        readonly string gapMinusStr = "-" + Constants.Rounding.ToString() + "ms";
        readonly string gapPlusStr = "+" + Constants.Rounding.ToString() + "ms";

        string submitRowLeftPosStr;
        string submitRowTopPosStr;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public EventCallback<CallbackParam> Callback { get; set; }

        protected override void OnParametersSet()
        {
            double diameter;
            double xPos;
            double yPos;
            double xAdj;
            double yAdj;

            // Boundary row
            int zeroGap = Blow.GapCumulativeRow - Blow.Gap;

            if (Blow.IsHandstroke == true)
            {
                xPos = (zeroGap * Screen.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((zeroGap + Screen.BaseGap) * Screen.XScale) + Screen.XMargin;
            }

            yPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            // xAdj adds gapMin and also allows for the width of the boundary box (which is 3px)
            xAdj = (Screen.GapMin * Screen.XScale) - (diameter / 2) - 3;

            // -3 is the width of the boundary box
            yAdj = ((diameter / 2) * -1) - 3;

            boundaryRowLeftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            boundaryRowTopPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";

            boundaryLabelWidthStr = Convert.ToInt32(((Screen.GapMax - Screen.GapMin) * Screen.XScale) +
                    diameter + 6).ToString() + "px";
            boundaryLabelHeightStr = Convert.ToInt32(diameter + 6).ToString() + "px";

            altGapLabelStr = Blow.AltGap.ToString() + "ms";
            altGapLabelFontSizeStr = (Screen.FontSize - 1).ToString() + "px";

            altGapLabelMarginTop = Math.Max((diameter / 2) - Screen.FontSize, 0);
            altGapLabelMarginTopStr = Convert.ToInt32(altGapLabelMarginTop).ToString() + "px";

            // Button and submit rows
            int midGap = Blow.GapCumulativeRow - Blow.Gap + Convert.ToInt32((Screen.GapMax - Screen.GapMin) /
                    (double)2) + Screen.GapMin;

            if (Blow.IsHandstroke == true)
            {
                xPos = (midGap * Screen.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((midGap + Screen.BaseGap) * Screen.XScale) + Screen.XMargin;
            }

            yPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            // Button row
            xAdj = -160;
            yAdj = (diameter / 2) + (13 * Screen.DiameterScale) + 5;

            buttonRowLeftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            buttonRowTopPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";

            // Submit row
            xAdj = -70;
            yAdj = (diameter / 2) + (13 * Screen.DiameterScale) + 58;

            submitRowLeftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            submitRowTopPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        async Task GapMinusClick()
        {
            int newGap = Blow.Gap - Constants.Rounding;

            if (newGap >= Screen.GapMin)
            {
                Blow.UpdateGap(newGap);
                await Callback.InvokeAsync(CallbackParam.GapMinus);
            }
        }

        async Task GapPlusClick()
        {
            int newGap = Blow.Gap + Constants.Rounding;

            if (newGap <= Screen.GapMax)
            {
                Blow.UpdateGap(newGap);
                await Callback.InvokeAsync(CallbackParam.GapPlus);
            }
        }

        async Task PlayClick()
        {
            await Callback.InvokeAsync(CallbackParam.Play);
        }

        async Task SubmitClick()
        {
            await Callback.InvokeAsync(CallbackParam.Submit);
        }
    }
}
