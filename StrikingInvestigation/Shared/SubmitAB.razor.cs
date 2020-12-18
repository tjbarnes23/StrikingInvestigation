using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class SubmitAB
    {
        string leftPosStr;
        string topPosStr;

        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public EventCallback<CallbackParam> Callback { get; set; }

        protected override void OnParametersSet()
        {
            double xPos;
            double yPos;
            double tenorDiameter;

            xPos = ((TestSpec.BaseGap / (double)2) * TestSpec.XScale) + Screen.XMargin;
            yPos = (TestSpec.NumRows * TestSpec.YScale) + Screen.YMargin;

            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;

            yPos += (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + TestSpec.FontPaddingTop + 10 + 44;

            leftPosStr = Convert.ToInt32(xPos).ToString() + "px";
            topPosStr = Convert.ToInt32(yPos).ToString() + "px";
        }

        async Task AHasErrors()
        {
            await Callback.InvokeAsync(CallbackParam.AHasErrors);
        }

        async Task BHasErrors()
        {
            await Callback.InvokeAsync(CallbackParam.BHasErrors);
        }

        async Task DontKnow()
        {
            await Callback.InvokeAsync(CallbackParam.DontKnow);
        }
    }
}
