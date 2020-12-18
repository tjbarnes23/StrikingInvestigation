using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class RowStartLabel
    {
        string strokeLabelFontSizeStr;
        string strokeLabelLeftPosStr;
        string strokeLabelTopPosStr;
        string rowStartLabelWidthStr;
        string rowStartLabelLeftPosStr;
        string rowStartLabelTopPosStr;
        string rowStartLabelHeightStr;
        string changeLabelFontSizeStr;
        string changeLabelLeftPosStr;
        string changeLabelTopPosStr;

        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        protected override void OnInitialized()
        {
            // Stroke label
            double strokeLabelXPos = Screen.XMargin + TestSpec.StrokeLabelXOffset;
            double strokeLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin + TestSpec.StrokeLabelYOffset;
            
            strokeLabelLeftPosStr = Convert.ToInt32(strokeLabelXPos).ToString() + "px";
            strokeLabelTopPosStr = Convert.ToInt32(strokeLabelYPos).ToString() + "px";

            strokeLabelFontSizeStr = Convert.ToInt32(TestSpec.FontSize * 1.5).ToString() + "px";

            // Row start label
            double rowStartLabelXPos;

            // Set values for StartOfRow label
            if (Blow.IsHandstroke == true)
            {
                rowStartLabelXPos = Screen.XMargin;
            }
            else
            {
                rowStartLabelXPos = (TestSpec.BaseGap * TestSpec.XScale) + Screen.XMargin;
            }

            rowStartLabelXPos -= TestSpec.RowStartLabelWidth / (double)2;

            double rowStartLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;

            rowStartLabelYPos -= TestSpec.RowStartLabelHeight / (double)2;
            
            rowStartLabelLeftPosStr = Convert.ToInt32(rowStartLabelXPos).ToString() + "px";
            rowStartLabelTopPosStr = Convert.ToInt32(rowStartLabelYPos).ToString() + "px";
            rowStartLabelWidthStr = TestSpec.RowStartLabelWidth.ToString() + "px";
            rowStartLabelHeightStr = TestSpec.RowStartLabelHeight.ToString() + "px";

            // Change label
            double changeLabelXPos = Screen.XMargin + TestSpec.ChangeLabelXOffset;
            double changeLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin + TestSpec.ChangeLabelYOffset;

            
            changeLabelLeftPosStr = Convert.ToInt32(changeLabelXPos).ToString() + "px";
            changeLabelTopPosStr = Convert.ToInt32(changeLabelYPos).ToString() + "px";

            changeLabelFontSizeStr = TestSpec.FontSize.ToString() + "px";
        }
    }
}
