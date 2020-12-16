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
            int xPos = Convert.ToInt32((TestSpec.BaseGap / (double)2) * TestSpec.XScale) + Screen.XMargin;
            int yPos = Convert.ToInt32((TestSpec.NumRows + 1) * TestSpec.YScale) + Screen.YMargin;

            leftPosStr = xPos.ToString() + "px";
            topPosStr = yPos.ToString() + "px";
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
