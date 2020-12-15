using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class PlayAB
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

            int xPos = Convert.ToInt32(TestSpec.BaseGap * (numBells + 2) * TestSpec.XScale) + Screen.XMargin - 105;
            int yPos = Convert.ToInt32((TestSpec.NumRows + 1) * TestSpec.YScale) + Screen.YMargin - 45;

            leftPosStr = xPos.ToString() + "px";
            topPosStr = yPos.ToString() + "px";
        }

        async Task PlayClick()
        {
            if (TestSpec.DeviceLoad == DeviceLoad.Low)
            {
                if (Screen.IsA == true)
                {
                    await Callback.InvokeAsync(CallbackParam.PlayA);
                }
                else
                {
                    await Callback.InvokeAsync(CallbackParam.PlayB);
                }
            }
            else
            {
                if (Screen.IsA == true)
                {
                    await Callback.InvokeAsync(CallbackParam.PlayAsyncA);
                }
                else
                {
                    await Callback.InvokeAsync(CallbackParam.PlayAsyncB);
                }
            }
        }
    }
}
