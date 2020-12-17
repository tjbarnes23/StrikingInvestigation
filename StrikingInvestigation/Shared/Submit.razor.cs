using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class Submit
    {
        string leftPosStr;
        string topPosStr;

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
            double xPos;
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

            // Calculate x coordinate of right side of boundary box
            x2Pos = x1Pos + ((TestSpec.GapMax - TestSpec.GapMin) * TestSpec.XScale) +
                    diameter + 6;

            // Submit row
            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;

            if (TestSpec.ButtonsCentered == true)
            {
                xPos = x1Pos + ((x2Pos - x1Pos) / 2);

                // Controls are 293px wide. -78 will center the submit button on the current gap label
                xPos += -78;
            }
            else
            {
                xPos = x2Pos;

                // Controls are 293px wide. -224 will center the submit button on the current gap label
                xPos += -224;
            }

            // Make sure xPos isn't too close to left hand edge of screen
            if (xPos < 10)
            {
                xPos = 10;
            }

            // 45 is height of buttons in previous row (39) + margin of 6
            yPos += (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + 5 + 45;

            leftPosStr = Convert.ToInt32(xPos).ToString() + "px";
            topPosStr = Convert.ToInt32(yPos).ToString() + "px";
        }

        async Task SubmitClick()
        {
            await Callback.InvokeAsync(CallbackParam.Submit);
        }
    }
}
