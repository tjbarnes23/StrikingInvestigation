using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrikingInvestigation.Utilities
{
    public static class ScreenSizing
    {
        public static double DiameterScale(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 0.4;
            }
            else if (browserWidth < 768)
            {
                return 0.55;
            }
            else if (browserWidth < 992)
            {
                return 0.7;
            }
            else if (browserWidth < 1200)
            {
                return 0.85;
            }
            else
            {
                return 1;
            }
        }

        public static double DiameterScaleAV(int browserWidth)
        {
            if (browserWidth < 576)
            {
                // *4
                return 1.6;
            }
            else if (browserWidth < 768)
            {
                // *3.5
                return 1.92;
            }
            else if (browserWidth < 992)
            {
                // *3
                return 2.1;
            }
            else if (browserWidth < 1200)
            {
                // *2.5
                return 2.12;
            }
            else
            {
                // *2
                return 2;
            }
        }

        public static double XScale(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 0.11;
            }
            else if (browserWidth < 768)
            {
                return 0.16;
            }
            else if (browserWidth < 992)
            {
                return 0.22;
            }
            else if (browserWidth < 1200)
            {
                return 0.29;
            }
            else
            {
                return 0.35;
            }
        }

        public static int XMargin(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 30;
            }
            else if (browserWidth < 768)
            {
                return 35;
            }
            else if (browserWidth < 992)
            {
                return 42;
            }
            else if (browserWidth < 1200)
            {
                return 50;
            }
            else
            {
                return 60;
            }
        }

        public static double YScale(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 50;
            }
            else if (browserWidth < 768)
            {
                return 58;
            }
            else if (browserWidth < 992)
            {
                return 66;
            }
            else if (browserWidth < 1200)
            {
                return 74;
            }
            else
            {
                return 82;
            }
        }

        public static int YMargin(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 0;
            }
            else if (browserWidth < 768)
            {
                return 0;
            }
            else if (browserWidth < 992)
            {
                return 0;
            }
            else if (browserWidth < 1200)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static int YMarginB(int browserWidth)
        {
            double margin;
            double tenorDiameter;

            margin = (YScale(browserWidth) * 4) + YMargin(browserWidth);

            tenorDiameter = Diam.Diameter("T") * DiameterScale(browserWidth);

            margin += (tenorDiameter / 2) + 1 + (FontSize(browserWidth) - 2) + FontPaddingTop(browserWidth) + 10 + 45;

            return Convert.ToInt32(margin);
        }

        public static int BorderWidth(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 3;
            }
            else if (browserWidth < 768)
            {
                return 4;
            }
            else if (browserWidth < 992)
            {
                return 4;
            }
            else if (browserWidth < 1200)
            {
                return 5;
            }
            else
            {
                return 5;
            }
        }

        public static int BorderWidthAV(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 6;
            }
            else if (browserWidth < 768)
            {
                return 6;
            }
            else if (browserWidth < 992)
            {
                return 7;
            }
            else if (browserWidth < 1200)
            {
                return 7;
            }
            else
            {
                return 7;
            }
        }

        public static int FontSize(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 10;
            }
            else if (browserWidth < 768)
            {
                return 11;
            }
            else if (browserWidth < 992)
            {
                return 12;
            }
            else if (browserWidth < 1200)
            {
                return 13;
            }
            else
            {
                return 14;
            }
        }

        public static int FontPaddingTop(int browserWidth)
        {
            // Font padding top is about 25% of font size
            if (browserWidth < 576)
            {
                return 3;
            }
            else if (browserWidth < 768)
            {
                return 3;
            }
            else if (browserWidth < 992)
            {
                return 3;
            }
            else if (browserWidth < 1200)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public static int StrokeLabelXOffset(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return -20;
            }
            else if (browserWidth < 768)
            {
                return -25;
            }
            else if (browserWidth < 992)
            {
                return -30;
            }
            else if (browserWidth < 1200)
            {
                return -35;
            }
            else
            {
                return -40;
            }
        }

        public static int StrokeLabelYOffset(int browserWidth)
        {
            // Stroke label is 150% of font size. Fonts have a top padding of about 25% of the font size
            // So StrokeLabelYOffset is 50% of (150% of font size) + 25% of (150 % of font size)
            // = 112.5% of font size
            if (browserWidth < 576)
            {
                // Font size is 10
                return -10;
            }
            else if (browserWidth < 768)
            {
                // Font size is 11
                return -11;
            }
            else if (browserWidth < 992)
            {
                // Font size is 12
                return -13;
            }
            else if (browserWidth < 1200)
            {
                // Font size is 13
                return -14;
            }
            else
            {
                // Font size is 14
                return -15;
            }
        }

        public static int RowStartLabelWidth(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 3;
            }
            else if (browserWidth < 768)
            {
                return 4;
            }
            else if (browserWidth < 992)
            {
                return 4;
            }
            else if (browserWidth < 1200)
            {
                return 5;
            }
            else
            {
                return 5;
            }
        }

        public static int RowStartLabelHeight(int browserWidth)
        {
            double height;
            
            // Row start label has the same height as the tenor
            height = Diam.Diameter("T") * DiameterScale(browserWidth);
            return Convert.ToInt32(height);
        }

        public static int ChangeLabelXOffset(int browserWidth)
        {
            double offset;

            // ChangeLabelXOffset is (RowStartLabelWidth / 2) + 10
            offset = (RowStartLabelWidth(browserWidth) / (double)2) + 10;
            return Convert.ToInt32(offset);
        }

        public static int ChangeLabelYOffset(int browserWidth)
        {
            double offset;

            // ChangeLabelYOffset is (50% of YScale) + (50% of font size) + FontPaddingTop
            offset = (((YScale(browserWidth) + FontSize(browserWidth)) * 0.5) + FontPaddingTop(browserWidth)) * -1;
            return Convert.ToInt32(offset);
        }
    }
}
