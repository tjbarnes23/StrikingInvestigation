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
            double xPos;
            double yPos;
            double xAdj;
            double yAdj;

            // Submit row
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
            
            xAdj = -70;
            yAdj = (diameter / 2) + (13 * TestSpec.DiameterScale) + 58;

            leftPosStr = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            topPosStr = Convert.ToInt32(yPos + yAdj).ToString() + "px";
        }

        async Task SubmitClick()
        {
            await Callback.InvokeAsync(CallbackParam.Submit);
        }
    }
}
