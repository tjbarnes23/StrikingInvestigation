using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class RowStartLabel
    {
        // These fields correspond to the startOfRowLabel CSS class dimensions
        private readonly int labelWidth = 6;
        private readonly int labelHeight = 60;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        protected override void OnInitialized()
        {
            double rowStartLabelXPos;

            // Set values for StartOfRow label
            if (Blow.IsHandstroke == true)
            {
                rowStartLabelXPos = Screen.XMargin;
            }
            else
            {
                rowStartLabelXPos = (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }

            double rowStartLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            LeftPos = Convert.ToInt32(rowStartLabelXPos - (labelWidth / 2)).ToString() + "px";
            TopPos = Convert.ToInt32(rowStartLabelYPos - (labelHeight / 2)).ToString() + "px";
        }
    }
}
