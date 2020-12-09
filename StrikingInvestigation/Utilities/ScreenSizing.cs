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
                return 0.46;
            }
            else if (browserWidth < 768)
            {
                return 0.61;
            }
            else if (browserWidth < 992)
            {
                return 0.80;
            }
            else if (browserWidth < 1200)
            {
                return 0.98;
            }
            else
            {
                return 1.22;
            }
        }

        public static double XScale(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 0.15;
            }
            else if (browserWidth < 768)
            {
                return 0.2;
            }
            else if (browserWidth < 992)
            {
                return 0.26;
            }
            else if (browserWidth < 1200)
            {
                return 0.32;
            }
            else
            {
                return 0.4;
            }
        }

        public static int XMargin(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 40;
            }
            else if (browserWidth < 768)
            {
                return 45;
            }
            else if (browserWidth < 992)
            {
                return 50;
            }
            else if (browserWidth < 1200)
            {
                return 55;
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
                return 60;
            }
            else if (browserWidth < 992)
            {
                return 70;
            }
            else if (browserWidth < 1200)
            {
                return 80;
            }
            else
            {
                return 90;
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
            if (browserWidth < 576)
            {
                return 250;
            }
            else if (browserWidth < 768)
            {
                return 300;
            }
            else if (browserWidth < 992)
            {
                return 350;
            }
            else if (browserWidth < 1200)
            {
                return 400;
            }
            else
            {
                return 450;
            }
        }

        public static int BorderWidth(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 4;
            }
            else if (browserWidth < 768)
            {
                return 5;
            }
            else if (browserWidth < 992)
            {
                return 6;
            }
            else if (browserWidth < 1200)
            {
                return 7;
            }
            else
            {
                return 8;
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
            if (browserWidth < 576)
            {
                return -14;
            }
            else if (browserWidth < 768)
            {
                return -15;
            }
            else if (browserWidth < 992)
            {
                return -16;
            }
            else if (browserWidth < 1200)
            {
                return -17;
            }
            else
            {
                return -18;
            }
        }

        public static int RowStartLabelWidth(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return 4;
            }
            else if (browserWidth < 768)
            {
                return 4;
            }
            else if (browserWidth < 992)
            {
                return 5;
            }
            else if (browserWidth < 1200)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        public static int RowStartLabelHeight(int browserWidth)
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
                return 40;
            }
            else if (browserWidth < 1200)
            {
                return 45;
            }
            else
            {
                return 50;
            }
        }

        public static int ChangeLabelXOffset(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return -10;
            }
            else if (browserWidth < 768)
            {
                return -15;
            }
            else if (browserWidth < 992)
            {
                return -20;
            }
            else if (browserWidth < 1200)
            {
                return -25;
            }
            else
            {
                return -30;
            }
        }

        public static int ChangeLabelYOffset(int browserWidth)
        {
            if (browserWidth < 576)
            {
                return -22;
            }
            else if (browserWidth < 768)
            {
                return -32;
            }
            else if (browserWidth < 992)
            {
                return -42;
            }
            else if (browserWidth < 1200)
            {
                return -52;
            }
            else
            {
                return -62;
            }
        }
    }
}
