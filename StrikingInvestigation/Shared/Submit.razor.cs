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
            double xAdj;
            double yAdj;
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

                // Controls are 307px wide. -81 will center the submit button on the current gap label
                xAdj = -81;
            }
            else
            {
                xPos = x2Pos;

                // Controls are 307px wide. -234 will center the submit button on the current gap label
                xAdj = -234;
            }

            // 48 is height of buttons in previous row (39) + margin of 9
            yAdj = (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + 5 + 48;

            leftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            topPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        async Task SubmitClick()
        {
            await Callback.InvokeAsync(CallbackParam.Submit);
        }
    }
}
