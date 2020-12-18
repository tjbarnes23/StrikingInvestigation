using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using StrikingInvestigation.Models;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Shared
{
    public partial class Bell
    {
        string borderWidthStr;
        string diameterStr;
        string bellLeftPosStr;
        string bellTopPosStr;
        string bellFontSizeStr;
        string paddingTopStr;

        string gapLabelLeftPosStr;
        string gapLabelTopPosStr;
        string gapLabelFontSizeStr;
        string bgColor;
        string gapStr;

        [Parameter]
        public TestSpec TestSpec { get; set; }

        [Parameter]
        public Blow Blow { get; set; }

        [Parameter]
        public Screen Screen { get; set; }

        [Parameter]
        public EventCallback<CallbackParam> Callback { get; set; }

        protected override void OnParametersSet()
        {
            // Calculations for bell
            double diameter;
            double diameterMin;
            double bellLeftPos;
            double bellTopPos;
            double paddingTop;
            
            borderWidthStr = TestSpec.BorderWidth.ToString() + "px";

            diameter = Diam.Diameter(Blow.BellActual) * TestSpec.DiameterScale;

            // Test to see if diameter is too small to fit bell number - if so set diameter to minimum value that
            // will fit the bell number
            diameterMin = ((TestSpec.FontSize / (double)2) + TestSpec.FontPaddingTop + TestSpec.BorderWidth) * 2;

            if (diameter < diameterMin)
            {
                diameter = diameterMin;
            }

            diameterStr = Convert.ToInt32(diameter).ToString() + "px";

            if (Blow.IsHandstroke)
            {
                bellLeftPos = (Blow.GapCumulativeRow * TestSpec.XScale) - (diameter / 2) +
                        Screen.XMargin;
            }
            else
            {
                bellLeftPos = (Blow.GapCumulativeRow * TestSpec.XScale) - (diameter / 2) +
                        (TestSpec.BaseGap * TestSpec.XScale) + Screen.XMargin;
            }

            bellLeftPosStr = Convert.ToInt32(bellLeftPos).ToString() + "px";

            bellTopPos = (Blow.RowNum * TestSpec.YScale) - (diameter / 2) +
                    Screen.YMargin;
            bellTopPosStr = Convert.ToInt32(bellTopPos).ToString() + "px";

            bellFontSizeStr = TestSpec.FontSize.ToString() + "px";

            // First set paddingTop to half the interior diameter
            paddingTop = (diameter - (TestSpec.BorderWidth * 2)) / 2;

            // Fonts seem to insert padding above of about 25% of font size, so subtract this
            paddingTop -= TestSpec.FontPaddingTop;

            // Now subtract half of the font size
            paddingTop -= TestSpec.FontSize / (double)2;

            paddingTopStr = Convert.ToInt32(paddingTop).ToString() + "px";

            // Calculations for gap label
            double tenorDiameter;
            double xPos;
            double yPos;
            double gapLabelLeftPos;
            double gapLabelTopPos;

            tenorDiameter = Diam.Diameter("T") * TestSpec.DiameterScale;

            if (Blow.IsHandstroke)
            {
                xPos = (Blow.GapCumulativeRow * TestSpec.XScale) + Screen.XMargin;
            }
            else
            {
                xPos = ((Blow.GapCumulativeRow + TestSpec.BaseGap) * TestSpec.XScale) + Screen.XMargin;
            }

            gapLabelLeftPos = xPos - (tenorDiameter / 2) - 11;

            gapLabelLeftPosStr = Convert.ToInt32(gapLabelLeftPos).ToString() + "px";

            yPos = (Blow.RowNum * TestSpec.YScale) + Screen.YMargin;

            gapLabelTopPos = yPos + (tenorDiameter / 2) + 1;
            gapLabelTopPosStr = Convert.ToInt32(gapLabelTopPos).ToString() + "px";

            gapLabelFontSizeStr = (TestSpec.FontSize - 2).ToString() + "px";

            if (Blow.IsHighlighted == true)
            {
                bgColor = "lightpink";

            }
            else
            {
                bgColor = "white";
            }

            gapStr = Blow.Gap.ToString();
        }

        async Task TestBellMouseDown(MouseEventArgs e)
        {
            // Mouse movement only active when the play button says Play (as opposed to
            // Stop or Wait), and the Play button is not disabled
            // The latter test is needed because when submitting, the play button says Play
            if (e.Buttons == 1 && Screen.PlayLabel == "Play" && Screen.PlayDisabled == false)
            {
                // Call TestBellMouseMove to center the bell on where the mouse is clicked
                await TestBellMouseMove(e);
            }
        }

        async Task TestBellMouseMove(MouseEventArgs e)
        {
            // Mouse movement only active when the play button says Play (as opposed to
            // Stop or Wait), and the Play button is not disabled
            // The latter test is needed because when submitting, the play button says Play
            if (e.Buttons == 1 && Screen.PlayLabel == "Play" && Screen.PlayDisabled == false)
            {
                int clientX = Convert.ToInt32(e.ClientX);

                int newGapCumulativeRow;

                if (Blow.IsHandstroke)
                {
                    newGapCumulativeRow = Convert.ToInt32((clientX - Screen.XMargin) / TestSpec.XScale);
                }
                else
                {
                    newGapCumulativeRow = Convert.ToInt32((clientX - Screen.XMargin) / TestSpec.XScale) -
                             TestSpec.BaseGap;
                }

                int newGap = Blow.Gap + (newGapCumulativeRow - Blow.GapCumulativeRow);
                int newGapRounded = Convert.ToInt32((double)newGap / Constants.Rounding) * Constants.Rounding;

                if (newGapRounded >= TestSpec.GapMin && newGapRounded <= TestSpec.GapMax &&
                        newGapRounded != Blow.Gap)
                {
                    Blow.UpdateGap(newGapRounded);
                    await Callback.InvokeAsync(CallbackParam.MouseMove);
                }
            }
        }
    }
}
