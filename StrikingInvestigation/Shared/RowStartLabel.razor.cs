using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class RowStartLabel
    {
        // These fields correspond to the startOfRowLabel CSS class dimensions
        readonly int labelWidth = 6;
        readonly int labelHeight = 60;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

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
