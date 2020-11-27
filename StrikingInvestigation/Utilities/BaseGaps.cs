using System;

namespace StrikingInvestigation.Utilities
{
    public static class BaseGaps
    {
        public static int BaseGap(int stage, int tenorWeight, int round)
        {
            int baseGap;

            switch (stage)
            {
                case 5:
                case 6:
                    baseGap = 284;
                    break;

                case 7:
                case 8:
                    if (tenorWeight == 8)
                    {
                        baseGap = 235;
                    }
                    else
                    {
                        baseGap = 273;
                    }
                    break;

                case 9:
                case 10:
                    baseGap = 227;
                    break;

                case 11:
                case 12:
                    baseGap = 195;
                    break;

                default:
                    baseGap = 200;
                    break;
            }

            if (round > 1)
            {
                baseGap = Convert.ToInt32((double)baseGap / round) * round;
            }

            return baseGap;
        }
    }
}
