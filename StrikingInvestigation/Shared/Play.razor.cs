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

        string altGapLeftPosStr;
        string altGapTopPosStr;
        string altGapLabelStr;
        string altGapLabelFontSizeStr;

        string gapMinusLeftPosStr;
        string gapMinusTopPosStr;

        string gapCurrLeftPosStr;
        string gapCurrTopPosStr;

        string gapPlusLeftPosStr;
        string gapPlusTopPosStr;

        string playLeftPosStr;
        string playTopPosStr;

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
            double x1Pos;
            double x2Pos;
            double yPos;
            double y1Pos;
            double y2Pos;
            double xPos;
            double boundaryLabelWidth;
            double boundaryLabelHeight;
            double altGapLeftPos;
            double altGapTopPos;
            double tenorDiameter;

            // Work out x coordinate of previous blow (i.e. zero gap)
            int zeroGap = Blow.GapCumulativeRow - Blow.Gap;
            
            if (Blow.IsHandstroke == true)
            {
                x1Pos = (zeroGap * TestSpec.XScale) + Screen.XMargin;
            }
            else
            {
                x1Pos = ((zeroGap + TestSpec.BaseGap) * TestSpec.XScale) + Screen.XMargin;
            }

            yPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;
            
            diameter = Diam.Diameter(Blow.BellActual) * TestSpec.DiameterScale;

            // Adjust x1Pos for gapMin, the diameter of the bell, and the width of the boundary box (which is 3px)
            x1Pos += (TestSpec.GapMin * TestSpec.XScale) - (diameter / 2) - 3;

            // Set y1Pos from the diameter of the bell, and the width of the boundary box (which is 3px)
            y1Pos = yPos - ((diameter / 2) + 3);
            
            // Calculate coordinates of bottom right corner of boundary box
            x2Pos = x1Pos + ((TestSpec.GapMax - TestSpec.GapMin) * TestSpec.XScale) +
                    diameter + 6;
            y2Pos = yPos + ((diameter / 2) + 3);

            // Boundary row
            boundaryRowLeftPosStr = Convert.ToInt32(x1Pos).ToString() + "px";
            boundaryRowTopPosStr = Convert.ToInt32(y1Pos).ToString() + "px";

            // Calculate width and height of boundary box
            boundaryLabelWidth = x2Pos - x1Pos;
            boundaryLabelHeight = y2Pos - y1Pos;

            boundaryLabelWidthStr = Convert.ToInt32(boundaryLabelWidth).ToString() + "px";
            boundaryLabelHeightStr = Convert.ToInt32(boundaryLabelHeight).ToString() + "px";

            // Alt gap label
            altGapLeftPos = x2Pos + 10;
            altGapTopPos = yPos - TestSpec.FontSize - 4;

            altGapLeftPosStr = Convert.ToInt32(altGapLeftPos).ToString() + "px";
            altGapTopPosStr = Convert.ToInt32(altGapTopPos).ToString() + "px";
            
            altGapLabelStr = Blow.AltGap.ToString() + "ms";
            altGapLabelFontSizeStr = (TestSpec.FontSize - 1).ToString() + "px";

            // Buttons
            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;

            if (TestSpec.ButtonsCentered == true)
            {
                xPos = x1Pos + ((x2Pos - x1Pos) / 2);

                // Controls are 293px wide. When centering, adjust by half this amount
                xPos += -146;
            }
            else
            {
                xPos = x2Pos;

                // Controls are 293px wide. When aligning right, adjust by this amount
                xPos += -293;
            }

            // Make sure xPos isn't too close to left hand edge of screen
            if (xPos < 10)
            {
                xPos = 10;
            }

            yPos += (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + TestSpec.FontPaddingTop + 10;

            gapMinusLeftPosStr = Convert.ToInt32(xPos).ToString() + "px";
            gapMinusTopPosStr = Convert.ToInt32(yPos).ToString() + "px";

            gapCurrLeftPosStr = Convert.ToInt32(xPos + 81).ToString() + "px";
            gapCurrTopPosStr = Convert.ToInt32(yPos).ToString() + "px";

            gapPlusLeftPosStr = Convert.ToInt32(xPos + 81 + 56).ToString() + "px";
            gapPlusTopPosStr = Convert.ToInt32(yPos).ToString() + "px";

            playLeftPosStr = Convert.ToInt32(xPos + 81 + 56 + 81).ToString() + "px";
            playTopPosStr = Convert.ToInt32(yPos).ToString() + "px";
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
