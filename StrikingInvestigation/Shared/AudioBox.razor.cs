using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class AudioBox
    {
        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public int Duration { get; set; }

        string BoundaryLeftPos { get; set; }

        string BoundaryTopPos { get; set; }

        string BoundaryWidth { get; set; }

        string BoundaryHeight { get; set; }

        string BarLeftPos { get; set; }

        string BarTopPos { get; set; }

        string BarWidth { get; set; }

        string DotLeftStartPos { get; set; }

        string DotLeftEndPos { get; set; }

        string DotTopPos { get; set; }

        string DurationStr { get; set; }

        protected override void OnParametersSet()
        {
            int numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            int x1Pos = Convert.ToInt32(Screen.BaseGap * Screen.XScale) + Screen.XMargin + 30;
            int y1Pos = Screen.YMargin + 50;

            int x2Pos = Convert.ToInt32(Screen.BaseGap * (numBells + 2) * Screen.XScale) + Screen.XMargin - 30;
            int y2Pos = Convert.ToInt32((TestSpec.NumRows + 1) * Screen.YScale) + Screen.YMargin - 50 ;

            BoundaryLeftPos = x1Pos.ToString() + "px";
            BoundaryTopPos = y1Pos.ToString() + "px";

            BoundaryWidth = (x2Pos - x1Pos).ToString() + "px";
            BoundaryHeight = (y2Pos - y1Pos).ToString() + "px";

            int x3Pos = x1Pos + 50;
            int y3Pos = (y1Pos + y2Pos) / 2;

            int x4Pos = x2Pos - 50;

            BarLeftPos = x3Pos.ToString() + "px";
            BarTopPos = y3Pos.ToString() + "px";

            BarWidth = (x4Pos - x3Pos).ToString() + "px";

            DotLeftStartPos = (x3Pos - 14).ToString() + "px";
            DotLeftEndPos = (x4Pos - 14).ToString() + "px";
            DotTopPos = (y3Pos - 14).ToString() + "px";

            DurationStr = Duration.ToString() + "ms";
        }
    }
}
