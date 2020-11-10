using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class StrokeLabel
    {
        private readonly int strokeLabelXOffset = -80;
        private readonly int strokeLabelYOffset = -18;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        protected override void OnInitialized()
        {
            double strokeLabelXPos = Screen.XMargin + strokeLabelXOffset;
            double strokeLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + strokeLabelYOffset;

            LeftPos = Convert.ToInt32(strokeLabelXPos).ToString() + "px";
            TopPos = Convert.ToInt32(strokeLabelYPos).ToString() + "px";
        }
    }
}
