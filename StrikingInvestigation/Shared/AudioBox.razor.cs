using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class AudioBox
    {
        string boundaryLeftPos;
        string boundaryTopPos;
        string boundaryWidth;
        string boundaryHeight;
        string barLeftPos;
        string barTopPos;
        string barWidth;
        string dotLeftStartPos;
        string dotLeftEndPos;
        string dotTopPos;
        string durationStr;
        string msgLeftPos;
        string msgTopPos;

        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        protected override void OnParametersSet()
        {
            int numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            int x1Pos = Convert.ToInt32(TestSpec.BaseGap * TestSpec.XScale) + Screen.XMargin + 30;
            int y1Pos = Screen.YMargin + 50;

            int x2Pos = Convert.ToInt32(TestSpec.BaseGap * (numBells + 2) * TestSpec.XScale) + Screen.XMargin - 30;
            int y2Pos = Convert.ToInt32((TestSpec.NumRows + 1) * TestSpec.YScale) + Screen.YMargin - 50 ;

            boundaryLeftPos = x1Pos.ToString() + "px";
            boundaryTopPos = y1Pos.ToString() + "px";

            boundaryWidth = (x2Pos - x1Pos).ToString() + "px";
            boundaryHeight = (y2Pos - y1Pos).ToString() + "px";

            int x3Pos = x1Pos + 50;
            int y3Pos = (y1Pos + y2Pos) / 2;

            int x4Pos = x2Pos - 50;

            barLeftPos = x3Pos.ToString() + "px";
            barTopPos = y3Pos.ToString() + "px";

            barWidth = (x4Pos - x3Pos).ToString() + "px";

            dotLeftStartPos = (x3Pos - 14).ToString() + "px";
            dotLeftEndPos = (x4Pos - 14).ToString() + "px";
            dotTopPos = (y3Pos - 14).ToString() + "px";

            durationStr = Screen.AnimationDuration.ToString() + "ms";

            msgLeftPos = x3Pos.ToString() + "px";
            msgTopPos = (y3Pos + 6).ToString() + "px";
        }
    }
}
