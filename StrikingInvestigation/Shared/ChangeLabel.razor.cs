using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class ChangeLabel
    {
        private readonly int changeLabelXOffset = -50;
        private readonly int changeLabelYOffset = -62;

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        protected override void OnInitialized()
        {
            double startOfRowLabelXPos = Screen.XMargin + changeLabelXOffset;
            double startOfRowLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + changeLabelYOffset;

            LeftPos = Convert.ToInt32(startOfRowLabelXPos).ToString() + "px";
            TopPos = Convert.ToInt32(startOfRowLabelYPos).ToString() + "px";
        }
    }
}
