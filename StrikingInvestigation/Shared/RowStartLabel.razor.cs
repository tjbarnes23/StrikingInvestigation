using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class RowStartLabel
    {
        string strokeFontSizeStr;
        string strokeLabelLeftPosStr;
        string strokeLabelTopPosStr;
        string rowStartLabelWidthStr;
        string rowStartLabelLeftPosStr;
        string rowStartLabelTopPosStr;
        string rowStartLabelHeightStr;
        string changeFontSizeStr;
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
            double strokeLabelXPos = Screen.XMargin + TestSpec.StrokeLabelXOffset;
            double strokeLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin + TestSpec.StrokeLabelYOffset;

            strokeFontSizeStr = Convert.ToInt32((double)TestSpec.FontSize * 1.5).ToString() + "px";
            strokeLabelLeftPosStr = Convert.ToInt32(strokeLabelXPos).ToString() + "px";
            strokeLabelTopPosStr = Convert.ToInt32(strokeLabelYPos).ToString() + "px";

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
            
            double rowStartLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;

            rowStartLabelWidthStr = TestSpec.RowStartLabelWidth.ToString() + "px";
            rowStartLabelLeftPosStr = Convert.ToInt32(rowStartLabelXPos - (TestSpec.RowStartLabelWidth / 2))
                    .ToString() + "px";
            rowStartLabelTopPosStr = Convert.ToInt32(rowStartLabelYPos - (TestSpec.RowStartLabelHeight / 2))
                    .ToString() + "px";
            rowStartLabelHeightStr = TestSpec.RowStartLabelHeight.ToString() + "px";


            double changeLabelXPos = Screen.XMargin + TestSpec.ChangeLabelXOffset;
            double changeLabelYPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin + TestSpec.ChangeLabelYOffset;

            changeFontSizeStr = TestSpec.FontSize.ToString() + "px";
            changeLabelLeftPosStr = Convert.ToInt32(changeLabelXPos).ToString() + "px";
            changeLabelTopPosStr = Convert.ToInt32(changeLabelYPos).ToString() + "px";
        }
    }
}
