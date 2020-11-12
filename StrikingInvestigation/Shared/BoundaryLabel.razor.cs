﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class BoundaryLabel
    {
        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        public string LeftPos { get; private set; }

        public string TopPos { get; private set; }

        public string Width { get; private set; }

        public string Height { get; private set; }

        protected override void OnParametersSet()
        {
            int zeroGap = Blow.GapCumulativeRow - Blow.Gap;
            double xPos;

            if (Blow.IsHandstroke == true)
            {
                xPos = (zeroGap * Screen.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = (zeroGap * Screen.XScale) + (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }

            double yPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            double diameter = Diam.Diameter(Blow.BellActual) * Screen.DiameterScale;

            double xAdj = (Screen.GapMin * Screen.XScale) - (diameter / 2) - 3;
            double yAdj = ((diameter / 2) * -1) - 3;

            LeftPos = Convert.ToInt32(xPos + xAdj).ToString() + "px";
            TopPos = Convert.ToInt32(yPos + yAdj).ToString() + "px";

            Width = Convert.ToInt32(((Screen.GapMax - Screen.GapMin) * Screen.XScale) +
                    diameter + 6).ToString() + "px";
            Height = Convert.ToInt32(diameter + 6).ToString() + "px";
        }
    }
}