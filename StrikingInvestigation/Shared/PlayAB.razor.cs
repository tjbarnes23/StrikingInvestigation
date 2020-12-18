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
            int numBells;
            double xPos;
            double yPos;
            double tenorDiameter;

            numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            xPos = (TestSpec.BaseGap * (numBells + 2) * TestSpec.XScale) + Screen.XMargin - 75;
            yPos = (TestSpec.NumRows * TestSpec.YScale) + Screen.YMargin;

            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;
            
            yPos += (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + TestSpec.FontPaddingTop + 10;

            leftPosStr = Convert.ToInt32(xPos).ToString() + "px";
            topPosStr = Convert.ToInt32(yPos).ToString() + "px";
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
