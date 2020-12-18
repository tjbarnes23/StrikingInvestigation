using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

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
            int numBells;
            double x1Pos;
            double y1Pos;
            double x2Pos;
            double y2Pos;
            double tenorDiameter;

            // Set audio box coordinates
            numBells = TestSpec.Stage + (TestSpec.Stage % 2);

            x1Pos = (TestSpec.BaseGap * TestSpec.XScale) + Screen.XMargin;
            y1Pos = TestSpec.YScale + Screen.YMargin;

            x2Pos = (TestSpec.BaseGap * (numBells + 2) * TestSpec.XScale) + Screen.XMargin;
            y2Pos = (TestSpec.NumRows * TestSpec.YScale) + Screen.YMargin;

            // Adjust y coordinates
            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;
            y1Pos -= (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + TestSpec.FontPaddingTop + 4;
            y2Pos += (tenorDiameter / 2) + 1 + (TestSpec.FontSize - 2) + TestSpec.FontPaddingTop + 4;

            boundaryLeftPos = Convert.ToInt32(x1Pos).ToString() + "px";
            boundaryTopPos = Convert.ToInt32(y1Pos).ToString() + "px";

            boundaryWidth = Convert.ToInt32(x2Pos - x1Pos).ToString() + "px";
            boundaryHeight = Convert.ToInt32(y2Pos - y1Pos).ToString() + "px";

            // Set audio bar coordinates
            double x3Pos = x1Pos + 50;
            double x4Pos = x2Pos - 50;
            double y3Pos = (y1Pos + y2Pos) / 2;

            barLeftPos = Convert.ToInt32(x3Pos).ToString() + "px";

            // 3 is half the width of the audio bar
            barTopPos = Convert.ToInt32(y3Pos - 3).ToString() + "px";

            barWidth = Convert.ToInt32(x4Pos - x3Pos).ToString() + "px";

            // Set audio dot coordinates
            dotLeftStartPos = (x3Pos - 12).ToString() + "px";
            dotLeftEndPos = (x4Pos - 12).ToString() + "px";
            dotTopPos = (y3Pos - 12).ToString() + "px";

            durationStr = Screen.AnimationDuration.ToString() + "ms";

            msgLeftPos = x3Pos.ToString() + "px";
            msgTopPos = (y3Pos + 6).ToString() + "px";
        }
    }
}
