using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class ChangeLabel
    {
        readonly int changeLabelXOffset = -50;
        readonly int changeLabelYOffset = -62;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        string LeftPos { get; set; }

        string TopPos { get; set; }

        protected override void OnInitialized()
        {
            double startOfRowLabelXPos = Screen.XMargin + changeLabelXOffset;
            double startOfRowLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + changeLabelYOffset;

            LeftPos = Convert.ToInt32(startOfRowLabelXPos).ToString() + "px";
            TopPos = Convert.ToInt32(startOfRowLabelYPos).ToString() + "px";
        }
    }
}
