using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class Play
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

        [Parameter]
        public TestSpec TestSpec { get; set; }

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
                xPos = (zeroGap * TestSpec.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((zeroGap + TestSpec.BaseGap) * TestSpec.XScale) + Screen.XMargin;
            }

            yPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;

            diameter = Diam.Diameter(Blow.BellActual) * TestSpec.DiameterScale;

            // xAdj adds gapMin and also allows for the width of the boundary box (which is 3px)
            xAdj = (TestSpec.GapMin * TestSpec.XScale) - (diameter / 2) - 3;

            // -3 is the width of the boundary box
            yAdj = ((diameter / 2) * -1) - 3;

            boundaryRowLeftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            boundaryRowTopPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";

            boundaryLabelWidthStr = Convert.ToInt32(((TestSpec.GapMax - TestSpec.GapMin) * TestSpec.XScale) +
                    diameter + 6).ToString() + "px";
            boundaryLabelHeightStr = Convert.ToInt32(diameter + 6).ToString() + "px";

            altGapLabelStr = Blow.AltGap.ToString() + "ms";
            altGapLabelFontSizeStr = (TestSpec.FontSize - 1).ToString() + "px";

            altGapLabelMarginTop = Math.Max((diameter / 2) - TestSpec.FontSize, 0);
            altGapLabelMarginTopStr = Convert.ToInt32(altGapLabelMarginTop).ToString() + "px";

            // Button row
            int midGap = Blow.GapCumulativeRow - Blow.Gap + Convert.ToInt32((TestSpec.GapMax - TestSpec.GapMin) /
                    (double)2) + TestSpec.GapMin;

            if (Blow.IsHandstroke == true)
            {
                xPos = (midGap * TestSpec.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((midGap + TestSpec.BaseGap) * TestSpec.XScale) + Screen.XMargin;
            }

            yPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;

            diameter = Diam.Diameter(Blow.BellActual) * TestSpec.DiameterScale;

            xAdj = -160;
            yAdj = (diameter / 2) + (13 * TestSpec.DiameterScale) + 5;

            buttonRowLeftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            buttonRowTopPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        async Task GapMinusClick()
        {
            int newGap = Blow.Gap - Constants.Rounding;

            if (newGap >= TestSpec.GapMin)
            {
                Blow.UpdateGap(newGap);
                await Callback.InvokeAsync(CallbackParam.GapMinus);
            }
        }

        async Task GapPlusClick()
        {
            int newGap = Blow.Gap + Constants.Rounding;

            if (newGap <= TestSpec.GapMax)
            {
                Blow.UpdateGap(newGap);
                await Callback.InvokeAsync(CallbackParam.GapPlus);
            }
        }

        async Task PlayClick()
        {
            if (TestSpec.DeviceLoad == DeviceLoad.Low)
            {
                await Callback.InvokeAsync(CallbackParam.Play);
            }
            else
            {
                await Callback.InvokeAsync(CallbackParam.PlayAsync);
            }
        }
    }
}
