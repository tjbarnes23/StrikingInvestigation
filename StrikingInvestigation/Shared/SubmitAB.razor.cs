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
            int numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            int xPos = Convert.ToInt32(TestSpec.BaseGap * (numBells + 2) * TestSpec.XScale) + Screen.XMargin - 160;
            int yPos = Convert.ToInt32((TestSpec.NumRows + 1) * TestSpec.YScale) + Screen.YMargin - 10;

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
