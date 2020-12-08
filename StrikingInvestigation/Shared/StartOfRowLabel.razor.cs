using System;
using Microsoft.AspNetCore.Components;
using StrikingInvestigation.Models;

namespace StrikingInvestigation.Shared
{
    public partial class StartOfRowLabel
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
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        protected override void OnInitialized()
        {
            double strokeLabelXPos = Screen.XMargin + Screen.StrokeLabelXOffset;
            double strokeLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + Screen.StrokeLabelYOffset;

            strokeFontSizeStr = Convert.ToInt32((double)Screen.FontSize * 1.5).ToString() + "px";
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
                rowStartLabelXPos = (Screen.BaseGap * Screen.XScale) + Screen.XMargin;
            }
            
            double rowStartLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin;

            rowStartLabelWidthStr = Screen.RowStartLabelWidth.ToString() + "px";
            rowStartLabelLeftPosStr = Convert.ToInt32(rowStartLabelXPos - (Screen.RowStartLabelWidth / 2))
                    .ToString() + "px";
            rowStartLabelTopPosStr = Convert.ToInt32(rowStartLabelYPos - (Screen.RowStartLabelHeight / 2))
                    .ToString() + "px";
            rowStartLabelHeightStr = Screen.RowStartLabelHeight.ToString() + "px";


            double changeLabelXPos = Screen.XMargin + Screen.ChangeLabelXOffset;
            double changeLabelYPos = (Blow.RowNum * Screen.YScale) + Screen.YMargin + Screen.ChangeLabelYOffset;

            changeFontSizeStr = Screen.FontSize.ToString() + "px";
            changeLabelLeftPosStr = Convert.ToInt32(changeLabelXPos).ToString() + "px";
            changeLabelTopPosStr = Convert.ToInt32(changeLabelYPos).ToString() + "px";
        }
    }
}
