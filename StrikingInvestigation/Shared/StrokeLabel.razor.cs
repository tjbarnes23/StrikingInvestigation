using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class StrokeLabel
    {
        readonly int strokeLabelXOffset = -80;
        readonly int strokeLabelYOffset = -18;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

        protected override void OnInitialized()
        {
            double strokeLabelXPos = Screen.XMargin + strokeLabelXOffset;
            double strokeLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + strokeLabelYOffset;

            LeftPos = Convert.ToInt32(strokeLabelXPos).ToString() + "px";
            TopPos = Convert.ToInt32(strokeLabelYPos).ToString() + "px";
        }
    }
}
